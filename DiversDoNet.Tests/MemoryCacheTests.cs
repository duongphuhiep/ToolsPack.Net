using System;
using System.Runtime.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiversDoNet.Tests
{
	[TestClass]
	public class MemoryCacheTests
	{
		[TestMethod]
		public void TestBasic()
		{
			var memoryCache = new MemoryCache("Foo");
			var policy = new CacheItemPolicy();
			memoryCache.AddOrGetExisting("Pop", 123, DateTimeOffset.MaxValue);
			memoryCache.AddOrGetExisting("Top", "Gun", DateTimeOffset.MaxValue);

			memoryCache.Add("Pop", 12, DateTime.MaxValue);

			Assert.AreEqual("Gun", memoryCache.Get("Top"));
			Assert.AreEqual(123, memoryCache.Get("Pop"));
		}
	}
}
