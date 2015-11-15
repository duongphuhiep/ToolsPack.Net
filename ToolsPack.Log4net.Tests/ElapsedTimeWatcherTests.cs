using System;
using System.Globalization;
using System.Threading;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolsPack.Log4net;

namespace ToolsPack.Log4net.Tests
{
	[TestClass]
	public class ElapsedTimeWatcherTests
	{
		private static readonly ILog Log = log4net.LogManager.GetLogger(typeof(ElapsedTimeWatcherTests));
		private ElapsedTimeWatcher _etw;

		[TestInitialize]
		public void SetUp()
		{
			Log4NetQuickSetup.SetUpConsole();
			_etw = ElapsedTimeWatcher.Create(Log, "ElapsedTimeWatcherTests");
		}

		[TestMethod]
		public void ElapsedTimeWatcherTest()
		{
			using (var etw = ElapsedTimeWatcher.Create(Log, "checkIntraday"))
			{
				Thread.Sleep(100);
				etw.Debug("step 1");
				Thread.Sleep(200);
				etw.Debug("step 2");
				Thread.Sleep(300);
				etw.Info("final step)");
				Thread.Sleep(400);
			}
		}

		[TestMethod]
		public void LastLogLevelTest()
		{
			using (var etw = ElapsedTimeWatcher.Create(Log, "checkIntraday").InfoEnd())
			{
				Thread.Sleep(170);
				etw.Debug("step 1");
				Thread.Sleep(260);
				etw.Debug("step 2");
				Thread.Sleep(40);
				etw.Debug("final step)");
				Thread.Sleep(400);
			} //expect last log is a info
		}

		[TestMethod]
		public void AutoJumpTest()
		{
			using (var etw = ElapsedTimeWatcher.Create(Log, "checkIntraday").InfoEnd().AutoJump(150, 250).AutoJumpLastLog(10, 20))
			{
				Thread.Sleep(170);
				etw.Debug("step 1"); //expect a info here
				Thread.Sleep(260);
				etw.Debug("step 2"); //expect a warning here
				Thread.Sleep(40);
				etw.Debug("final step)"); 
				Thread.Sleep(400);
			} //expect last log is a warning
		}

	
		[TestMethod]
		public void AutoJumpLastLogTest()
		{
			ElapsedTimeWatcher.Create(Log, "").AutoJumpLastLog(10, 15);
		}

		
		[TestMethod]
		public void LevelEndTest()
		{
			ElapsedTimeWatcher.Create(Log, "").LevelEnd(ElapsedTimeWatcher.LoggerLevel.Warn);
		}

		[TestMethod]
		public void LevelBeginTest()
		{
			ElapsedTimeWatcher.Create(Log, "").LevelBegin(ElapsedTimeWatcher.LoggerLevel.Error);
		}

		[TestMethod]
		public void InfoEndTest()
		{
			ElapsedTimeWatcher.Create(Log, "").InfoEnd();
		}

		[TestMethod]
		public void DebugEndTest()
		{
			ElapsedTimeWatcher.Create(Log, "").DebugEnd();
		}

		[TestMethod]
		public void InfoBeginTest()
		{
			ElapsedTimeWatcher.Create(Log, "").InfoBegin();
		}

		[TestMethod]
		public void DebugBeginTest()
		{
			ElapsedTimeWatcher.Create(Log, "").DebugBegin();
		}

		[TestMethod]
		public void RestartScopeStopwatchTest()
		{
			ElapsedTimeWatcher.Create(Log, "").RestartScopeStopwatch();
		}

		[TestMethod]
		public void RestartTest()
		{
			ElapsedTimeWatcher.Create(Log, "").Restart();
		}

		[TestMethod]
		public void DebugTest()
		{
			_etw.Debug("");
			_etw.Debug("", new Exception());
			_etw.DebugFormat("{0}", 1);
			_etw.DebugFormat("{0} {1}", 0, 1);
			_etw.DebugFormat("{0} {1} {2}", 0, 1, 2);
			_etw.DebugFormat("{0} {1} {2} {3}", 0, 1, 2, 3);
			_etw.DebugFormat(CultureInfo.CurrentCulture, "{0} {1} {2}", 0, 1, 2);

			_etw.AutoJump(1, 2).AutoJumpLastLog(1, 2);
			_etw.Debug("");
			_etw.Debug("", new Exception());
			_etw.DebugFormat("{0}", 1);
			_etw.DebugFormat("{0} {1}", 0, 1);
			_etw.DebugFormat("{0} {1} {2}", 0, 1, 2);
			_etw.DebugFormat("{0} {1} {2} {3}", 0, 1, 2, 3);
			_etw.DebugFormat(CultureInfo.CurrentCulture, "{0} {1} {2}", 0, 1, 2);

			_etw.Dispose();
		}

		[TestMethod]
		public void InfoTest()
		{
			_etw.Info("");
			_etw.Info("", new Exception());
			_etw.InfoFormat("{0}", 1);
			_etw.InfoFormat("{0} {1}", 0, 1);
			_etw.InfoFormat("{0} {1} {2}", 0, 1, 2);
			_etw.InfoFormat("{0} {1} {2} {3}", 0, 1, 2, 3);
			_etw.InfoFormat(CultureInfo.CurrentCulture, "{0} {1} {2}", 0, 1, 2);

			_etw.AutoJump(1, 2).AutoJumpLastLog(1, 2);
			_etw.Info("");
			_etw.Info("", new Exception());
			_etw.InfoFormat("{0}", 1);
			_etw.InfoFormat("{0} {1}", 0, 1);
			_etw.InfoFormat("{0} {1} {2}", 0, 1, 2);
			_etw.InfoFormat(CultureInfo.CurrentCulture, "{0} {1} {2}", 0, 1, 2);

			_etw.Dispose();
		}

		[TestMethod]
		public void WarnTest()
		{
			_etw.Warn("");
			_etw.Warn("", new Exception());
			_etw.WarnFormat("{0}", 1);
			_etw.WarnFormat("{0} {1}", 0, 1);
			_etw.WarnFormat("{0} {1} {2}", 0, 1, 2);
			_etw.WarnFormat("{0} {1} {2} {3}", 0, 1, 2, 3);
			_etw.WarnFormat(CultureInfo.CurrentCulture, "{0} {1} {2}", 0, 1, 2);
		}

		[TestMethod]
		public void ErrorTest()
		{
			_etw.Error("");
			_etw.Error("", new Exception());
			_etw.ErrorFormat("{0}", 1);
			_etw.ErrorFormat("{0} {1}", 0, 1);
			_etw.ErrorFormat("{0} {1} {2}", 0, 1, 2);
			_etw.ErrorFormat("{0} {1} {2} {3}", 0, 1, 2, 3);
			_etw.ErrorFormat(CultureInfo.CurrentCulture, "{0} {1} {2}", 0, 1, 2);
		}

		[TestMethod]
		public void FatalTest()
		{
			_etw.Fatal("");
			_etw.Fatal("", new Exception());
			_etw.FatalFormat("{0}", 1);
			_etw.FatalFormat("{0} {1}", 0, 1);
			_etw.FatalFormat("{0} {1} {2}", 0, 1, 2);
			_etw.FatalFormat("{0} {1} {2} {3}", 0, 1, 2, 3);
			_etw.FatalFormat(CultureInfo.CurrentCulture, "{0} {1} {2}", 0, 1, 2);
		}
	}
}
