using System;
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
			Log4NetQuickSetup.SetUpConsole();
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
					d1.DealId = "D2";
					d1.Client = "C1";
					d1.Amount = 40;
				}
				var d3 = Factory.NewShielded<Resa>();
				{
					d1.DealId = "D3";
					d1.Client = "C2";
					d1.Amount = 60;
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


		private void throwAnException()
		{
			throw new NotSupportedException();
		}
	}
}
