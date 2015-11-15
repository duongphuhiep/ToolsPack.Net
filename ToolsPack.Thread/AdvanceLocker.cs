using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace ToolsPack.Thread
{
	/// <summary>
	///     http://johnculviner.com/achieving-named-lock-locker-functionality-in-c-4-0/
	/// </summary>
	public class AdvanceLocker<T>
	{
		private readonly ConcurrentDictionary<T, object> _lockDict =
			new ConcurrentDictionary<T, object>();

		public ReaderWriterLockSlim GetLock(T name)
		{
			return _lockDict.GetOrAdd(name, s => new object());
		}

		public void Register(T[] names)
		{
			foreach (var name in names)
			{
				if (_lockDict.ContainsKey(name))
				{

				}
			}
		}

		public TResult RunWithReadLock<TResult>(T name, Func<TResult> body)
		{
			ReaderWriterLockSlim rwLock = GetLock(name);
			try
			{
				rwLock.EnterReadLock();
				rwLock.
				return body();
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		public void RunWithReadLock(T name, Action body)
		{
			ReaderWriterLockSlim rwLock = GetLock(name);
			try
			{
				rwLock.EnterReadLock();
				body();
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		public TResult RunWithWriteLock<TResult>(T name, Func<TResult> body)
		{
			ReaderWriterLockSlim rwLock = GetLock(name);
			try
			{
				rwLock.EnterWriteLock();
				return body();
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}

		public void RunWithWriteLock(T name, Action body)
		{
			ReaderWriterLockSlim rwLock = GetLock(name);
			try
			{
				rwLock.EnterWriteLock();
				body();
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}

		public TResult RunWithWriteLock<TResult>(IEnumerable<T> names, Func<TResult> body)
		{
			var rwLocks = from name in names select GetLock(name);
			try
			{
				foreach (var rwLock in rwLocks)
				{
					rwLock.EnterWriteLock();
				}
				return body();
			}
			finally
			{
				foreach (var rwLock in rwLocks)
				{
					rwLock.ExitWriteLock();
				}
			}
		}

		public void RunWithWriteLock(IEnumerable<T> names, Action body)
		{
			var rwLocks = from name in names select GetLock(name);
			try
			{
				foreach (var rwLock in rwLocks)
				{
					rwLock.EnterWriteLock();
				}
				body();
			}
			finally
			{
				foreach (var rwLock in rwLocks)
				{
					rwLock.ExitWriteLock();
				}
			}
		}

	}
}