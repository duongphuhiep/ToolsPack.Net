using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolsPack.Displayer
{
	/// <summary>
	/// convert Array/List to String (used in log message)
	///This class must not depend on anything else, 
	///and can be move to anywhere (CommonServices for example)
	/// 
	/// IEnumerable arr;
	/// arr.Display().SeparatedBy("; ").MaxItems()
	/// </summary>
	public static class ArrayDisplayer
	{
		public class ArrayFormat
		{
			private readonly IEnumerable _arr;
			private int _maxItems = int.MaxValue;
			private int _typicalLength = int.MaxValue;
			private string _separator = ", ";
			private Func<string, int, string, string> _ellipsis = null;
			private int _maxItemLength = int.MaxValue;

			public ArrayFormat(IEnumerable arr)
			{
				_arr = arr;
			}

			public ArrayFormat SeparatedBy(string separator)
			{
				_separator = separator;
				return this;
			}

			public ArrayFormat SeparatedByNewLine()
			{
				_separator = Environment.NewLine;
				return this;
			}

			public ArrayFormat SeparatedByComma()
			{
				_separator = ", ";
				return this;
			}

			public ArrayFormat MaxItems(int maxItems)
			{
				_maxItems = maxItems;
				return this;
			}

			public ArrayFormat TypicalLength(int typicalLength)
			{
				_typicalLength = typicalLength;
				return this;
			}

			public ArrayFormat MaxItemLength(int maxItemLength)
			{
				_maxItemLength = maxItemLength;
				_ellipsis = ArrayDisplayer.DefaultEllipsis;
				return this;
			}

			/// <summary>
			/// We have 3 pre-defined ellipsis functions: 
			/// - DefaultEllipsis
			/// - WordEllipsis
			/// - WordEllipsisStrictLength
			/// </summary>
			public ArrayFormat MaxItemLength(int maxItemLength, Func<string, int, string, string> ellipsis)
			{
				_maxItemLength = maxItemLength;
				_ellipsis = ellipsis;
				return this;
			}

			public override string ToString()
			{
				if (_typicalLength < int.MaxValue)
				{
					return DisplayMaxLength(_arr, _typicalLength, _separator, _ellipsis, _maxItemLength);
				}
				return DisplayMaxItem(_arr, _maxItems, _separator, _ellipsis, _maxItemLength);
			}
		}

		public static ArrayFormat Display(this IEnumerable arr)
		{
			return new ArrayFormat(arr);
		}

		/// <summary>
		/// Convert array to string to display in log message:
		/// if the given array has only 1 element:
		/// - display the only element.
		/// * if the given array has more than maxItemsToDisplay elements:
		/// -   display "{ item1, item2, item3, item4, ..and 245 (of 1000) more }"
		/// if maxItemsToDisplay = 0 or negative:
		/// - display all items. Example "{ item1, item2, item3, item4 }"
		/// 
		/// The maxItemLength is used to limit item length. 
		/// if a item is too long, it will be cut and marked ellipsisFunc. Example:
		/// With maxItemLength = 10: the item "Lorem ipsum dolor sit amet" is displayed as "[[Lorem ipsu...]]".
		/// </summary>
		public static string DisplayMaxItem(IEnumerable arr, int maxItemsToDisplay = int.MaxValue, string separator = ", ",
			Func<string, int, string, string> ellipsisFunc = null,
			int maxItemLength = 255)
		{
			if (arr == null)
			{
				return "NULL";
			}

			var e = arr.GetEnumerator();

			object item1 = null;
			bool hasItem1 = e.MoveNext();
			if (hasItem1)
			{
				item1 = e.Current;
			}
			else
			{
				//arr is empty
				return "{}";
			}
			string item1ToString = Display(item1, maxItemLength, ellipsisFunc);

			object item2 = null;
			bool hasItem2 = e.MoveNext();
			if (hasItem2)
			{
				item2 = e.Current;
			}
			else
			{
				//arr has only 1 item (which can be a null item)
				return item1ToString;
			}
			string item2ToString = Display(item2, maxItemLength, ellipsisFunc);


			var resu = new StringBuilder("{ ");

			if (maxItemsToDisplay <= 0)
			{
				//print all items

				resu.Append(item1ToString + separator + item2ToString);
				while (e.MoveNext())
				{
					resu.Append(separator + Display(e.Current, maxItemLength, ellipsisFunc));
				}
			}
			else
			{
				//print { 1, 2, 3, ..and 1001 more }

				int pos = 0;
				switch (maxItemsToDisplay)
				{
					case 1:
						resu.Append(item1ToString);
						pos = 2;
						break;
					case 2:
						resu.Append(item1ToString + separator + item2ToString);
						pos = 2;
						break;
					default: //maxItemsToDisplay > 2
						resu.Append(item1ToString + separator + item2ToString);
						pos = 2;
						while (pos < maxItemsToDisplay && e.MoveNext())
						{
							resu.Append(separator + Display(e.Current, maxItemLength, ellipsisFunc));
							pos++;
						}
						break;
				}

				//print ", ..and 1001 more"

				//count numbers of all items
				while (e.MoveNext())
				{
					pos++;
				}

				if (pos > maxItemsToDisplay)
				{
					resu.Append(separator + "..and " + (pos - maxItemsToDisplay) + " (of " + pos + ")" + " more");
				}
			}

			resu.Append(" }");

			return resu.ToString();
		}

		/// <summary>
		/// Convert array to string to display in log message:
		/// - The first item will always be displayed
		/// - All items will be displayed if the output string is not too long 
		/// otherwise the output string will be trim as following:
		/// 		 { item1, item2, item3, item4, ..and 245 (of 10000) more }
		/// 
		/// TypicalLength parameter is used to limit the result length. 
		/// The greater TypicalLength is, the more item will be display 
		/// - TypicalLength is not the max length allowed of the output string
		/// - The output string length is about TypicalyLength + length of the last displayed item + length of the trailing text "..and 245 more"
		/// 
		/// The maxItemLength is used to limit item length. 
		/// if a item is too long, it will be cut in ellipses format. Example:
		/// With maxItemLength = 10: the item "Lorem ipsum dolor sit amet" is displayed as "[[Lorem ipsu...]]".
		/// </summary>
		public static string DisplayMaxLength(IEnumerable arr, int typicalLength = int.MaxValue, string separator = ", ",
			Func<string, int, string, string> ellipsisFunc = null, int maxItemLength = 255)
		{
			if (arr == null)
			{
				return "NULL";
			}

			var e = arr.GetEnumerator();

			object item1 = null;
			bool hasItem1 = e.MoveNext();
			if (hasItem1)
			{
				item1 = e.Current;
			}
			else
			{
				//arr is empty
				return "{}";
			}
			string item1ToString = Display(item1, maxItemLength, ellipsisFunc);

			object item2 = null;
			bool hasItem2 = e.MoveNext();
			if (hasItem2)
			{
				item2 = e.Current;
			}
			else
			{
				//arr has only 1 item (which can be a null item)
				return item1ToString;
			}
			string item2ToString = Display(item2, maxItemLength, ellipsisFunc);


			var resu = new StringBuilder("{ ");

			if (typicalLength <= 0)
			{
				//print all items

				resu.Append(item1ToString + separator + item2ToString);
				while (e.MoveNext())
				{
					resu.Append(separator + Display(e.Current, maxItemLength, ellipsisFunc));
				}
			}
			else
			{
				//print { 1, 2, 3, ..and 1001 more }

				int pos = 2;
				var displayed = 1;
				resu.Append(item1ToString);
				if (resu.Length < typicalLength)
				{
					resu.Append(separator + item2ToString);
					displayed = 2;
				}

				while (resu.Length < typicalLength && e.MoveNext())
				{
					resu.Append(separator + Display(e.Current, maxItemLength, ellipsisFunc));
					displayed++;
					pos++;
				}

				//print ", ..and 1001 more"

				//count numbers of all items
				while (e.MoveNext())
				{
					pos++;
				}

				if (pos > displayed)
				{
					resu.Append(separator + "..and " + (pos - displayed) + " (of " + pos + ")" + " more");
				}
			}

			resu.Append(" }");

			return resu.ToString();
		}

		#region Convenient signatures

		public static string DisplayMaxItem(IEnumerable arr, string separator = ", ", int maxItemsToDisplay = int.MaxValue,
			Func<string, int, string, string> ellipsisFunc = null,
			int maxItemLength = 255)
		{
			return DisplayMaxItem(arr, maxItemsToDisplay, separator, ellipsisFunc, maxItemLength);
		}

		public static string DisplayMaxLength(IEnumerable arr, string separator = ", ", int typicalLength = int.MaxValue,
			Func<string, int, string, string> ellipsisFunc = null, int maxItemLength = 255)
		{
			return DisplayMaxLength(arr, typicalLength, separator, ellipsisFunc, maxItemLength);
		}

		#endregion

		/// <summary>
		/// Display a dictionary, it is not recommended to use on big dictionary
		/// </summary>
		public static string Display<TK, TV>(this Dictionary<TK, TV> dict, string separator = ", ")
		{
			return "{ " + string.Join(separator, dict.Select(x => x.Key + " -> " + x.Value).ToArray()) + " }";
		}

		private static string Display(object o, int maxLength, Func<string, int, string, string> ellipsisFunc)
		{
			if (o == null)
			{
				return "NULL";
			}
			if (ellipsisFunc == null)
			{
				return o.ToString();
			}

			string s = o.ToString();
			string e = ellipsisFunc(s, maxLength, "...");
			if (e.Length != s.Length)
			{
				return "[[" + e + "]]";
			}

			return s;
		}

		public static string DefaultEllipsis(string text, int length, string ellipsis)
		{
			if (length <= 0)
			{
				return text;
			}
			if (ellipsis.Length <= text.Length - length)
			{
				return text.Substring(0, length) + ellipsis;
			}
			return text;
		}

		/// <summary>
		/// Always display the first words.
		/// 
		/// 0 : Lorem...
		/// 1 : Lorem...
		/// 2 : Lorem...
		/// 3 : Lorem...
		/// 4 : Lorem...
		/// 5 : Lorem...
		/// 6 : Lorem ipsum...
		/// 7 : Lorem ipsum...
		/// 8 : Lorem ipsum...
		/// 9 : Lorem ipsum...
		/// 10 : Lorem ipsum...
		/// 11 : Lorem ipsum...
		/// 12 : Lorem ipsum dolor...
		/// </summary>
		public static string WordEllipsis(string text, int length, string ellipsis)
		{
			if (text.Length <= length) return text;
			int pos = text.IndexOf(" ", length, StringComparison.Ordinal);
			if (pos >= 0)
			{
				return text.Substring(0, pos) + ellipsis;
			}
			return text;
		}

		/// <summary>
		/// Strict Max Length
		/// 
		/// 0 : 
		/// 1 : .
		/// 2 : ..
		/// 3 : ...
		/// 4 : ...
		/// 5 : ...
		/// 6 : ...
		/// 7 : ...
		/// 8 : ...
		/// 9 : Lorem...
		/// 10 : Lorem...
		/// 11 : Lorem...
		/// 12 : Lorem...
		/// 13 : Lorem...
		/// 14 : Lorem...
		/// 15 : Lorem ipsum...
		/// 16 : Lorem ipsum...
		/// 17 : Lorem ipsum...
		/// 18 : Lorem ipsum...
		/// 19 : Lorem ipsum..
		/// </summary>
		public static string WordEllipsisStrictLength(string text, int maxLength, string ellipsis)
		{
			string result;

			if (text.Length <= maxLength)
			{
				result = text;
			}
			else if (maxLength <= ellipsis.Length)
			{
				result = ellipsis.Substring(0, maxLength);
			}
			else
			{
				result = text.Substring(0, maxLength - ellipsis.Length);
				var lastWordPosition = result.LastIndexOf(' ');

				if (lastWordPosition < 0)
				{
					lastWordPosition = 0;
				}
				result = result.Substring(0, lastWordPosition).Trim(new[] {'.', ',', '!', '?'}) + ellipsis;
			}

			return result;
		}
	}
}
