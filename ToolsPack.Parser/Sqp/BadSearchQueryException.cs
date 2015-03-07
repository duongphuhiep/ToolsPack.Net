using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ToolsPack.Parser.Sqp
{
	public class BadSearchQueryException : ApplicationException
	{
		public BadSearchQueryException() : base()
		{

		}
		public BadSearchQueryException(string message): base(message)
		{

		}
	}
}
