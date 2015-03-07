﻿using System;
using System.Collections.Concurrent;

namespace Common.Thread
{
	/// <summary>
	///     http://johnculviner.com/achieving-named-lock-locker-functionality-in-c-4-0/
	/// </summary>
	public class NamedLocker
	{
		private readonly ConcurrentDictionary<string, object> _lockDict = new ConcurrentDictionary<string, object>();

		//get a lock for use with a lock(){} block
		public object GetLock(string name)
		{
			return _lockDict.GetOrAdd(name, s => new object());
		}

		//run a short lock inline using a lambda
		public TResult RunWithLock<TResult>(string name, Func<TResult> body)
		{
			lock (_lockDict.GetOrAdd(name, s => new object()))
				return body();
		}

		//run a short lock inline using a lambda
		public void RunWithLock(string name, Action body)
		{
			lock (_lockDict.GetOrAdd(name, s => new object()))
				body();
		}

		//remove an old lock object that is no longer needed
		public void RemoveLock(string name)
		{
			object o;
			_lockDict.TryRemove(name, out o);
		}
	}
}