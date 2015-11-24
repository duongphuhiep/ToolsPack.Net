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
				Assert.AreEqual(DBNull.Value, cmd.Parameters[6].Value);
			}
		}

		[TestMethod()]
		public void CreateCommandTest2()
		{
			var query = "INSERT INTO notification"
						+ " (creation, method, category, user_id, transaction_id, recipient, content_type, body,"
						+ " last_process, status, error, network_status, http_status, response_body) "
					  + " VALUES"
						+ " (getdate(), @method, @category, @user_id, @transaction_id, @recipient, @content_type, @body,"
						+ " getdate(), @status, @error, @network_status, @http_status, @response_body)";
			//db.ExecNonQuery(query,
			//	"@method", notif.Method.ToString(), 6,
			//	"@category", notif.Category,
			//	"@user_id", notif.UserId,
			//	"@transaction_id", notif.TransactionId,
			//	"@recipient", notif.Recipient, 256,
			//	"@content_type", notif.ContentType, 40,
			//	"@body", notif.Body,

			//	"@status", notif.SendStatus == null ? 0 : notif.SendStatus.Status,
			//	"@error", notif.SendStatus == null ? null : notif.SendStatus.Error, 256,
			//	"@network_status", notif.SendStatus == null ? null : notif.SendStatus.NetworkStatus, 10,
			//	"@http_status", notif.SendStatus == null ? 0 : notif.SendStatus.HttpStatus,
			//	"@response_body", notif.SendStatus == null ? null : notif.SendStatus.ResponseBody, 256
			//);

			using (AdoHelper db = new AdoHelper(ConfigReader.Read<string>("ConnectionString", string.Empty)))
			{
				var cmd = db.CreateCommand(query, CommandType.Text,
					"@method", "GET", 6,
					"@category", 1,
					"@user_id", 123,
					"@transaction_id", 456,
					"@recipient", "https://vivaldi.com", 256,
					"@content_type", "application/json", 40,
					"@body", "{wallet:68, msg:disabled}",

					"@status", null,
					"@error", null, 256,
					"@network_status", null, 10,
					"@http_status", 0,
					"@response_body", null, 256);

				Assert.AreEqual(12, cmd.Parameters.Count);

				Assert.AreEqual("@method", cmd.Parameters[0].ParameterName);
				Assert.AreEqual("GET", cmd.Parameters[0].Value);
				Assert.AreEqual(6, cmd.Parameters[0].Size);

				Assert.AreEqual("@category", cmd.Parameters[1].ParameterName);
				Assert.AreEqual(1, cmd.Parameters[1].Value);

				Assert.AreEqual("@user_id", cmd.Parameters[2].ParameterName);
				Assert.AreEqual(123, cmd.Parameters[2].Value);

				Assert.AreEqual("@transaction_id", cmd.Parameters[3].ParameterName);
				Assert.AreEqual(456, cmd.Parameters[3].Value);

				Assert.AreEqual("@recipient", cmd.Parameters[4].ParameterName);
				Assert.AreEqual("https://vivaldi.com", cmd.Parameters[4].Value);
				Assert.AreEqual(256, cmd.Parameters[4].Size);

				Assert.AreEqual("@content_type", cmd.Parameters[5].ParameterName);
				Assert.AreEqual("application/json", cmd.Parameters[5].Value);
				Assert.AreEqual(40, cmd.Parameters[5].Size);

				Assert.AreEqual("@body", cmd.Parameters[6].ParameterName);
				Assert.AreEqual("{wallet:68, msg:disabled}", cmd.Parameters[6].Value);

				Assert.AreEqual("@status", cmd.Parameters[7].ParameterName);
				Assert.AreEqual(DBNull.Value, cmd.Parameters[7].Value);

				Assert.AreEqual("@error", cmd.Parameters[8].ParameterName);
				Assert.AreEqual(DBNull.Value, cmd.Parameters[8].Value);
				Assert.AreEqual(256, cmd.Parameters[8].Size);

				Assert.AreEqual("@network_status", cmd.Parameters[9].ParameterName);
				Assert.AreEqual(DBNull.Value, cmd.Parameters[9].Value);
				Assert.AreEqual(10, cmd.Parameters[9].Size);

				Assert.AreEqual("@http_status", cmd.Parameters[10].ParameterName);
				Assert.AreEqual(0, cmd.Parameters[10].Value);

				Assert.AreEqual("@response_body", cmd.Parameters[11].ParameterName);
				Assert.AreEqual(DBNull.Value, cmd.Parameters[11].Value);
				Assert.AreEqual(256, cmd.Parameters[11].Size);

			}
		}

	}
}