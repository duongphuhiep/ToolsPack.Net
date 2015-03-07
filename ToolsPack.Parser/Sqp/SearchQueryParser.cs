using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ToolsPack.Parser.Sqp
{
	public enum SearchQueryOperator { And, Or, AndNot, OrNot };
	public enum TermColumnOperator { Equal, Like, Higer, Lower, HigerOrEqual, LowerOrEqual }

	/// <summary>
	/// Parse a search query string. Eg: [ CAC -Swap expiry=jan10 "Put over" ]
	/// 
	/// Usage example:
	/// 
	/// string userInputQuery = "sell || expiry>=11/5/2000 && -UnderlyingCode:\"Newyork CITI\"";
	/// SearchQuery searchQuery = SearchQueryParser.Parse(searchQueryString);
	/// Console.WriteLine(searchQuery.ToString()); //print a human-readable SQL-like query
	/// 
	/// The parser help to recognize:
	/// *) Rule A: Term:
	///		*) word
	///		*) phrase between double quote (")
	///		*) Column[OPR]Value ( OPR can be ':', '=', '>', '<', '>=', '<=')
	/// *) Rule B: OR condition
	///		*) +Term           : Match/Contains Term
	///		*) |Term or ||Term : Match/Contains Term
	/// *) Rule C: AND condition
	///		*) -Term           : Do not match/contain Term (Exclusive Term)
	///		*) [Blank] Term    : Must to match/contain Term (Inclusive Term)
	///		*) &Term or &&Term : Must to match/contain Term (Inclusive Term)
	///		*) Column:Value    : Column must to contain Value (Column and Value are Terms)
	///		*) Column=Value    : Column must to be Value
	///		*) Column<Value    : 
	///		*) Column>Value    : 
	///		*) Column<=Value   : 
	///		*) Column>=Value   : 
	/// 
	/// 
	/// How to use?
	/// 
	/// string searchQueryString = cac +toto -titi -columA<11 +"expiry date">="1/1/2000" isin:"XX000XXX" -columnB:"tu tu"
	/// SearchQuery = SearchQueryParser.Parse(searchQueryString);
	/// Console.WriteLine(SearchQuery.ToString()); //print a human-readable query
	/// 
	/// Query examples:
	/// 
	///  toto tata
	///  toto -tata
	///  toto +tata
	///  toto || -titi
	///  sell || expiry>=11/5/2000 && -UnderlyingCode:"Newyork CITI"
	///  BuyCode="XYZD-03839-1234" ShowUp
	/// 
	/// Build your SQL query with SearchQuery object:
	/// 
	/// SearchQuery = List of SearchQueryComponent
	/// SearchQueryComponent = SearchQueryOperator (And/Or/AndNot/OrNot) + SearchQueryTerm
	/// SearchQueryTerm = SearchQueryTermSimple ("toto") or SearchQueryTermColumn ("column:value")
	/// SearchQueryTermSimple = a string
	/// SearchQueryTermColumn represent something like "column>=value" so a Column, Value, and an Operator (':', '=', '>', '<', '>=', '<=')
	/// </summary>
	public class SearchQueryParser
	{
		private static readonly string[] listOperators = new string[] { "+", "-", "|", "||", ":", ">", "<", "=", "<=", ">=" };
		private readonly string query;
		private readonly int queryLength;
		private readonly List<string> items = new List<string>();
		private readonly SearchQuery parsedResult = new SearchQuery();

		private SearchQueryParser(string searchQueryString)
		{
			this.query = searchQueryString;
			this.queryLength = this.query.Length;
		}

		private void itemize()
		{
			int currentPosition = 0;
			while (currentPosition < queryLength)
			{
				string holdingPart;
				char metChar;
				int metAt;
				if (this.grabUntilMeet(new char[] { '"', '+', '-', '<', '>', ':', '=', ' ','&','|' }, currentPosition, out holdingPart, out metChar, out metAt))
				{
					if (!string.IsNullOrEmpty(holdingPart))
					{
						this.items.Add(holdingPart);
					}
					if (metChar == '"')
					{
						currentPosition = metAt;
						if (this.grabUntilMeet('"', currentPosition + 1, out holdingPart, out metAt))
						{
							this.items.Add(holdingPart);
							currentPosition = metAt + 1;
						}
						else
						{
							throw new BadSearchQueryException("An opening double-quote is not closed");
						}
					}
					else
					{
						if (metAt == this.queryLength - 1)
						{
							return;
						}

						//there is still something left after metAt
						char nextChar = this.query[metAt + 1];

						if ((metChar == '<' || metChar == '>') && nextChar == '=') ///met '>=', '<='
						{
							this.items.Add(new string(new char[] { metChar, nextChar })); ///add '<=', '>=' to the items list
							currentPosition = metAt + 2;
						}
						else
						{
							///add separator to the items list and
							///1) ignore 'blank', '&' and '&&' 
							///2) '||' is saved as '|' into the list

							if (metChar != ' ' && metChar != '&')
							{
								this.items.Add(metChar.ToString());
							}
							
							///move forward the current position

							if ((metChar == '&' && nextChar == '&') || (metChar == '|' && nextChar == '|')) ///&, &&, |, ||
							{
								currentPosition = metAt + 2;
							}
							else
							{
								currentPosition = metAt + 1;
							}
						}
					}
				}
				else
				{
					string leftStuff = query.Substring(currentPosition);
					if (!string.IsNullOrEmpty(leftStuff))
					{
						this.items.Add(query.Substring(currentPosition));
					}
					break;
				}
			}
		}
		private bool grabUntilMeet(char[] separators, int fromIndex, out string holdingPart, out char metChar, out int metAt)
		{
			metAt = query.IndexOfAny(separators, fromIndex, this.queryLength - fromIndex);
			if (metAt < 0)
			{
				holdingPart = null;
				metChar = '#';
				metAt = -1;
				return false;
			}
			metChar = this.query[metAt];
			char preMetChar = ' ';
			if (metAt >= 1)
			{
				preMetChar = this.query[metAt - 1];
			}
			/// To distinguish the two cases by the preMetChar:
			/// With Empty space before: titi -toto (- means the operator substract) 
			/// Without Empty space before: titi-toto( - means the charactor '-'), the metAt should continue to find the next operator
			while (metChar == '-' && preMetChar != ' ')
			{					
				int currentMetAt = metAt;
				metAt = query.IndexOfAny(separators, currentMetAt+1, this.queryLength - currentMetAt-1);
				if (metAt < 0)
				{
					holdingPart = query.Substring(fromIndex, this.queryLength - fromIndex);
					metAt = this.queryLength-1;
					metChar = this.query[metAt];
					return true;
				}
				else
				{
					metChar = this.query[metAt];
					preMetChar = this.query[metAt - 1];
				}
			}				
			holdingPart = query.Substring(fromIndex, metAt - fromIndex);
			return true;
		}
		private bool grabUntilMeet(char separator, int fromIndex, out string holdingPart, out int metAt)
		{
			char metChar;
			return this.grabUntilMeet(new char[] { separator }, fromIndex, out holdingPart, out metChar, out metAt);
		}

		private void publishParseResult()
		{
			this.itemize(); //builds items list

			/////print items for debug 

			//string msg = "";
			//foreach (string i in this.items)
			//{
			//    msg += i + Environment.NewLine;
			//}
			//Log.AdminInfo("[Itemize] " + query, msg);

			int i = 0;
			while (i < this.items.Count)
			{
				string preCi0 = null;
				if (i > 1)
				{
					preCi0 = this.items[i - 1];					
				}				
				string ci0 = this.items[i];

				string ci1 = null;
				if (i + 1 < this.items.Count)
				{
					ci1 = this.items[i + 1];
				}

				string ci2 = null;
				if (i + 2 < this.items.Count)
				{
					ci2 = this.items[i + 2];
				}

				string ci3 = null;
				if (i + 3 < this.items.Count)
				{
					ci3 = this.items[i + 3];
				}

				string ci4 = null;
				if (i + 4 < this.items.Count)
				{
					ci4 = this.items[i + 4];
				}


				if (ci0 == "|" && ci1 == "-")
				{
					if (ci3 == ":" || ci3 == "=" || ci3 == "<" || ci3 == ">" || ci3 == "<=" || ci3 == ">=")
					{
						//case: || -Column:[Value]
						if (ci4 != null)
						{
							//case: || -Column:Value

							this.parsedResult.SearchQueryComponents.Add(new SearchQueryComponent(SearchQueryOperator.OrNot, new SearchQueryTermColumn(ci2, ci4, parseTermColumnOperator(ci3))));
							i += 5;
						}
						else
						{
							//case: || -Column:

							this.parsedResult.SearchQueryComponents.Add(new SearchQueryComponent(SearchQueryOperator.OrNot, new SearchQueryTermSimple(ci2)));
							i += 4;
						}
					}
					else
					{
						//case: || -Term
						this.parsedResult.SearchQueryComponents.Add(new SearchQueryComponent(SearchQueryOperator.OrNot, new SearchQueryTermSimple(ci2)));
						i += 3;
					}
				}
				else if (Array.IndexOf(SearchQueryParser.listOperators, ci0) >= 0 && Array.IndexOf(SearchQueryParser.listOperators, ci1) >= 0)
				{
					//case: || +toto
					throw new BadSearchQueryException("Do not understand '"+ci0+ci1+"'. Try to use double-quote(\") for term which contains special character.");
				}
				else if (ci0 == "+" || ci0 == "|" || ci0 == "-")
				{
					SearchQueryOperator oper;
					if (ci0 == "-")
					{
						oper = SearchQueryOperator.AndNot;				
					}
					else
					{
						oper = SearchQueryOperator.Or;
					}

					if (ci2 == ":" || ci2 == "=" || ci2 == "<" || ci2 == ">" || ci2 == "<=" || ci2 == ">=")
					{
						//case: +Column:[Value]

						if (ci3 != null)
						{
							//case: +Column:Value
							this.parsedResult.SearchQueryComponents.Add(new SearchQueryComponent(oper, new SearchQueryTermColumn(ci1, ci3, parseTermColumnOperator(ci2))));
							i += 4;
						}
						else
						{
							//case: +Column:
							this.parsedResult.SearchQueryComponents.Add(new SearchQueryComponent(oper, new SearchQueryTermSimple(ci1)));
							i += 3;
						}
					}
					else
					{
						//case: +Term
						this.parsedResult.SearchQueryComponents.Add(new SearchQueryComponent(oper, new SearchQueryTermSimple(ci1)));
						i += 2;
					}
				}
				else
				{
					if (ci1 == ":" || ci1 == "=" || ci1 == "<" || ci1 == ">" || ci1 == "<=" || ci1 == ">=")
					{
						//case: Column:[Value]

						if (ci2 != null)
						{
							//case: Column:Value
							this.parsedResult.SearchQueryComponents.Add(new SearchQueryComponent(SearchQueryOperator.And, new SearchQueryTermColumn(ci0, ci2, parseTermColumnOperator(ci1))));
							i += 3;	
						}
						else
						{
							//case: Column:
							this.parsedResult.SearchQueryComponents.Add(new SearchQueryComponent(SearchQueryOperator.And, new SearchQueryTermSimple(ci0)));
							i += 2;
						}
					}
					else
					{
						//case: Term
						this.parsedResult.SearchQueryComponents.Add(new SearchQueryComponent(SearchQueryOperator.And, new SearchQueryTermSimple(ci0)));
						i += 1;
					}
				}
			}
		}

		/// <summary>
		/// Main entry
		/// </summary>
		public static SearchQuery Parse(string query)
		{
			SearchQueryParser parser = new SearchQueryParser(query);
			parser.publishParseResult();
			return parser.parsedResult;
		}

		private static TermColumnOperator parseTermColumnOperator(string op)
		{ 
			switch (op)
			{
				case "=": return TermColumnOperator.Equal;
				case ">": return TermColumnOperator.Higer;
				case "<": return TermColumnOperator.Lower;
				case ">=": return TermColumnOperator.HigerOrEqual;
				case "<=": return TermColumnOperator.LowerOrEqual;
				default: return TermColumnOperator.Like;
			}
		}
	}
}
