using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace ToolsPack.FileIo.Csv
{
	/// <summary>
	/// Source: http://knab.ws/blog/index.php?/archives/3-CSV-file-parser-and-writer-in-C-Part-1.html
	/// 
	/// Warning: Not tested yet!
	/// 
	/// Each record is one line (with exceptions)
	/// Fields are separated with commas
	/// Leading and trailing space-characters adjacent to comma field separators are ignored
	/// Fields with embedded commas must be delimited with double-quote characters
	/// Fields that contain double quote characters must be surounded by double-quotes, and the embedded double-quotes must each be represented by a pair of consecutive double quotes.
	/// A field that contains embedded line-breaks must be surounded by double-quotes
	/// Fields with leading or trailing spaces must be delimited with double-quote characters
	/// Fields may always be delimited with double quotes
	/// The first record in a CSV file may be a header record containing column (field) names
	/// 
	/// Example:
	/// 
	/// DataTable dataTable = new DataTable();
	/// //fill the dataTable
	/// //write it to a file
	/// using (StreamWriter writer = new StreamWriter("result.csv"))
	/// {
	///		CsvWriter.WriteToStream(writer, dataTable, true, true);
	///		writer.Close(); ///Save file
	/// }
	/// 
	/// </summary>
	public class CsvWriter
	{
		public static string WriteToString(DataTable table, bool header, bool quoteall)
		{
			StringWriter writer = new StringWriter();
			WriteToStream(writer, table, header, quoteall);
			return writer.ToString();
		}

		public static void WriteToStream(TextWriter stream, DataTable table, bool header, bool quoteall)
		{
			if (header)
			{
				for (int i = 0; i < table.Columns.Count; i++)
				{
					WriteItem(stream, table.Columns[i].Caption, quoteall);
					if (i < table.Columns.Count - 1)
						stream.Write(',');
					else
						stream.Write('\n');
				}
			}
			foreach (DataRow row in table.Rows)
			{
				for (int i = 0; i < table.Columns.Count; i++)
				{
					WriteItem(stream, row[i], quoteall);
					if (i < table.Columns.Count - 1)
						stream.Write(',');
					else
						stream.Write('\n');
				}
			}
		}

		private static void WriteItem(TextWriter stream, object item, bool quoteall)
		{
			if (item == null)
				return;
			string s = item.ToString();
			if (quoteall || s.IndexOfAny("\",\x0A\x0D".ToCharArray()) > -1)
				stream.Write("\"" + s.Replace("\"", "\"\"") + "\"");
			else
				stream.Write(s);
		}
	}
}
