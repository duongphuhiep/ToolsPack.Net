﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsPack.Collection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToolsPack.Collection.Tests
{
	[TestClass]
	public class MultiKeyDictionaryTests
	{
		[TestMethod]
		public void MultiKeyDictionary2Test()
		{
			var dict = new MultiKeyDictionary<int, int, string>();
			dict[1, 1] = "hiep";
			Assert.IsTrue(dict.ContainsKey(1, 1));
			Assert.IsFalse(dict.Remove(1, 2));
			Assert.IsTrue(dict.Remove(1, 1));
			dict.Add(2, 1, "nhu");
		}

		[TestMethod]
		public void MultiKeyDictionary3Test()
		{
			var dict = new MultiKeyDictionary<int, int, int, string>();
			dict[1, 1, 1] = "hiep";
			Assert.IsTrue(dict.ContainsKey(1, 1, 1));
			Assert.IsFalse(dict.Remove(1, 2, 1));
			Assert.IsTrue(dict.Remove(1, 1, 1));
			dict.Add(2, 1, 1, "nhu");
			dict.Add(2, 2, 1, "nhu");
		}

		[TestMethod]
		public void MultiKeyDictionary4Test()
		{
			var dict = new MultiKeyDictionary<int, int, int, int, string>();
			dict[1, 1, 1, 1] = "hiep";
			Assert.IsTrue(dict.ContainsKey(1, 1, 1, 1));
			Assert.IsFalse(dict.Remove(1, 2, 1, 1));
			Assert.IsTrue(dict.Remove(1, 1, 1, 1));
			dict.Add(2, 1, 1, 1, "nhu");
			dict.Add(2, 2, 1, 1, "nhu");
		}

		[TestMethod]
		public void MultiKeyDictionary5Test()
		{
			var dict = new MultiKeyDictionary<int, int, int, int, int, string>();
			dict[1, 1, 1, 1, 1] = "hiep";
			dict[2, 2, 1, 1, 2] = "nhu";
			Assert.AreEqual(2, dict.Values.Count());
			Assert.IsTrue(dict.ContainsKey(1, 1, 1, 1, 1));
			Assert.IsFalse(dict.Remove(1, 2, 1, 1, 1));
			Assert.IsTrue(dict.Remove(1, 1, 1, 1, 1));
		}
	}
}
