using System.Threading;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToolsPack.Log4net.Tests
{
	[TestClass]
	public class ElapsedTimeWatcherTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (ElapsedTimeWatcherTests));

		[ClassInitialize]
		public static void SetUp(TestContext context)
		{
			Log4NetQuickSetup.SetUpConsole();
		}

		[TestMethod]
		public void CreateTest()
		{
			using (ElapsedTimeWatcher etw = ElapsedTimeWatcher.Create(Log, "checkIntraday").AutoJump(150, 250))
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
	}
}