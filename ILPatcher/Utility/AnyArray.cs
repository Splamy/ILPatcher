using System;

namespace ILPatcher.Utility
{
	class AnyArray<T> where T : class
	{
		public T this[int key]
		{
			get
			{
				if (key < array.Length)
					return array[key];
				else
					return null;
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
						array[i] = null;
				}
				else
				{
					T[] tmp;
					if (size - max < 10) //TODO: make some tests, im sleepy
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
	}
}
