using System;
using System.Collections.Generic;
using UnityEngine.Pool;
namespace External
{

	public class ObjectPoolOptimized<T> : IDisposable, IObjectPool<T> where T : class
	{
		static IComparer<T> Sorter = Comparer<T>.Create(Comparison);
		static int Comparison(T x, T y) => 0;

		internal readonly HashSet<T> m_Collection;
		internal readonly List<T> m_Collection2;
		private readonly Func<T> m_CreateFunc;
		private readonly Action<T> m_ActionOnGet;
		private readonly Action<T> m_ActionOnRelease;
		private readonly Action<T> m_ActionOnDestroy;
		private readonly int m_MaxSize;
		internal bool m_CollectionCheck;

		public int CountAll { get; private set; }

		public int CountActive => CountAll - CountInactive;

		public int CountInactive => m_Collection.Count;

		public ObjectPoolOptimized(
			Func<T> createFunc,
			Action<T> actionOnGet = null,
			Action<T> actionOnRelease = null,
			Action<T> actionOnDestroy = null,
			bool collectionCheck = true,
			int defaultCapacity = 10,
			int maxSize = 10000)
		{
			if (createFunc == null)
				throw new ArgumentNullException(nameof(createFunc));
			if (maxSize <= 0)
				throw new ArgumentException("Max Size must be greater than 0", nameof(maxSize));
			m_Collection = new HashSet<T>();
			m_Collection2 = new List<T>(defaultCapacity);
			for (int i = 0; i < defaultCapacity; i++)
			{
				var element = createFunc();
				m_Collection.Add(element);
				m_Collection.Add(element);
			}
			m_CreateFunc = createFunc;
			m_MaxSize = maxSize;
			m_ActionOnGet = actionOnGet;
			m_ActionOnRelease = actionOnRelease;
			m_ActionOnDestroy = actionOnDestroy;
			m_CollectionCheck = collectionCheck;
		}

		public T Get()
		{
			T obj;
			if (m_Collection2.Count == 0)
			{
				obj = m_CreateFunc();
				++CountAll;
			}
			else
			{
				obj = m_Collection2[0];
				m_Collection.Remove(obj);
				m_Collection2.RemoveAt(0);
			}
			Action<T> actionOnGet = m_ActionOnGet;
			if (actionOnGet != null)
				actionOnGet(obj);
			return obj;
		}

		public PooledObject<T> Get(out T v) => new PooledObject<T>(v = Get(), (IObjectPool<T>)this);

		public void Release(T element) 
		{
			if (m_CollectionCheck && m_Collection.Count > 0 && m_Collection.Contains(element))
			{
				return;
				throw new InvalidOperationException($"Trying to release an object '{element}' that has already been released to the pool.");
			}
			Action<T> actionOnRelease = m_ActionOnRelease;
			if (actionOnRelease != null)
				actionOnRelease(element);
			if (CountInactive < m_MaxSize)
			{
				m_Collection.Add(element);
				m_Collection2.Add(element);
			}
			else
			{
				Action<T> actionOnDestroy = m_ActionOnDestroy;
				if (actionOnDestroy != null)
					actionOnDestroy(element);
			}
		}

		public void Clear()
		{
			if (m_ActionOnDestroy != null)
			{
				foreach (T obj in m_Collection)
					m_ActionOnDestroy(obj);
			}
			m_Collection.Clear();
			m_Collection2.Clear();
			CountAll = 0;
		}

		public void Dispose() => Clear();
	}
}