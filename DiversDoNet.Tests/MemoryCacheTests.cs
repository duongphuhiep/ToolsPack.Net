using System;
using System.Runtime.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiversDoNet.Tests
{
	[TestClass]
	public class MemoryCacheTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			var memoryCache = new MemoryCache("Foo");
			CacheItemPolicy pưolicy = new CacheItemPolicy();
			memoryCache.AddOrGetExisting("Pop", 123, DateTimeOffset.MaxValue);

		}
	}
}
