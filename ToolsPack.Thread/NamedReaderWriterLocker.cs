﻿using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ToolsPack.Thread
{
	/// <summary>
	///     http://johnculviner.com/achieving-named-lock-locker-functionality-in-c-4-0/
	/// </summary>
	public class NamedReaderWriterLocker<T>
	{
		private readonly ConcurrentDictionary<T, ReaderWriterLockSlim> _lockDict =
			new ConcurrentDictionary<T, ReaderWriterLockSlim>();

		public ReaderWriterLockSlim GetLock(T name)
		{
			return _lockDict.GetOrAdd(name, s => new ReaderWriterLockSlim());
		}

		public TResult RunWithReadLock<TResult>(T name, Func<TResult> body)
		{
			ReaderWriterLockSlim rwLock = GetLock(name);
			try
			{
				rwLock.EnterReadLock();
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
	}
}