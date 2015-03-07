using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ToolsPack.Collection
{
	[DebuggerDisplay("this.Count = {this.Count}")]
	[CLSCompliant(true)]
	public class UniqueList<T> : ICollection<T>
	{
		#region Fields

		private readonly List<T> list = new List<T>();
		private readonly Dictionary<T, object> hashtable = new Dictionary<T, object>();

		#endregion Fields

		#region Constructors

		public UniqueList()
		{
		}

		public UniqueList(IEnumerable<T> collection)
		{
			this.AddRange(collection);
			//foreach (T item in collection)
			//{
			//	this.Add(itemn);
			//}
		}

		#endregion Constructors

		#region Management of the List

		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public T this[int index]
		{
			get { return this.list[index]; }
		}
		
		public void Add(T item)
		{
			if (item == null)///For the momment null is not accepted.
			{
				throw new ArgumentNullException();
			}
			if (!this.Contains(item))///Perf= 2.66% during BusinessLogic. It would be great if we can do the TryInsert with just one look-up.
			{
				this.list.Add(item);
				this.hashtable.Add(item, null);
			}
		}

		//public void InsertFirst(T item)
		//{
		//    if (item == null)///For the momment null is not accepted.
		//    {
		//        throw new ArgumentNullException();
		//    }
		//    if (!this.Contains(item))
		//    {
		//        this.list.Insert(0, item);
		//        this.hashtable.Add(item, null);
		//    }			
		//}


		public void AddRange(IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				this.Add(item);
			}
		}

		public void AddRange(UniqueList<T> collection)
		{
			foreach (T item in collection)
			{
				this.Add(item);
			}
		}

		public void Clear()
		{
			this.list.Clear();
			this.hashtable.Clear();
		}

		public bool Contains(T item)
		{
			return this.hashtable.ContainsKey(item);
		}

		public void CopyTo(T[] arrayDest, int arrayIndex)
		{
			this.list.CopyTo(arrayDest, arrayIndex);
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return ((ICollection<T>)this.list).IsReadOnly;
			}
		}

		public bool Remove(T item)
		{
			this.hashtable.Remove(item);
			return this.list.Remove(item);
		}

		public T[] ToArray()
		{
			T[] array = new T[this.list.Count];
			this.CopyTo(array, 0);
			return array;
		}

		public List<T> ToList()
		{
			return new List<T>(this.list);
		}

		public List<T> GetList()
		{
			return this.list;
		}

		/// <summary>		
		/// Return a new list with items there are both in this list AND in the argument list.
		/// </summary>
		public UniqueList<T> Intersect(IEnumerable<T> l)
		{
			UniqueList<T> returned = new UniqueList<T>();
			foreach (T item in l)
			{
				if (this.Contains(item))
				{
					returned.Add(item);
				}
			}
			return returned;
		}

		#endregion Managment of the List

		#region Overrided object methods

		public override string ToString()
		{
			string returned = "[";

			int i = 0;
			foreach (object element in this)
			{
				if (element != null)
				{
					returned += element.ToString();
				}
				else
				{
					returned += "null";
				}

				i++;

				if (i != this.Count)
				{
					returned += ",";
				}
				if (i > 8)
				{
					returned += "...";
					break;
				}
			}

			returned += "]";

			return returned;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;

			foreach (T item in this.list)
			{
				hashCode += item.GetHashCode();
			}

			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is UniqueList<T>))
			{
				return false;
			}

			UniqueList<T> otherUniqueList = (UniqueList<T>)obj;

			if (this.Count != otherUniqueList.Count)
			{
				return false;
			}

			foreach (T item in this.list)
			{
				if (!otherUniqueList.Contains(item))
				{
					return false;
				}
			}

			return true;
		}

		#endregion Overrided object methods
	}
}
