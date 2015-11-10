using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ToolsPack.Sql
{
	/// <summary>
	/// An ADO.NET helper class
	/// http://www.blackbeltcoder.com/Articles/ado/an-ado-net-sql-helper-class
	/// 
	///    string qry = "SELECT ArtID, ArtTitle, ArtSlug" +
	///        " FROM Article WHERE ArtApproved = @Approved AND ArtUpdated > @Updated";
	///
	///    using (AdoHelper db = new AdoHelper())
	///    using (SqlDataReader rdr = db.ExecDataReader(qry, 
	///        "@Approved", true,
	///        "@Fuu", "Beuh",
	///        "@Foo", "Bazz", 50 //50 is the parameter size to optimize query cache in some case
	///        "@Updated", new DateTime(2011, 3, 1)))
	///    {
	///        while (rdr.Read())
	///        {
	///            // Get row of data from rdr
	///        }
	///    }
	/// 
	/// Note: The optional parameter size exist only for string parameters
	/// </summary>
	public class AdoHelper : IDisposable
	{
		// Internal members
		protected string _connString = null;
		protected SqlConnection _conn = null;
		protected SqlTransaction _trans = null;
		protected bool _disposed = false;

		/// <summary>
		/// Sets or returns the connection string use by all instances of this class.
		/// </summary>
		public static string ConnectionString { get; set; }

		/// <summary>
		/// Returns the current SqlTransaction object or null if no transaction
		/// is in effect.
		/// </summary>
		public SqlTransaction Transaction { get { return _trans; } }

		/// <summary>
		/// Constructor using global connection string.
		/// </summary>
		public AdoHelper()
		{
			_connString = ConnectionString;
			Connect();
		}

		/// <summary>
		/// Constructure using connection string override
		/// </summary>
		/// <param name="connString">Connection string for this instance</param>
		public AdoHelper(string connString)
		{
			_connString = connString;
			Connect();
		}

		// Creates a SqlConnection using the current connection string
		protected void Connect()
		{
			_conn = new SqlConnection(_connString);
			_conn.Open();
		}

		/// <summary>
		/// Constructs a SqlCommand with the given parameters. This method is normally called
		/// from the other methods and not called directly. But here it is if you need access
		/// to it.
		/// </summary>
		/// <param name="qry">SQL query or stored procedure name</param>
		/// <param name="type">Type of SQL command</param>
		/// <param name="args">Query arguments. Arguments should be in pairs where one is the
		/// name of the parameter and the second is the value. The very last argument can
		/// optionally be a SqlParameter object for specifying a custom argument type</param>
		/// <returns></returns>
		public SqlCommand CreateCommand(string qry, CommandType type, params object[] args)
		{
			SqlCommand cmd = new SqlCommand(qry, _conn);

			// Associate with current transaction, if any
			if (_trans != null)
				cmd.Transaction = _trans;

			// Set command type
			cmd.CommandType = type;

			int L = args.Length;

			// Construct SQL parameters
			for (int i = 0; i < L; i++)
			{
				if (args[i] is string && i+1 < L)
				{
					SqlParameter param = new SqlParameter();
					param.ParameterName = (string)args[i];
					param.Value = args[++i]; 

					//if value is a string, so the next args might be the size
					if (param.Value is string && i+1 < L && args[i+1] is int)
					{
						param.Size = (int)args[++i]; //the next args is really the size
					}

					cmd.Parameters.Add(param);
				}
				else if (args[i] is SqlParameter)
				{
					cmd.Parameters.Add((SqlParameter)args[i]);
				}
				else throw new ArgumentException("Invalid number or type of arguments supplied");
			}
			return cmd;
		}

		#region Exec Members

		/// <summary>
		/// Executes a query that returns no results
		/// </summary>
		/// <param name="qry">Query text</param>
		/// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
		/// <returns>The number of rows affected</returns>
		public int ExecNonQuery(string qry, params object[] args)
		{
			using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
			{
				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Executes a stored procedure that returns no results
		/// </summary>
		/// <param name="proc">Name of stored proceduret</param>
		/// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
		/// <returns>The number of rows affected</returns>
		public int ExecNonQueryProc(string proc, params object[] args)
		{
			using (SqlCommand cmd = CreateCommand(proc, CommandType.StoredProcedure, args))
			{
				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Executes a query that returns a single value
		/// </summary>
		/// <param name="qry">Query text</param>
		/// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
		/// <returns>Value of first column and first row of the results</returns>
		public object ExecScalar(string qry, params object[] args)
		{
			using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
			{
				return cmd.ExecuteScalar();
			}
		}

		/// <summary>
		/// Executes a query that returns a single value
		/// </summary>
		/// <param name="proc">Name of stored proceduret</param>
		/// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
		/// <returns>Value of first column and first row of the results</returns>
		public object ExecScalarProc(string qry, params object[] args)
		{
			using (SqlCommand cmd = CreateCommand(qry, CommandType.StoredProcedure, args))
			{
				return cmd.ExecuteScalar();
			}
		}

		/// <summary>
		/// Executes a query and returns the results as a SqlDataReader
		/// </summary>
		/// <param name="qry">Query text</param>
		/// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
		/// <returns>Results as a SqlDataReader</returns>
		public SqlDataReader ExecDataReader(string qry, params object[] args)
		{
			using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
			{
				return cmd.ExecuteReader();
			}
		}

		/// <summary>
		/// Executes a stored procedure and returns the results as a SqlDataReader
		/// </summary>
		/// <param name="proc">Name of stored proceduret</param>
		/// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
		/// <returns>Results as a SqlDataReader</returns>
		public SqlDataReader ExecDataReaderProc(string qry, params object[] args)
		{
			using (SqlCommand cmd = CreateCommand(qry, CommandType.StoredProcedure, args))
			{
				return cmd.ExecuteReader();
			}
		}

		/// <summary>
		/// Executes a query and returns the results as a DataSet
		/// </summary>
		/// <param name="qry">Query text</param>
		/// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
		/// <returns>Results as a DataSet</returns>
		public DataSet ExecDataSet(string qry, params object[] args)
		{
			using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
			{
				SqlDataAdapter adapt = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				adapt.Fill(ds);
				return ds;
			}
		}

		/// <summary>
		/// Executes a stored procedure and returns the results as a Data Set
		/// </summary>
		/// <param name="proc">Name of stored proceduret</param>
		/// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
		/// <returns>Results as a DataSet</returns>
		public DataSet ExecDataSetProc(string qry, params object[] args)
		{
			using (SqlCommand cmd = CreateCommand(qry, CommandType.StoredProcedure, args))
			{
				SqlDataAdapter adapt = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				adapt.Fill(ds);
				return ds;
			}
		}

		#endregion

		#region Transaction Members

		/// <summary>
		/// Begins a transaction
		/// </summary>
		/// <returns>The new SqlTransaction object</returns>
		public SqlTransaction BeginTransaction()
		{
			Rollback();
			_trans = _conn.BeginTransaction();
			return Transaction;
		}

		/// <summary>
		/// Commits any transaction in effect.
		/// </summary>
		public void Commit()
		{
			if (_trans != null)
			{
				_trans.Commit();
				_trans = null;
			}
		}

		/// <summary>
		/// Rolls back any transaction in effect.
		/// </summary>
		public void Rollback()
		{
			if (_trans != null)
			{
				_trans.Rollback();
				_trans = null;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				// Need to dispose managed resources if being called manually
				if (disposing)
				{
					if (_conn != null)
					{
						Rollback();
						_conn.Dispose();
						_conn = null;
					}
				}
				_disposed = true;
			}
		}

		#endregion
	}
}
