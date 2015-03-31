using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiversDoNet.Tests.model
{
	[Serializable]
	public class Exposure
	{
		public virtual string Client { get; set; }
		public virtual double Amount { get; set; }

		public Exposure()
		{
		}
		public Exposure(string client, double amount)
		{
			Client = client;
			Amount = amount;
		}
		public override string ToString()
		{
			return string.Format("(Exposure {0} {1})", Client, Amount);
		}
	}
}
