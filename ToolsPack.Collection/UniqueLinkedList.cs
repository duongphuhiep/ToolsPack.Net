using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ToolsPack.Collection
{
	/// <summary>
	/// The unique liked list is implemented with a linked list,
	/// thus providing very good performances with Add/AddRange.
	/// 
	/// It has better performance than UniqueList because the Add:
	///  - do not reallocate memory (linked-list)
	///  - do not store anything in a dictionnary.
	/// 
	/// Uniqueness is ensured at reading time. 
	/// The 'this.hashtable' is build just on the first reads (GetEnumerator implementation and in the Contains).	
	/// </summary>	
	/// <remarks>
	/// Avoid:
	///   foreach(Node n in xxx)
	///   {
	///     if(!uniqueLinkedList.Contains(n))
	///     {
	///          uniqueLinkedList.Add(n);
	///     }
	///   } 
	/// Because it destroy and reconstructs all the hastable at each iteration.
	/// </remarks>
	[DebuggerDisplay("this.Count = {this.Count}")]
	public class UniqueLinkedList<T> : ICollection<T>, IDisposable
	{
		#region UniqueLinkedListNode class (linked element)

		private class UniqueLinkedListNode
		{
			public T Value;
			public UniqueLinkedListNode Previous;
			public UniqueLinkedListNode Next;
		}

		#endregion UniqueLinkedListNode<T> class
		#region UniqueLinkedListEnum class

		private class UniqueLinkedListEnum : IEnumerator<T>
		{
			public UniqueLinkedList<T> list;
			private UniqueLinkedListNode current;

			public UniqueLinkedListEnum(UniqueLinkedList<T> list)
			{
				this.current = null;
				this.list = list;
			}

			T IEnumerator<T>.Current
			{
				get
				{
					return this.getCurrent();
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return this.getCurrent();
				}
			}

			private T getCurrent()
			{
				if (this.current != null) { return this.current.Value; }
				else { return default(T); }
			}

			bool System.Collections.IEnumerator.MoveNext()
			{
				if (this.list.reverse)
				{
					return this.getPrevious();
				}
				else
				{
					return this.getNext();
				}
			}

			private bool getNext()
			{
				if (this.current == null)
				{
					this.current = this.list.first;
				}
				else
				{
					this.current = this.current.Next;
				}

				if (this.current == null) /// list end
				{
					this.list.hashtableOk = true;
					return false;
				}
				else
				{
					if (this.list.hashtableOk) /// list already unique and hashtable built
					{
						return true;
					}
					else
					{
						bool alreadyFound;
						if (this.current.Value == null)
						{
							alreadyFound = this.list.containsNull;
						}
						else
						{
							alreadyFound = this.list.hashtable.ContainsKey(this.current.Value);
						}

						if (alreadyFound)
						{
							/// remove element already found
							UniqueLinkedListNode toRemove = this.current;
							if (toRemove.Previous != null) { toRemove.Previous.Next = toRemove.Next; }
							if (toRemove.Next != null) { toRemove.Next.Previous = toRemove.Previous; }
							if (toRemove == this.list.first) { this.list.first = toRemove.Next; }
							if (toRemove == this.list.last) { this.list.last = toRemove.Previous; }
							this.current = toRemove.Previous;
							/// and getNext from old current
							return this.getNext();
						}
						else
						{
							if (this.current.Value == null)
							{
								this.list.containsNull = true;
							}
							else
							{
								this.list.hashtable.Add(this.current.Value, null);
							}
							return true;
						}
					}
				}
			}

			private bool getPrevious()
			{
				if (this.current == null)
				{
					this.current = this.list.last;
				}
				else
				{
					this.current = this.current.Previous;
				}

				if (this.current == null) /// list begin
				{
					this.list.hashtableOk = true;
					return false;
				}
				else
				{
					if (this.list.hashtableOk) /// list already unique and hashtable built
					{
						return true;
					}
					else if (this.list.hashtable.ContainsKey(this.current.Value))
					{
						/// remove element already found
						UniqueLinkedListNode toRemove = this.current;
						if (toRemove.Previous != null) { toRemove.Previous.Next = toRemove.Next; }
						if (toRemove.Next != null) { toRemove.Next.Previous = toRemove.Previous; }
						if (toRemove == this.list.first) { this.list.first = toRemove.Next; }
						if (toRemove == this.list.last) { this.list.last = toRemove.Previous; }
						this.current = toRemove.Next;
						/// and getNext from old current
						return this.getPrevious();
					}
					else
					{
						this.list.hashtable.Add(this.current.Value, null);
						return true;
					}
				}
			}

			void System.Collections.IEnumerator.Reset()
			{
				this.current = null;
			}

			public bool Remove(T item)
			{
				this.current = null;
				UniqueLinkedListNode toRemove = null;
				while (this.getNext())
				{
					bool match = false;
					if (item == null)
					{
						match = (this.current.Value == null);
					}
					else
					{
						match = this.current.Value.Equals(item);
					}

					if (match)
					{
						toRemove = this.current;
						if (this.list.hashtableOk) /// this is the only occurence
						{
							break;
						}
					}
				}
				this.current = null;

				/// Here hashtable is now ok, keep it ok by removing element in it
				if (toRemove == null)
				{
					return false;
				}
				else
				{
					if (toRemove.Previous != null) { toRemove.Previous.Next = toRemove.Next; }
					if (toRemove.Next != null) { toRemove.Next.Previous = toRemove.Previous; }
					if (toRemove == this.list.first) { this.list.first = toRemove.Next; }
					if (toRemove == this.list.last) { this.list.last = toRemove.Previous; }
					if (item == null)
					{
						this.list.containsNull = false;
					}
					else
					{
						this.list.hashtable.Remove(toRemove.Value);
					}
					return true;
				}
			}


			public void Dispose()
			{
				this.list = null;
				this.current = null;
			}
		}

		#endregion UniqueLinkedListEnum class

		#region Fields

		private bool containsNull;
		private UniqueLinkedListNode first;
		private UniqueLinkedListNode last;

		private Dictionary<T, object> hashtable;
		private bool hashtableOk;

		private bool reverse;

		/// <summary>
		/// Used in foreach, perform a reverse order if set to true
		/// </summary>
		public bool Reverse 
		{ 
			get { return this.reverse; }
			set { this.reverse = value; }
		}

		#endregion Fields

		#region Constructors

		public UniqueLinkedList()
		{
			this.first = null;
			this.last = null;
			this.hashtable = new Dictionary<T, object>();
			this.hashtableOk = true;
			this.reverse = false;
			this.containsNull = false;
		}

		public UniqueLinkedList(IEnumerable<T> collection)
			: this()
		{
		  this.AddRange(collection);
		}

		#endregion Constructors
		
		#region IEnumerable<T> & IEnumerable Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			if (!this.hashtableOk)
			{
				/// the hashtable may have been partialy built.
				/// in this case, just rebuild it entirely.
				this.hashtable.Clear();
			}
			return new UniqueLinkedListEnum(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		#endregion
		
		#region IDisposable Members

		void IDisposable.Dispose()
		{
			this.hashtable.Clear();
		}

		#endregion
		
		#region ICollection<T> Members

		public bool IsEmpty
		{
			get { return (this.first == null); }
		}

		public void Add(T item)
		{
			UniqueLinkedListNode newItem = new UniqueLinkedListNode();
			newItem.Next = null;
			newItem.Value = item;

			if (this.IsEmpty)
			{
				this.first = this.last = newItem;
			}
			else
			{
				UniqueLinkedListNode oldLast = this.last;
				oldLast.Next = newItem;
				newItem.Previous = oldLast;
				this.last = newItem;
			}

			this.hashtable.Clear();
			this.containsNull = false;
			this.hashtableOk = false;
		}

		public void AddRange(IEnumerable<T> items)
		{
			foreach (T item in items)
			{ 
				this.Add(item); 
			}
		}

		public void AddRange(UniqueLinkedList<T> list)
		{
			if (!list.IsEmpty)
			{
				if (this.IsEmpty)
				{
					this.first = list.first;
					this.last = list.last;
				}
				else
				{
					this.last.Next = list.first;
					list.first.Previous = this.last;
					this.last = list.last;
				}

				this.hashtableOk = false;
				this.containsNull = false;
				this.hashtable.Clear();
			}
		}

		public void Clear()
		{
			this.first = null;
			this.last = null;
			this.hashtable.Clear();
			this.containsNull = false;
			this.hashtableOk = true;
		}

		public bool Contains(T item)
		{
			this.buildDictionaryIfNeeded();	
			return this.hashtable.ContainsKey(item);
		}

		private void buildDictionaryIfNeeded()
		{
			if (!this.hashtableOk)
			{
				this.hashtable.Clear();
				this.containsNull = false;
				foreach (T item in this)
				{
					/// do nothing, just iterate to build dictionary
				}
			}
		}

		public T Last()
		{
			this.buildDictionaryIfNeeded();
			if (this.last != null) { return this.last.Value; }
			else { return default(T); }
		}

		public T First()
		{
			if (this.first != null) { return this.first.Value; }
			else { return default(T); }
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			int i=0;
			foreach (T item in this)
			{
				array[arrayIndex + i] = item;
				i++;
			}
		}

		public int Count
		{
			get 
			{
				this.buildDictionaryIfNeeded();
				return this.hashtable.Count + (this.containsNull?1:0);
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			if (this.hashtableOk)
			{
				if (item == null)
				{
					if (!this.containsNull) { return false; }
				}
				else if (!this.hashtable.ContainsKey(item))
				{
					return false;
				}
			}

			return new UniqueLinkedListEnum(this).Remove(item);			
		}

		#endregion

		#region ToString

		public override string ToString()
		{
			return this.displayElements(20);
		}

		public string DisplayAllElements()
		{
			return this.displayElements(int.MaxValue);
		}

		private string displayElements(int maxNumberOfElements)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");///OdR: Do not use '{' to have nice format in expression debug display

			int i = 0;
			foreach (T item in this)
			{
				if (i != 0)
				{
					sb.Append(", ");
				}
				if (i >= maxNumberOfElements)
				{
					sb.Append("...");///Perf
					break;
				}
				sb.Append(item == null ? "<null>" : item.ToString());
				i++;
			}

			sb.Append("]");

			return sb.ToString();
		}

		#endregion ToString
	}
}
