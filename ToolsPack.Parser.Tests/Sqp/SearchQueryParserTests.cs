using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ToolsPack.Log4net;
using ToolsPack.Parser.Sqp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ToolsPack.Parser.Sqp.Tests
{
	[TestClass()]
	public class SearchQueryParserTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(SearchQueryParserTests));
		[ClassInitialize]
		public static void SetUp(TestContext context)
		{
			Log4NetQuickSetup.SetUpConsole();
		}

		[TestMethod()]
		public void ParseTest()
		{
			var s = SearchQueryParser.Parse("strike>=3 -spot Bloomberg Paris");
			Assert.AreEqual(4, s.SearchQueryComponents.Count);
			Log.Info(s.ToString());
		}
	}
}
