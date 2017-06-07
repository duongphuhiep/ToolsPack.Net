using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolsPack.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ToolsPack.Config.Tests
{
	[TestClass()]
	public class ConfigReaderTests
	{
		[TestMethod()]
		public void ReadConfig_with_default_value()
		{
			//declared in app.config => return the declared value (not the default value)
			Assert.AreEqual("hiep", ConfigReader.Read("name", "moha"));
			Assert.AreEqual(32, ConfigReader.Read("age", 1));

			//not declared in app.config => return the default value
			Assert.AreEqual(1000.0, ConfigReader.Read("salary", 1000.0));
			Assert.AreEqual("none", ConfigReader.Read("address", "none"));
		}

		[TestMethod()]
		public void ReadConfig_without_default_value()
		{
			Assert.IsNull(ConfigReader.Read<string>("films"));
			Assert.AreEqual((decimal)0.0, ConfigReader.Read<decimal>("amount"));
		}

		[TestMethod()]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ReadConfig_with_mismatch_type1()
		{
			ConfigReader.Read<int>("name");
		}

		[TestMethod()]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ReadConfig_with_mismatch_type2()
		{
			ConfigReader.Read("name", 1.0);
		}
	}
}