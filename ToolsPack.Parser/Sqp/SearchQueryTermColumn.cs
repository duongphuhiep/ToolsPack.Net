using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ToolsPack.Parser.Sqp
{
	/// <summary>
	/// A SearchQueryTerm represents query "Column Operator Value" example: "Strike > 20"
	/// It's a part of the search query which means we are looking for object which has "Strike > 20"
	/// </summary>
	public class SearchQueryTermColumn : SearchQueryTerm
	{
		public readonly string Column;
		public readonly string Value;
		/// <summary>
		/// ':', '=', '>', '<', '>=', '<='
		/// </summary>
		public readonly TermColumnOperator Operator;

		internal SearchQueryTermColumn(string column, string value, TermColumnOperator operateur)
		{
			this.Column = column;
			this.Value = value;
			this.Operator = operateur;
		}

		public override string ToString()
		{
			return this.Column + " " + termColumnOperatorToString(this.Operator) + " " + this.Value;
		}

		private static string termColumnOperatorToString(TermColumnOperator op)
		{ 
			switch (op)
			{
				case TermColumnOperator.Equal: return "=";
				case TermColumnOperator.Higer: return ">";
				case TermColumnOperator.Lower: return "<";
				case TermColumnOperator.HigerOrEqual: return ">=";
				case TermColumnOperator.LowerOrEqual: return "<=";
				default: return "CONTAINS";
			}
		}
	}
}
