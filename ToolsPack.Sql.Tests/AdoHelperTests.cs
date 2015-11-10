using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolsPack.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ToolsPack.Log4net;

namespace ToolsPack.Sql.Tests
{
	[TestClass()]
	public class AdoHelperTests
	{
		[TestInitialize]
		public void SetUp()
		{
			Log4NetQuickSetup.SetUpConsole();
		}

		[TestMethod()]
		public void CreateCommandTest()
		{
			using (AdoHelper db = new AdoHelper(ConfigReader.Read<string>("ConnectionString", string.Empty)))
			{
				var cmd = db.CreateCommand("MyStoredProc", CommandType.StoredProcedure,
					"@k0", "aaa",
					"@k1", 14,
					"@k2", (decimal)12.5,
					"@k3", "bbb", 50,  //with size
					"@k4", 1.5);

				Assert.AreEqual(5, cmd.Parameters.Count);

				Assert.AreEqual("@k0", cmd.Parameters[0].ParameterName);
				Assert.AreEqual("aaa", cmd.Parameters[0].Value);

				Assert.AreEqual("@k1", cmd.Parameters[1].ParameterName);
				Assert.AreEqual(14, cmd.Parameters[1].Value);

				Assert.AreEqual("@k2", cmd.Parameters[2].ParameterName);
				Assert.AreEqual("@k3", cmd.Parameters[3].ParameterName);
				Assert.AreEqual("@k4", cmd.Parameters[4].ParameterName);
			}
		}
	}
}