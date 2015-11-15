using C5;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolsPack.Displayer;
using ToolsPack.Log4net;
using SCG = System.Collections.Generic;

namespace ToolsPack.Collection.Tests
{
	[TestClass]
	public class CollectionTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (CollectionTests));

		[ClassInitialize]
		public static void SetUp(TestContext context)
		{
			Log4NetQuickSetup.SetUpConsole();
		}

		[TestMethod]
		public void ChildrenContainsTest()
		{
			var l = new ArrayList<double> {0, 1, 2, 3, 4, 5};
			IList<double> v1 = l.View(1, 3);
			Log.Info(v1.Display());
			v1.Add(4.5);
			Log.Info(l.Display());
		}
	}
}