using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ToolsPack.Parser.Sqp
{
	/// <summary>
	/// A SearchQueryTerm represents a simple text example: "CAC40".
	/// It's a part of the search query which means we are looking for object which contains the text "CAC40"
	/// </summary>
	public class SearchQueryTermSimple : SearchQueryTerm
	{
		public readonly string Term;

		internal SearchQueryTermSimple(string term)
		{
			this.Term = term;
			if (this.Term == null)
			{
				this.Term = "";///Avoid NullRef
			}
		}

		public override string ToString()
		{
			return this.Term;
		}
	}
}
