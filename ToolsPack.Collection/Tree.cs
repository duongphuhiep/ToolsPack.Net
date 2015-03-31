using System;
using System.Collections.Generic;

namespace ToolsPack.Collection
{
	/// <summary>
	/// A generic treeNode structure.
	/// </summary>	
	public class Tree<T> 
	{
		public T Content;
		
		private Tree<T> parent;
		private List<Tree<T>> children = new List<Tree<T>>();

		public Tree<T> Parent
		{
			get { return this.parent; }
		}

		public bool IsRoot { get { return this.Parent == null; } }

		public IEnumerable<Tree<T>> Children { get { return this.children; } }

		public bool IsLeaf { get { return this.children.Count == 0; } }

		public bool ChildrenContains(T item)
		{
			foreach (Tree<T> current in this.Children)
			{
				if (current.Content.Equals(item)) { return true; }
			}
			return false;
		}

		public int ChildrenCount { get { return this.children.Count; } }

		public Tree<T> GetChild(int index)
		{
			return this.children[index];
		}

		public Tree()
		{
		}

		public Tree(T content)
		{
			this.Content = content;
		}

		public void AddChild(Tree<T> child)
		{
			if (child.Parent != null)
			{
				throw new ArgumentException();
			}
			child.parent = this;
			this.children.Add(child);
		}

		public void RemoveChild(Tree<T> child)
		{
			if (child.Parent != this) 
			{
				throw new ArgumentException();
			}
			child.parent = null;
			this.children.Remove(child);
		}

		/// <summary>
		/// For debug
		/// </summary>		
		public override string ToString()
		{
			return "NodeContent{" + this.Content + "}";
		}

		public void Sort(Comparer<T> comparer)
		{
			this.Sort(new ContentComparer<T>(comparer));
		}

		private void Sort(ContentComparer<T> comparer)
		{
			this.children.Sort(comparer);
			foreach (Tree<T> child in this.Children)
			{
				child.Sort(comparer);
			}
		}
		
		/// <summary>
		/// For the Sort()
		/// </summary>
		private struct ContentComparer<W> : IComparer<Tree<W>>
		{
			private Comparer<W> comparer;
			public ContentComparer(Comparer<W> comparer)
			{
				this.comparer = comparer;
			}

			public int Compare(Tree<W> x, Tree<W> y)
			{
				return comparer.Compare(x.Content, y.Content);
			}
		
		}
	}
}
