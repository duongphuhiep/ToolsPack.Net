using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ToolsPack.Parser.Sqp
{
	/// <summary>
	/// A part of SearchQuery in form: "Opertor Term". Example: "AND Strike > 20" or "CONTAINS 'cac40'"
	/// </summary>
	public class SearchQueryComponent
	{
		public readonly SearchQueryOperator Operator;
		public readonly SearchQueryTerm Term;

		internal SearchQueryComponent(SearchQueryOperator oper, SearchQueryTerm term)
		{
			this.Operator = oper;
			this.Term = term;
		}

		public override string ToString()
		{
			return this.Operator + " " + this.Term.ToString();
		}
	}
}
