using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToolsPack.Thread.Tests
{
	[TestClass]
	public class MultiNamedTimedLockerTests
	{
		[TestMethod]
		public void MultiNamedTimedLockerTest()
		{
			var locker = new MultiNamedTimedLocker<string>();

			using (locker.Lock(new[] {"hiep"}, 100))
			{
			}
			Assert.AreEqual(0, locker.CountLockedNames());

			using (locker.Lock(new[] {"hiep", "nhu", "hang"}, 100))
			{
			}
			Assert.AreEqual(0, locker.CountLockedNames());
		}

		[TestMethod]
		public void MultiNamedTimedLocker1Test()
		{
			//Test lock on same key "hiep"

			var codeFlow = new StringBuilder();

			var locker = new MultiNamedTimedLocker<string>();

			Task t1 = Task.Factory.StartNew(() =>
			{
				codeFlow.Append("0");
				System.Threading.Thread.Sleep(20);
				using (locker.Lock(new[] {"hiep", "nhu"}, 100))
				{
					//t1 acquired lock before t2
					codeFlow.Append("2");
					System.Threading.Thread.Sleep(100);
					codeFlow.Append("3"); //release lock
				}
				;
			});

			System.Threading.Thread.Sleep(10); //ensure that t2 is entered after t1 ("01")
			Task t2 = Task.Factory.StartNew(() =>
			{
				codeFlow.Append("1");
				System.Threading.Thread.Sleep(20);
				using (locker.Lock(new[] {"nhu", "quyen"}, 200))
				{
					//t1 has finished
					codeFlow.Append("4");
					System.Threading.Thread.Sleep(100);
				}
			});

			Task.WaitAll(t1, t2);
			Assert.AreEqual("01234", codeFlow.ToString());
			Assert.AreEqual(0, locker.CountLockedNames());
		}

		[TestMethod]
		public void MultiNamedTimedLocker2Test()
		{
			//Test lock on the different key "hiep", "nhu"

			var codeFlow = new StringBuilder();

			var locker = new MultiNamedTimedLocker<string>();

			Task t1 = Task.Factory.StartNew(() =>
			{
				codeFlow.Append("0");
				System.Threading.Thread.Sleep(50);
				using (locker.Lock(new[] {"hiep", "nhu"}, 100))
				{
					codeFlow.Append("2");
					System.Threading.Thread.Sleep(100);
				}
				codeFlow.Append("4"); //marked t1 finished
			});

			System.Threading.Thread.Sleep(20); //ensure that t2 is entered at the same time as t1 ("01")
			Task t2 = Task.Factory.StartNew(() =>
			{
				codeFlow.Append("1");
				System.Threading.Thread.Sleep(50);
				using (locker.Lock(new[] {"quyen", "hoanganh"}, 100))
				{
					//t2 entered the lock just after t1 entered the lock (the 2 lock is different)
					codeFlow.Append("3");
					System.Threading.Thread.Sleep(100);
				}
				;
			});

			Task.WaitAll(t1, t2);
			Assert.AreEqual("01234", codeFlow.ToString());
			Assert.AreEqual(0, locker.CountLockedNames());
		}
	}
}