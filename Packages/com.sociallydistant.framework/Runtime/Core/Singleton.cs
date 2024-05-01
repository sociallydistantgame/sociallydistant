#nullable enable

using System;

namespace Core
{
	public sealed class Singleton<T> where T : class
	{
		private T? instance;

		public T? Instance => instance;

		public T MustGetInstance()
		{
			if (instance == null)
				throw new InvalidOperationException("Object is null");

			return instance;
		}

		public void MustGetInstance(out T result)
		{
			result = MustGetInstance();
		}

		public void SetInstance(T? newInstance)
		{
			if (instance == newInstance)
				return;

			if (newInstance == null)
			{
				instance = newInstance;
				return;
			}

			if (instance != null)
				throw new InvalidOperationException("You are trying to set a new non-null instance on a singleton that already holds a valid reference.");

			instance = newInstance;
		}
	}
}