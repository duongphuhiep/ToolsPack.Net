using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToolsPack.Displayer.Tests
{
	[TestClass]
	public class StopwatchDisplayerTests
	{
		[TestMethod]
		public void GetElapsedStringTest()
		{
			Assert.AreEqual("1.7 s", StopwatchDisplayer.GetElapsedString(1.67));
			Assert.AreEqual("9.2 s", StopwatchDisplayer.GetElapsedString(9.23));
			Assert.AreEqual("1670 ms", StopwatchDisplayer.GetElapsedString(1.67, StopwatchDisplayer.TimeUnit.MiliSecond));
			Assert.AreEqual("9230 ms", StopwatchDisplayer.GetElapsedString(9.23, StopwatchDisplayer.TimeUnit.MiliSecond));
			Assert.AreEqual("1670000 mcs", StopwatchDisplayer.GetElapsedString(1.67, StopwatchDisplayer.TimeUnit.MicroSecond));
			Assert.AreEqual("9230000 mcs", StopwatchDisplayer.GetElapsedString(9.23, StopwatchDisplayer.TimeUnit.MicroSecond));

			Assert.AreEqual("1 ms", StopwatchDisplayer.GetElapsedString(0.0011));
			Assert.AreEqual("10 ms", StopwatchDisplayer.GetElapsedString(0.0099));
			Assert.AreEqual("1.1 ms", StopwatchDisplayer.GetElapsedString(0.0011, StopwatchDisplayer.TimeUnit.MiliSecond));
			Assert.AreEqual("9.9 ms", StopwatchDisplayer.GetElapsedString(0.0099, StopwatchDisplayer.TimeUnit.MiliSecond));

			Assert.AreEqual("1 mcs", StopwatchDisplayer.GetElapsedString(0.0000011));
			Assert.AreEqual("10 mcs", StopwatchDisplayer.GetElapsedString(0.0000099));
			Assert.AreEqual("0 ms", StopwatchDisplayer.GetElapsedString(0.0000041, StopwatchDisplayer.TimeUnit.MiliSecond));
			Assert.AreEqual("0.01 ms", StopwatchDisplayer.GetElapsedString(0.0000051, StopwatchDisplayer.TimeUnit.MiliSecond));
			Assert.AreEqual("0.18 ms", StopwatchDisplayer.GetElapsedString(0.0001799, StopwatchDisplayer.TimeUnit.MiliSecond));
			Assert.AreEqual("5 mcs", StopwatchDisplayer.GetElapsedString(0.0000051, StopwatchDisplayer.TimeUnit.MicroSecond));
			Assert.AreEqual("180 mcs", StopwatchDisplayer.GetElapsedString(0.0001799, StopwatchDisplayer.TimeUnit.MicroSecond));

			Assert.AreEqual("1 ns", StopwatchDisplayer.GetElapsedString(0.0000000011));
			Assert.AreEqual("10 ns", StopwatchDisplayer.GetElapsedString(0.0000000099));
			Assert.AreEqual("0 ms", StopwatchDisplayer.GetElapsedString(0.0000000011, StopwatchDisplayer.TimeUnit.MiliSecond));
			Assert.AreEqual("0 ms", StopwatchDisplayer.GetElapsedString(0.0000000099, StopwatchDisplayer.TimeUnit.MiliSecond));
		}
	}
}