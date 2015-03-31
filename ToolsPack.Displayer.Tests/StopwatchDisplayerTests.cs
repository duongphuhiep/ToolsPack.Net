using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ToolsPack.Displayer;
using ToolsPack.Log4net;
using ToolsPack.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ToolsPack.Parser.Tests
{
	[TestClass()]
	public class StopwatchDisplayerTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(StopwatchDisplayerTests));

		private static Stopwatch sw;

		[ClassInitialize]
		public static void SetUp(TestContext context)
		{
			sw = Stopwatch.StartNew();
			System.Threading.Thread.Sleep(2000);
			sw.Stop();

			Log4NetQuickSetup.SetUpConsole();
		}

		[TestMethod()]
		public void DisplayTest()
		{
			Log.Info(sw.Display());
			Log.Info(sw.DisplayMili());
			Log.Info(sw.DisplayMicro());
		}
	}
}
