using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolsPack.Log4net;

namespace ToolsPack.Parser.Sqp.Tests
{
	[TestClass]
	public class SearchQueryParserTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (SearchQueryParserTests));

		[ClassInitialize]
		public static void SetUp(TestContext context)
		{
			Log4NetQuickSetup.SetUpConsole();
		}

		[TestMethod]
		public void ParseTest()
		{
			SearchQuery s = SearchQueryParser.Parse("strike>=3 -spot Bloomberg Paris");
			Assert.AreEqual(4, s.SearchQueryComponents.Count);
			Log.Info(s.ToString());
		}
	}
}