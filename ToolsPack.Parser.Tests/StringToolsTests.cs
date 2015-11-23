using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsPack.Displayer;
using log4net;
using ToolsPack.Log4net;

namespace ToolsPack.Parser.Tests
{
	[TestClass()]
	public class StringToolsTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(AlignedStringListToTable));

		[TestInitialize]
		public void SetUp()
		{
			Log4NetQuickSetup.SetUpConsole();
		}

		[TestMethod()]
		public void GetSubstringTest()
		{
			Assert.AreEqual("0123", StringTools.GetSubstring("0123456789", 0, 4));
			Assert.AreEqual("34567", StringTools.GetSubstring("0123456789", 3, 8));
			Assert.AreEqual("78", StringTools.GetSubstring("0123456789", 7, 9));
			Assert.AreEqual("789", StringTools.GetSubstring("0123456789", 7, 15));
			Assert.AreEqual(string.Empty, StringTools.GetSubstring("0123456789", 12, 15));
		}

		[TestMethod()]
		public void SplitTest()
		{
			//standard case
			{
				var cells = StringTools.Split("0123456789", new SortedSet<int>() { 3, 7 });
				Assert.AreEqual("{ 012, 3456, 789 }", cells.Display().ToString());
			}

			//with some empty column
			{
				var cells = StringTools.Split("0123456789", new SortedSet<int>() { 3, 7, 15, 100 });
				Assert.AreEqual("{ 012, 3456, 789, ,  }", cells.Display().ToString());
			}

			//empty column breaks
			{
				var cells = StringTools.Split("0123456789", new SortedSet<int>());
				Assert.AreEqual("0123456789", cells.Display().ToString());
			}
		}
   }
}