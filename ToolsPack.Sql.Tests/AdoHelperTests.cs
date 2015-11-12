using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolsPack.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ToolsPack.Log4net;
using System.Data.SqlClient;

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
		public void CreateParameterTest()
		{
			var p = new SqlParameter("@holder", SqlDbType.VarChar, 256);
			Assert.AreEqual("@holder", p.ParameterName);
			Assert.AreEqual(256, p.Size);
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
					"@k4", 1.5,
					"@k5", new DateTime(2015, 5, 21),
                    "@k6", null);

				Assert.AreEqual(7, cmd.Parameters.Count);

				Assert.AreEqual("@k0", cmd.Parameters[0].ParameterName);
				Assert.AreEqual("aaa", cmd.Parameters[0].Value);

				Assert.AreEqual("@k1", cmd.Parameters[1].ParameterName);
				Assert.AreEqual(14, cmd.Parameters[1].Value);

				Assert.AreEqual("@k2", cmd.Parameters[2].ParameterName);
				Assert.AreEqual((decimal)12.5, cmd.Parameters[2].Value);

				Assert.AreEqual("@k3", cmd.Parameters[3].ParameterName);
				Assert.AreEqual("bbb", cmd.Parameters[3].Value);
				Assert.AreEqual(50, cmd.Parameters[3].Size);

				Assert.AreEqual("@k4", cmd.Parameters[4].ParameterName);
				Assert.AreEqual(1.5, cmd.Parameters[4].Value);

				Assert.AreEqual("@k5", cmd.Parameters[5].ParameterName);
				Assert.AreEqual(new DateTime(2015, 5, 21), cmd.Parameters[5].Value);

				Assert.AreEqual("@k6", cmd.Parameters[6].ParameterName);
				Assert.IsNull(cmd.Parameters[6].Value);
			}
		}
	}
}