using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DiversDoNet.Tests.model;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shielded;
using Shielded.ProxyGen;
using ToolsPack.Log4net;

namespace DiversDoNet.Tests
{
	[TestClass]
	public class ShieldedTransactionTest
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(ShieldedTransactionTest));

		[ClassInitialize]
		public static void SetUp(TestContext context)
		{
			Log4NetQuickSetup.SetUpConsole(Log4NetQuickSetup.CreatePattern("threadId"));
			//Factory.PrepareTypes(new[] { typeof(Exposure), typeof(Resa) });
		}

		private ShieldedDict<string, Exposure> exposureStore;
		private ShieldedDict<Resa.Key, Resa> resaStore;

		[TestInitialize]
		public void SetUpUnit()
		{
			Shield.InTransaction(() =>
			{
				//init exposure store
				exposureStore = new ShieldedDict<string, Exposure>();
				var e1 = Factory.NewShielded<Exposure>();
				{
					e1.Client = "C1";
					e1.Amount = 100;
				}
				var e2 = Factory.NewShielded<Exposure>();
				{
					e2.Client = "C1";
					e2.Amount = 200;
				}
				exposureStore["C1"] = e1;
				exposureStore["C2"] = e2;

				//init resa store
				resaStore = new ShieldedDict<Resa.Key, Resa>();
				var d1 = Factory.NewShielded<Resa>();
				{
					d1.DealId = "D1";
					d1.Client = "C1";
					d1.Amount = 10;
				}
				var d2 = Factory.NewShielded<Resa>();
				{
					d2.DealId = "D2";
					d2.Client = "C1";
					d2.Amount = 40;
				}
				var d3 = Factory.NewShielded<Resa>();
				{
					d3.DealId = "D3";
					d3.Client = "C2";
					d3.Amount = 60;
				}


				resaStore[d1.K] = d1;
				resaStore[d2.K] = d2;
				resaStore[d3.K] = d3;
			});
		}

		[TestMethod]
		public void BasicTest()
		{
			//confirm in transaction
			Shield.InTransaction(() =>
			{
				exposureStore["C1"].Amount = 150;
				resaStore.Remove(new Resa.Key("D1", "C1"));
				resaStore.Remove(new Resa.Key("D2", "C1"));
			});

			Assert.AreEqual(150, exposureStore["C1"].Amount);
			Assert.AreEqual(1, resaStore.Count);
		}

		[TestMethod]
		public void RollbackTest()
		{
			try
			{
				//confirm in transaction
				Shield.InTransaction(() =>
				{
					exposureStore["C1"].Amount = 150;
					resaStore.Remove(new Resa.Key("D1", "C1"));

					Assert.AreEqual(150, exposureStore["C1"].Amount);
					Assert.AreEqual(2, resaStore.Count);
					throwAnException();

					resaStore.Remove(new Resa.Key("D2", "C1"));
				});
				
			}
			catch (NotSupportedException)
			{
				Log.Info("Crash inside transaction");
			}

			Assert.AreEqual(100, exposureStore["C1"].Amount);
			Assert.AreEqual(3, resaStore.Count);
		}

		[TestMethod]
		public void MultiThreadTest()
		{
			Shield.InTransaction(() =>
			{
				var d4 = Factory.NewShielded<Resa>();
				{
					d4.DealId = "D4";
					d4.Client = "C2";
					d4.Amount = 80;
				}
				resaStore[d4.K] = d4;
			});

			var t1 = Task.Factory.StartNew(() =>
			{
				try
				{
					ThreadContext.Properties["threadId"] = "T1";
					Log.Info("Enter task 1");

					//confirm in transaction
					Shield.InTransaction(() =>
					{
						Log.Info("start transaction 1");
						exposureStore["C1"].Amount = 150;
						resaStore.Remove(new Resa.Key("D1", "C1"));

						//The value is set inside the transaction, not outside
						Assert.AreEqual(150, exposureStore["C1"].Amount);
						Assert.AreEqual(3, resaStore.Count);
						Log.Info("The transaction 1 is on dirty state");
						Thread.Sleep(100);

						resaStore.Remove(new Resa.Key("D2", "C1"));
						Log.Info("end transaction 1");
					});

					Log.Info("Finish task 1");
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
			});

			Thread.Sleep(50); //ensure that the t2 is execute after t1

			var t2 = Task.Factory.StartNew(() =>
			{
				try
				{
					ThreadContext.Properties["threadId"] = "T2";
					Log.Info("Enter task 2");

					//confirm in transaction
					Shield.InTransaction(() =>
					{
						Assert.AreEqual(100, exposureStore["C1"].Amount);
						Assert.AreEqual(4, resaStore.Count);
						Log.Info("The transaction 2 won't see any dirty state of the transaction 1");

						Log.Info("start transaction 2");
						exposureStore["C1"].Amount = 160;
						resaStore.Remove(new Resa.Key("D1", "C1"));

						//Log.Info("current exposure of C1 = " + exposureStore["C1"].Amount);
						//Log.Info("current resas of C1 = " + resaStore.Count);
						Log.Info("some value is changed in transaction 2");
						Thread.Sleep(100);

						resaStore.Remove(new Resa.Key("D3", "C2"));
						Log.Info("end transaction 2");
					});
					Log.Info("Finish task 2");
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
			});

			Assert.AreEqual(100, exposureStore["C1"].Amount);
			Assert.AreEqual(4, resaStore.Count);
			Log.Info("everything look normal outside the transaction");

			Task.WaitAll(t1, t2);
			Assert.AreEqual(160, exposureStore["C1"].Amount);
			Assert.AreEqual(1, resaStore.Count);
		}

		/// <summary>
		/// If dictionary key is an object, if the object content changed, we need to remove / re-add the item
		/// </summary>
		[TestMethod]
		public void DictionaryKeyChangeTest()
		{
			var r1 = new Resa("D1", "C1", 100);
			var r2 = new Resa("D2", "C2", 200);
			var store = new Dictionary<Resa.Key, Resa>() {{r1.K, r1}, {r2.K, r2}};

			//change key
			Assert.AreEqual(r1, store[new Resa.Key("D1", "C1")]);
			
			r1.Client = "C1bis"; //change key content of r1

			//we won't find the item r1 again
			Assert.IsFalse(store.ContainsKey(new Resa.Key("D1", "C1bis")));
			Assert.IsFalse(store.ContainsKey(new Resa.Key("D1", "C1")));

			//we must remove r1 first, change its key content, then re-add it to the dictionary
		}

		private void throwAnException()
		{
			throw new NotSupportedException();
		}
	}
}
