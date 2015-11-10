using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ToolsPack.Sql
{
	/// <summary>
	/// Helper class for SqlDataReader, which allows for the calling code to retrieve a value in a generic fashion.
	/// http://stackoverflow.com/questions/18550769/sqldatareader-best-way-to-check-for-null-values-sqldatareader-isdbnull-vs-dbnul
	/// 
	///   yourSqlReaderObject.GetValue<int?>("SOME_ID_COLUMN");
	///   yourSqlReaderObject.GetValue<string>("SOME_VALUE_COLUMN");
	/// </summary>
	public static class SqlReaderHelper
	{
		private static bool IsNullableType(Type valueType)
		{
			return (valueType.IsGenericType && valueType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
		}

		/// <summary>
		/// Returns the value, of type T, from the SqlDataReader, accounting for both generic and non-generic types.
		/// </summary>
		/// <typeparam name="T">T, type applied</typeparam>
		/// <param name="reader">The SqlDataReader object that queried the database</param>
		/// <param name="columnName">The column of data to retrieve a value from</param>
		/// <returns>T, type applied; default value of type if database value is null</returns>
		public static T GetValue<T>(this SqlDataReader reader, string columnName)
		{
			// Read the value out of the reader by string (column name); returns object
			object v = reader[columnName];

			// Cast to the generic type applied to this method (i.e. int?)
			Type valueType = typeof(T);

			// Check for null value from the database
			if (DBNull.Value != v)
			{
				// We have a null, do we have a nullable type for T?
				if (!IsNullableType(valueType))
				{
					// No, this is not a nullable type so just change the value's type from object to T
					return (T)Convert.ChangeType(v, valueType);
				}
				else
				{
					// Yes, this is a nullable type so change the value's type from object to the underlying type of T
					NullableConverter nullableConverter = new NullableConverter(valueType);

					return (T)Convert.ChangeType(v, nullableConverter.UnderlyingType);
				}
			}

			// The value was null in the database, so return the default value for T; this will vary based on what T is (i.e. int has a default of 0)
			return default(T);
		}
	}
}
