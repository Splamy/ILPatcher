using System;
using System.Collections.Generic;

namespace ILPatcher.Utility
{
	class AnyArray<T> : ICollection<T>
	{
		public T this[int key]
		{
			get
			{
				if (key < array.Length)
					return array[key];
				else
					return default(T);
			}
			set
			{
				if (key >= Length)
					ExtendTo(key + 1);
				array[key] = value;
			}
		}

		private T[] array;
		private int max;
		public int Length
		{
			get { return max; }
			set { ExtendTo(value); }
		}

		public AnyArray()
		{
			array = new T[0];
		}

		public AnyArray(int size)
		{
			array = new T[size];
		}

		private void ExtendTo(int size)
		{
			if (size > max)
			{
				if (size < array.Length)
				{
					for (int i = max; i < size; i++)
						array[i] = default(T);
				}
				else
				{
					T[] tmp;
					if (size - max < 10)
						tmp = new T[max + 10];
					else
						tmp = new T[size];
					Array.Copy(array, tmp, max);
					array = tmp;
				}
			}
			max = size;
		}

		public T[] ToArray()
		{
			T[] tmp = new T[max];
			Array.Copy(array, tmp, max);
			return tmp;
		}

		#region ICollection

		public void Add(T item)
		{
			throw new InvalidOperationException();
		}

		public void Clear()
		{
			ExtendTo(0);
		}

		public bool Contains(T item)
		{
			if (item == null)
				return false;

			for (int i = 0; i < max; i++)
				if (item.Equals(array[i]))
					return true;
			return false;
		}

		public void CopyTo(T[] destination, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(destination));
			if (max + arrayIndex >= destination.Length)
				throw new ArgumentException("The destination array is not big enough for the data");

			for (int i = 0; i < max; i++)
				destination[i + arrayIndex] = array[i];
		}

		public int Count
		{
			get { return max; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			for (int i = 0; i < max; i++)
				if (item.Equals(array[i]))
				{
					array[i] = default(T);
					return true;
				}
			return false;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new AnyArrayEnumerator(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new AnyArrayEnumerator(this);
		}

		private class AnyArrayEnumerator : IEnumerator<T>
		{
			int pos = -1;
			AnyArray<T> myArr;

			public AnyArrayEnumerator(AnyArray<T> pMyArr)
			{
				myArr = pMyArr;
			}

			public T Current{	get { return myArr.array[pos]; }}

			public void Dispose() {}

			object System.Collections.IEnumerator.Current { get { return myArr.array[pos]; } }

			public bool MoveNext()
			{
				return ++pos < myArr.max;
			}

			public void Reset()
			{
				pos = -1;
			}
		}

		#endregion
	}
}
