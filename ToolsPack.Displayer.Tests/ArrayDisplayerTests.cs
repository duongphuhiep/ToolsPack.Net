using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ToolsPack.Log4net;
using ToolsPack.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ToolsPack.Parser.Tests
{
	[TestClass()]
	public class ArrayDisplayerTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(ArrayDisplayerTests));

		[ClassInitialize]
		public static void SetUp(TestContext context)
		{
			Log4NetQuickSetup.SetUpConsole();
		}

		[TestMethod()]
		public void ToStringTest()
		{
			var arr = new[] {"hiep", "duong", "hoai", "trang", "nhu", "linh", "hoa"};
			Log.Info(arr.Display().SeparatedBy(Environment.NewLine).MaxItems(5));
		}

	}
}
