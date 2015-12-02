using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolsPack.Parser
{
	public class StringTools
	{
		/// <summary>
		///	Split the row into cells using the given columnsBreak
		/// </summary>
		public static string[] Split(string row, SortedSet<int> columnsBreak, bool trimCells = false)
		{
			var cells = new string[columnsBreak.Count + 1];

			var prePos = 0;
			int i = 0;
			foreach (int pos in columnsBreak)
			{
				cells[i] = GetSubstring(row, prePos, pos, trimCells);
				prePos = pos;
				i++;
			}

			if (i == 0) //no columnsBreak (the list is empty)
			{
				cells[0] = row;
			}
			else //fill the last cells
			{
				cells[i] = GetSubstring(row, prePos, row.Length, trimCells);
			}

			return cells;
		}

		/// <summary>
		/// Get the substring from postion p1 to position p2
		/// </summary>
		public static string GetSubstring(string str, int p1, int p2, bool trimCells = false)
		{
			if (p1 > str.Length)
			{
				return string.Empty;
			}
			if (p2 > str.Length)
			{
				p2 = str.Length;
			}
			if (p1 >= p2)
			{
				throw new InvalidOperationException();
			}
			return trimCells ? str.Substring(p1, p2 - p1).Trim() : str.Substring(p1, p2 - p1);
		}

		/// <summary>
		/// Remove consecutive multiple WhiteSpace
		/// </summary>
		public static string NormalizeWhiteSpace(string input, char normalizeTo = ' ')
		{
			if (string.IsNullOrEmpty(input))
				return string.Empty;

			int current = 0;
			char[] output = new char[input.Length];
			bool skipped = false;

			foreach (char c in input.ToCharArray())
			{
				if (char.IsWhiteSpace(c))
				{
					if (!skipped)
					{
						if (current > 0)
							output[current++] = normalizeTo;

						skipped = true;
					}
				}
				else
				{
					skipped = false;
					output[current++] = c;
				}
			}

			return new string(output, 0, skipped ? current - 1 : current);
		}
	}
}
}
