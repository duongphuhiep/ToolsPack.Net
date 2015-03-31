﻿using System;

namespace DiversDoNet.Tests.model
{
	[Serializable]
	public class Resa
	{
		[Serializable]
		public class Key
		{
			public virtual string DealId { get; set; }
			public virtual string Client { get; set; }

			public Key()
			{
			}
			public Key(string dealId, string client)
			{
				DealId = dealId;
				Client = client;
			}

			#region Equals & HashCode (generated by ReSharper)

			protected bool Equals(Key other)
			{
				return string.Equals(DealId, other.DealId) && string.Equals(Client, other.Client);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((Key) obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((DealId != null ? DealId.GetHashCode() : 0)*397) ^ (Client != null ? Client.GetHashCode() : 0);
				}
			}

			#endregion
		}

		private Key _k = new Key();
		public virtual Key K {
			get { return _k; }
			set { _k = value; }
		}

		public virtual string DealId {
			get { return _k.DealId; }
			set { _k.DealId = value; }
		}
		public virtual string Client
		{
			get { return _k.Client; }
			set { _k.Client = value; }
		}
		public virtual double Amount { get; set; }

		public Resa()
		{
		}

		public Resa(string dealId, string client, double amount)
		{
			DealId = dealId;
			Client = client;
			Amount = amount;
		}

		public override string ToString()
		{
			return string.Format("(Resa {0} {1} {2})", DealId, Client, Amount);
		}

	}
}