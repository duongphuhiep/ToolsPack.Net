using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolsPack.Log4net;

namespace ToolsPack.Displayer.Tests
{
	[TestClass]
	public class ArrayDisplayerTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (ArrayDisplayerTests));

		[ClassInitialize]
		public static void SetUp(TestContext testContext)
		{
			Log4NetQuickSetup.SetUpConsole();
		}

		[TestMethod]
		public void DisplayTest()
		{
			var arr = new[] {"hoai", "trang", "nhu"};
			arr.Display().SeparatedByComma().MaxItemLength(100, ArrayDisplayer.WordEllipsis);
			arr.Display().SeparatedByNewLine().MaxItems(100).MaxItemLength(100, ArrayDisplayer.DefaultEllipsis);
		}

		[TestMethod]
		public void ArrayToStringEllipse()
		{
			{
				var a1 = new[] {"Lorem ipsum dolor sit amet", "consectetuer adipiscing elit"};
				Assert.AreEqual("{ [[Lor...]], [[con...]] }",
					ArrayDisplayer.DisplayMaxItem(a1, 2, ", ", ArrayDisplayer.DefaultEllipsis, 3));
				Assert.AreEqual("{ [[Lorem...]], [[consectetuer...]] }",
					ArrayDisplayer.DisplayMaxItem(a1, 2, ", ", ArrayDisplayer.WordEllipsis, 3));
				Assert.AreEqual("{ [[Lorem...]], ..and 1 (of 2) more }",
					ArrayDisplayer.DisplayMaxItem(a1, 1, ", ", ArrayDisplayer.WordEllipsis, 3));
				Assert.AreEqual("{ Lorem ipsum dolor sit amet, ..and 1 (of 2) more }",
					ArrayDisplayer.DisplayMaxItem(a1, 1, ", ", ArrayDisplayer.WordEllipsis, int.MaxValue));
				Assert.AreEqual("{ Lorem ipsum dolor sit amet, ..and 1 (of 2) more }",
					ArrayDisplayer.DisplayMaxItem(a1, 1, ", ", ArrayDisplayer.DefaultEllipsis, int.MaxValue));
			}
		}

		[TestMethod]
		public void ArrayToString2()
		{
			{
				int[] a1 = null;
				Assert.AreEqual("NULL", ArrayDisplayer.DisplayMaxItem(a1, 4));
			}
			{
				var a1 = new int[0];
				Assert.AreEqual("{}", ArrayDisplayer.DisplayMaxItem(a1, 4));
			}
			{
				var a1 = new int[10];
				Assert.AreEqual("{ 0, 0, 0, 0, ..and 6 (of 10) more }", ArrayDisplayer.DisplayMaxItem(a1, 4));
			}
			{
				var a1 = new object[10];
				Assert.AreEqual("{ NULL, NULL, NULL, NULL, ..and 6 (of 10) more }", ArrayDisplayer.DisplayMaxItem(a1, 4));
			}
			{
				var a1 = new[] {1};
				Assert.AreEqual("1", ArrayDisplayer.DisplayMaxItem(a1, 1));
			}
			{
				var a1 = new[] {(object) null};
				Assert.AreEqual("NULL", ArrayDisplayer.DisplayMaxItem(a1, 1));
			}
			{
				var a1 = new[] {1, 2};
				Assert.AreEqual("{ 1, ..and 1 (of 2) more }", ArrayDisplayer.DisplayMaxItem(a1, 1));
			}
			{
				var a1 = new[] {1, 2, 3};
				Assert.AreEqual("{ 1, ..and 2 (of 3) more }", ArrayDisplayer.DisplayMaxItem(a1, 1));
			}
			{
				var a1 = new[] {1, 2};
				Assert.AreEqual("{ 1, 2 }", ArrayDisplayer.DisplayMaxItem(a1, 2));
			}
			{
				var a1 = new[] {1, 2, 3};
				Assert.AreEqual("{ 1, 2, ..and 1 (of 3) more }", ArrayDisplayer.DisplayMaxItem(a1, 2));
			}
			{
				var a1 = new[] {1, 2, 3, 4};
				Assert.AreEqual("{ 1, 2, ..and 2 (of 4) more }", ArrayDisplayer.DisplayMaxItem(a1, 2));
			}
			{
				var a1 = new[] {1, 2};
				Assert.AreEqual("{ 1, 2 }", ArrayDisplayer.DisplayMaxItem(a1, 4));
			}
			{
				var a1 = new[] {1, 2, 3, 4};
				Assert.AreEqual("{ 1, 2, 3, 4 }", ArrayDisplayer.DisplayMaxItem(a1, 4));
			}
			{
				var a1 = new[] {1, 2, 3, 4, 5};
				Assert.AreEqual("{ 1, 2, 3, 4, ..and 1 (of 5) more }", ArrayDisplayer.DisplayMaxItem(a1, 4));
			}
			{
				var a1 = new[] {1, 2, 3, 4, 5};
				Assert.AreEqual("{ 1, 2, 3, 4, 5 }", ArrayDisplayer.DisplayMaxItem(a1, 0));
			}
			{
				var a1 = new[] {1, 2, 3, 4, 5};
				Assert.AreEqual("{ 1, 2, 3, 4, 5 }", ArrayDisplayer.DisplayMaxItem(a1, -1));
			}
			{
				var a1 = new[] {1, 2, 3, 4, 5};
				Assert.AreEqual("{ 1, 2, 3, 4, 5 }", ArrayDisplayer.DisplayMaxItem(a1, int.MaxValue));
			}
		}

		[TestMethod]
		public void ArrayToString()
		{
			{
				int[] a1 = null;
				Assert.AreEqual("NULL", ArrayDisplayer.DisplayMaxLength(a1, 1));
			}
			{
				var a1 = new int[0];
				Assert.AreEqual("{}", ArrayDisplayer.DisplayMaxLength(a1, 1));
			}
			{
				var a1 = new int[10];
				Assert.AreEqual("{ 0, 0, 0, ..and 7 (of 10) more }", ArrayDisplayer.DisplayMaxLength(a1, 9));
			}
			{
				var a1 = new object[10];
				Assert.AreEqual("{ NULL, NULL, NULL, NULL, ..and 6 (of 10) more }", ArrayDisplayer.DisplayMaxLength(a1, 24));
			}
			{
				var a1 = new[] {1};
				Assert.AreEqual("1", ArrayDisplayer.DisplayMaxLength(a1, 1));
			}
			{
				var a1 = new[] {(object) null};
				Assert.AreEqual("NULL", ArrayDisplayer.DisplayMaxLength(a1, 1));
			}
			{
				var a1 = new[] {1, 2};
				Assert.AreEqual("{ 1, ..and 1 (of 2) more }", ArrayDisplayer.DisplayMaxLength(a1, 1));
			}
			{
				var a1 = new[] {1, 2, 3};
				Assert.AreEqual("{ 1, ..and 2 (of 3) more }", ArrayDisplayer.DisplayMaxLength(a1, 1));
			}
			{
				var a1 = new[] {1, 2};
				Assert.AreEqual("{ 1, ..and 1 (of 2) more }", ArrayDisplayer.DisplayMaxLength(a1, 3));
			}
			{
				var a1 = new[] {1, 2, 3};
				Assert.AreEqual("{ 1, 2, ..and 1 (of 3) more }", ArrayDisplayer.DisplayMaxLength(a1, 6));
			}
			{
				var a1 = new[] {1, 2, 3, 4};
				Assert.AreEqual("{ 1, 2, ..and 2 (of 4) more }", ArrayDisplayer.DisplayMaxLength(a1, 6));
			}
			{
				var a1 = new[] {1, 2};
				Assert.AreEqual("{ 1, 2 }", ArrayDisplayer.DisplayMaxLength(a1, 7));
			}
			{
				var a1 = new[] {1, 2, 3, 4};
				Assert.AreEqual("{ 1, 2, 3, 4 }", ArrayDisplayer.DisplayMaxLength(a1, 13));
			}
			{
				var a1 = new[] {1, 2, 3, 4, 5};
				Assert.AreEqual("{ 1, 2, 3, 4, ..and 1 (of 5) more }", ArrayDisplayer.DisplayMaxLength(a1, 12));
			}
			{
				var a1 = new[] {1, 2, 3, 4, 5};
				Assert.AreEqual("{ 1, 2, 3, 4, 5 }", ArrayDisplayer.DisplayMaxLength(a1, 0));
			}
			{
				var a1 = new[] {1, 2, 3, 4, 5};
				Assert.AreEqual("{ 1, 2, 3, 4, 5 }", ArrayDisplayer.DisplayMaxLength(a1, -1));
			}
			{
				var a1 = new[] {1, 2, 3, 4, 5};
				Assert.AreEqual("{ 1, 2, 3, 4, 5 }", ArrayDisplayer.DisplayMaxLength(a1, int.MaxValue));
			}
		}
	}
}