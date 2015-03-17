using System;
using System.Collections.Concurrent;

namespace ToolsPack.Thread
{
	/// <summary>
	///     http://johnculviner.com/achieving-named-lock-locker-functionality-in-c-4-0/
	/// </summary>
	public class NamedLocker<T>
	{
		private readonly ConcurrentDictionary<T, object> _lockDict = new ConcurrentDictionary<T, object>();

		//get a lock for use with a lock(){} block
		public object GetLock(T name)
		{
			return _lockDict.GetOrAdd(name, s => new object());
		}

		//run a short lock inline using a lambda
		public TResult RunWithLock<TResult>(T name, Func<TResult> body)
		{
			lock (_lockDict.GetOrAdd(name, s => new object()))
				return body();
		}

		//run a short lock inline using a lambda
		public void RunWithLock(T name, Action body)
		{
			lock (_lockDict.GetOrAdd(name, s => new object()))
				body();
		}

		//remove an old lock object that is no longer needed
		public void RemoveLock(T name)
		{
			object o;
			_lockDict.TryRemove(name, out o);
		}
	}
}