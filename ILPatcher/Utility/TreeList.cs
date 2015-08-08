using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ILPatcher.Utility
{
	public class TreeList<T> : TreeListNode<T>, IEnumerable<T>
	{
		public TreeList()
			: base(default(T), null)
		{
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new TreeListEnumerator(this, true);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new TreeListEnumerator(this, true);
		}
	}

	public class TreeListNode<T> : IEnumerable<T>, ICollection<TreeListNode<T>>
	{
		protected T data;
		public T Value { get { return data; } set { value = data; } }
		protected TreeListNode<T> parent;
		public TreeListNode<T> Parent { get { return parent; } }
		protected readonly List<TreeListNode<T>> children;
		public ReadOnlyCollection<TreeListNode<T>> Children { get { return children.AsReadOnly(); } }

		protected TreeListNode<T> next;
		protected TreeListNode<T> previous;

		public TreeListNode(T data, TreeListNode<T> parent)
		{
			this.data = data;
			this.parent = parent;
			children = new List<TreeListNode<T>>();
		}

		public void Add(T item)
		{
			(this as ICollection<TreeListNode<T>>).Add(new TreeListNode<T>(item, this));
		}

		#region IEnumerator

		public IEnumerator<T> GetEnumerator()
		{
			return new TreeListEnumerator(this, false);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new TreeListEnumerator(this, false);
		}

		protected class TreeListEnumerator : IEnumerator<T>
		{
			private TreeListNode<T> start;
			private TreeListNode<T> current;
			private bool hasCurrent;
			private bool includingCurrent;

			internal TreeListEnumerator(TreeListNode<T> startNode, bool includeCurrent)
			{
				start = startNode;
				current = startNode;
				includingCurrent = includeCurrent;
				hasCurrent = !includeCurrent;
			}

			T IEnumerator<T>.Current { get { return current.data; } }

			object IEnumerator.Current { get { return current.data; } }

			bool IEnumerator.MoveNext()
			{
				if (!hasCurrent)
				{
					hasCurrent = true;
					return true;
				}

				if (current.children.Count > 0)
				{
					current = current.children.First();
					hasCurrent = false;
					return true;
				}

				if (current.next != null)
				{
					current = current.next;
					hasCurrent = false;
					return true;
				}

				if (current.parent != null)
				{
					current = current.parent.next;
					if (current != null)
					{
						hasCurrent = false;
						return true;
					}
				}
				return false;
			}

			void IEnumerator.Reset()
			{
				current = start;
				hasCurrent = !includingCurrent;
			}

			void IDisposable.Dispose()
			{
				start = null;
				current = null;
			}
		}

		#endregion

		#region IList

		public void Add(TreeListNode<T> item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			var end = children.Last();
			end.next = item;
			item.previous = end;
			children.Add(item);
		}

		public void Clear()
		{
			children.Clear();
		}

		public bool Contains(TreeListNode<T> item)
		{
			return children.Contains(item);
		}

		public void CopyTo(TreeListNode<T>[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public int Count { get { return children.Count; } }

		public bool IsReadOnly { get { return false; } }

		public bool Remove(TreeListNode<T> item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			if (item.next != null)
			{
				item.next.previous = item.previous;
				if (item.previous != null)
				{
					item.previous.next = null;
					item.previous = null;
				}
				item.next = null;
			}
			return children.Remove(item);
		}

		IEnumerator<TreeListNode<T>> IEnumerable<TreeListNode<T>>.GetEnumerator()
		{
			return children.GetEnumerator();
		}

		#endregion
	}
}
