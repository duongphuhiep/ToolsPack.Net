using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ToolsPack.Parser.Sqp
{
	/// <summary>
	/// Result of SearchQueryParser, represent list of SearchQueryComponents.
	/// 
	/// SearchQueryComponent = SearchQueryOperator + SearchQueryTerm + [ SearchQueryOperator + SearchQueryTerm... ]
	/// SearchQueryOperator = And | Or | AndNot | OrNot
	/// SearchQueryTerm = SearchQueryTermColumn | SearchQueryTermSimple
	/// 
	/// Usage example:
	/// 
	/// string userInputQuery = "sell || expiry>=11/5/2000 && -UnderlyingCode:\"Newyork CITI\"";
	/// SearchQuery searchQuery = SearchQueryParser.Parse(searchQueryString);
	/// Console.WriteLine(searchQuery.ToString()); //print a human-readable SQL-like query
	/// 
	/// </summary>
	public class SearchQuery
	{
		public List<SearchQueryComponent> SearchQueryComponents = new List<SearchQueryComponent>();

		public static string ConvertToSqlString(SearchQueryOperator op)
		{
			switch (op)
			{
				case SearchQueryOperator.And: return "AND";
				case SearchQueryOperator.Or: return "OR";
				case SearchQueryOperator.AndNot: return "AND NOT";
				default: return "OR NOT";
			}
		}
		public static string ConvertToSqlString(TermColumnOperator op)
		{
			switch (op)
			{
				case TermColumnOperator.Equal: return "=";
				case TermColumnOperator.Higer: return ">";
				case TermColumnOperator.Lower: return "<";
				case TermColumnOperator.HigerOrEqual: return ">=";
				case TermColumnOperator.LowerOrEqual: return "<=";
				default: return "LIKE";
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach (SearchQueryComponent component in this.SearchQueryComponents)
			{
				if (sb.Length > 0)
				{
					sb.Append(" " + SearchQuery.ConvertToSqlString(component.Operator) + " ");
				}
				else 
				{
					if (component.Operator == SearchQueryOperator.AndNot || component.Operator == SearchQueryOperator.OrNot)
					{
						sb.Append("NOT ");
					}
				}

				if (component.Term is SearchQueryTermSimple)
				{
					SearchQueryTermSimple term = (SearchQueryTermSimple)component.Term;
					sb.Append("(OneOfColumns CONTAINS '" + term.Term + "')");
				}
				else
				{
					SearchQueryTermColumn term = (SearchQueryTermColumn)component.Term;
					string columnOperator;
					if (term.Operator == TermColumnOperator.Like)
					{
						columnOperator = "CONTAINS";
					}
					else
					{
						columnOperator = ConvertToSqlString(term.Operator);
					}

					sb.Append("('" + term.Column + "' " + columnOperator + " '" + term.Value + "')");
				}
				
				sb.Append(Environment.NewLine); //for a nice display
			}

			return sb.ToString();
		}
	}
}
