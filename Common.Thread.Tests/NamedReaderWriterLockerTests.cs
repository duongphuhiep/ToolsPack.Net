using System.Threading.Tasks;
using Common.Log4net;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Thread.Tests
{
	[TestClass]
	public class NamedReaderWriterLockerTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (NamedReaderWriterLockerTests));

		private readonly NamedReaderWriterLocker _namedReaderWriterLocker = new NamedReaderWriterLocker();

		[ClassInitialize]
		public static void SetUp(TestContext context)
		{
			string log4netPattern = Log4NetQuickSetup.CreatePattern("Session");
			Log4NetQuickSetup.SetUpConsole(log4netPattern);
		}

		[TestMethod]
		public void WriteLockTest()
		{
			Task t1 = Task.Factory.StartNew(() =>
			{
				ThreadContext.Properties["Session"] = "T1";
				_namedReaderWriterLocker.RunWithWriteLock("hiep", () =>
				{
					Log.Info("Start write lock on hiep");
					System.Threading.Thread.Sleep(3000);
					Log.Info("End write lock on hiep");
				});
			});

			Task t2 = Task.Factory.StartNew(() =>
			{
				ThreadContext.Properties["Session"] = "T2";
				_namedReaderWriterLocker.RunWithWriteLock("hiep", () =>
				{
					Log.Info("Start write lock on hiep");
					System.Threading.Thread.Sleep(3000);
					Log.Info("End write lock on hiep");
				});
			});

			Task t3 = Task.Factory.StartNew(() =>
			{
				ThreadContext.Properties["Session"] = "T3";
				_namedReaderWriterLocker.RunWithWriteLock("mary", () =>
				{
					Log.Info("Start write lock on mary");
					System.Threading.Thread.Sleep(2000);
					Log.Info("End write lock on mary");
				});
			});

			Task.WaitAll(t1, t2, t3);
		}


		[TestMethod]
		public void WriteReadLockTest()
		{
			Task t1 = Task.Factory.StartNew(() =>
			{
				ThreadContext.Properties["Session"] = "T1";
				_namedReaderWriterLocker.RunWithWriteLock("hiep", () =>
				{
					Log.Info("Start write lock on hiep");
					System.Threading.Thread.Sleep(3000);
					Log.Info("End write lock on hiep");
				});
			});

			System.Threading.Thread.Sleep(500);

			Task t2 = Task.Factory.StartNew(() =>
			{
				ThreadContext.Properties["Session"] = "T2";
				_namedReaderWriterLocker.RunWithReadLock("hiep", () =>
				{
					Log.Info("Start read lock on hiep");
					System.Threading.Thread.Sleep(2000);
					Log.Info("End read lock on hiep");
				});
			});

			System.Threading.Thread.Sleep(500);

			Task t3 = Task.Factory.StartNew(() =>
			{
				ThreadContext.Properties["Session"] = "T3";
				_namedReaderWriterLocker.RunWithWriteLock("hiep", () =>
				{
					Log.Info("Start write lock on hiep");
					System.Threading.Thread.Sleep(3000);
					Log.Info("End write lock on hiep");
				});
			});

			System.Threading.Thread.Sleep(500);

			Task t4 = Task.Factory.StartNew(() =>
			{
				ThreadContext.Properties["Session"] = "T4";
				_namedReaderWriterLocker.RunWithReadLock("hiep", () =>
				{
					Log.Info("Start read lock on hiep");
					System.Threading.Thread.Sleep(2000);
					Log.Info("End read lock on hiep");
				});
			});

			Task.WaitAll(t1, t2, t3, t4);
		}
	}
}