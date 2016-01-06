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
	public class AlignedStringListToTableTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(AlignedStringListToTable));

		[TestInitialize]
		public void SetUp()
		{
			Log4NetQuickSetup.SetUpConsole();
		}

		[TestMethod()]
		public void FindWeakColumnBreaksTest()
		{
			{
				var breakPos = AlignedStringListToTable.FindWeakColumnBreaks("a    bcd   50 8      7").ToArray();
				Assert.AreEqual(2, breakPos.Length);
				Assert.AreEqual(5, breakPos[0]);
				Assert.AreEqual(21, breakPos[1]);
			}
			{
				var breakPos = AlignedStringListToTable.FindWeakColumnBreaks("ab bcd       50 88      7                   cde fg        hh").ToArray();
				Assert.AreEqual(4, breakPos.Length);
				Assert.AreEqual(13, breakPos[0]);
				Assert.AreEqual(24, breakPos[1]);
				Assert.AreEqual(44, breakPos[2]);
				Assert.AreEqual(58, breakPos[3]);
			}
		}

		[TestMethod()]
		public void FindNormalColumnBreaksTest()
		{
			{
				SortedSet<int>[] weaks = new[] {
					new SortedSet<int>() { 21, 49, 72, 54 },
					new SortedSet<int>() { 21, 44, 72, 56 },
					new SortedSet<int>() { 21, 43, 70, 77 },
				};
				var breakPos = AlignedStringListToTable.FindNormalColumnBreaks(weaks).ToArray();
				Assert.AreEqual(2, breakPos.Length);
				Assert.AreEqual(21, breakPos[0]);
				Assert.AreEqual(72, breakPos[1]);
			}
			{
				SortedSet<int>[] weaks = new[] {
					new SortedSet<int>() { 21, 49, 72, 54 },
					new SortedSet<int>() { 21, 44, 72, 56 },
					new SortedSet<int>() { 21, 43, 70, 54 },
				};
				var breakPos = AlignedStringListToTable.FindNormalColumnBreaks(weaks).ToArray();
				Assert.AreEqual(3, breakPos.Length);
				Assert.AreEqual(21, breakPos[0]);
				Assert.AreEqual(54, breakPos[1]);
				Assert.AreEqual(72, breakPos[2]);
			}
		}

		[TestMethod()]
		public void GetNeareastPositionTest()
		{
			var arr = new SortedSet<int>() { 1, 7, 8, 20, 79, 201 };

			int nearestPos, minDist;
			AlignedStringListToTable.GetNeareastPosition(arr, 3, out nearestPos, out minDist);
			Assert.AreEqual(1, nearestPos);

			AlignedStringListToTable.GetNeareastPosition(arr, 5, out nearestPos, out minDist);
			Assert.AreEqual(7, nearestPos);

			AlignedStringListToTable.GetNeareastPosition(arr, 14, out nearestPos, out minDist);
			Assert.AreEqual(8, nearestPos);

			AlignedStringListToTable.GetNeareastPosition(arr, 90, out nearestPos, out minDist);
			Assert.AreEqual(79, nearestPos);
		}

		[TestMethod()]
		public void FindHardColumnBreaksTest()
		{
			{
				SortedSet<int> normal = new SortedSet<int>() { 3, 40, 55, 100 };
				SortedSet<int> weak = new SortedSet<int>() { 8, 42, 55, 78, 96 };

				var hard = AlignedStringListToTable.FindHardColumnBreaks(normal, weak, 4);
				Assert.AreEqual("{ 3, 42, 55, 96 }", hard.Display().ToString());
			}

			//unodred list
			{
				SortedSet<int> normal = new SortedSet<int>() { 3, 55, 40, 100 };
				SortedSet<int> weak = new SortedSet<int>() { 55, 42, 96, 8, 78 };

				var hard = AlignedStringListToTable.FindHardColumnBreaks(normal, weak, 4);
				Assert.AreEqual("{ 3, 42, 55, 96 }", hard.Display().ToString());
			}
		}

		[TestMethod()]
		public void PorcessTest()
		{
			var src = new[] {
				"aa aa       15       aaaaaa      aa          17",
				"b bbbb      165      bb          bbb 26c   18",
				"cccc        18       ccc ceerergfcc 29  27   28",
				"dddd dd eeee19       dd          gggg 2927   25"
			};

			var expectedResult = new[] {
				new[]{ "aa aa", "15", "aaaaaa", "aa", "17"},
				new[]{ "b bbbb", "165", "bb", "bbb 26c", "18"},
				new[]{ "cccc", "18", "ccc ceerergf", "cc 29  27", "28"},
				new[]{ "dddd dd eeee", "19", "dd", "gggg 2927", "25"}
			};

			var converter = new AlignedStringListToTable(src, true, 3);
			var resu = converter.Process();

			for (int i = 0; i < expectedResult.Length; i++)
			{
				var row = expectedResult[i];
				for (int j = 0; j < row.Length; j++)
				{
					Assert.AreEqual(row[j], resu[i][j]);
				}
			}
		}

		[TestMethod()]
		public void GetNeareastPositionTest1()
		{
			int nearestPos;
			int minDist;
			AlignedStringListToTable.GetNeareastPosition(new SortedSet<int>() { }, 15, out nearestPos, out minDist);
			Assert.AreEqual(0, nearestPos);
			Assert.AreEqual(15, minDist);
		}
	}
}