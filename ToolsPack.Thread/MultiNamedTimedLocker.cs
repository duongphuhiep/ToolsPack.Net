using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace ToolsPack.Thread
{
	/// <summary>
	/// like NamedTimedLocker, but lock a list of names
	/// 
	/// static readonly MultiNamedTimedLocker<string> CustomerLocker = new MultiNamedTimedLocker<string>();
	/// 
	/// using (customerLocker.Lock(new[] {"hiep", "nhu"}, 100))
	/// {
	///		synchronized code
	/// }
	/// 
	/// </summary>
	public class MultiNamedTimedLocker<T> : IDisposable where T : IComparable
	{
		private readonly ConcurrentDictionary<T, LockRef> _refs = new ConcurrentDictionary<T, LockRef>();
		private class LockRef
		{
			public volatile bool IsLocked = false;
		}

		public class Locker : IDisposable
		{
			private readonly MultiNamedTimedLocker<T> _owner;
			/// <summary>
			/// unique ordered names
			/// </summary>
			private readonly T[] _uniqueOderedNames;
			private readonly int _lastIndexLocked;

			public Locker(MultiNamedTimedLocker<T> owner, T[] uniqueNames, int timeOutPerNameInMiliSecond)
			{
				_owner = owner;
				_uniqueOderedNames = uniqueNames;
				Array.Sort(_uniqueOderedNames); //avoid deadlock

				for (var i = 0; i < _uniqueOderedNames.Length; i++)
				{
					_owner.EnterLock(uniqueNames[i], timeOutPerNameInMiliSecond);
					_lastIndexLocked = i;
				}
			}

			public void Dispose()
			{
				for (var i = _lastIndexLocked; i >= 0; i--)
				{
					_owner.ExitLock(_uniqueOderedNames[i]);
				}
			}
		}

		private void EnterLock(T name, int timeOut)
		{
			var reference = GetReference(name);
			if (Monitor.TryEnter(reference, timeOut))
			{
				reference.IsLocked = true;
			}
			else
			{
				throw new TimeoutException("Failed acquired lock on " + name + " after " + timeOut + " ms");
			}
		}

		private void ExitLock(T name)
		{
			var reference = GetReference(name);
			if (reference.IsLocked)
			{
				reference.IsLocked = false;
				Monitor.Exit(reference);
			}
		}

		public Locker Lock(T[] names, int timeOutPerNameInMiliSecond)
        {
			if (names == null || !names.Any())
			{
				throw new ArgumentNullException("names", "empty argument");
			}
			var uniqueNames = names.Distinct().ToArray();
			return new Locker(this, uniqueNames, timeOutPerNameInMiliSecond);
        }

		private LockRef GetReference(T name)
		{
			return _refs.GetOrAdd(name, s => new LockRef());
		}

		public int CountLockedNames()
		{
			return _refs.Values.Count(r => r.IsLocked);
		}

		public void Dispose()
        {
	        var orderedNames = _refs.Keys.ToArray();
			Array.Sort(orderedNames);

			for (var i = orderedNames.Length-1; i >= 0; i--)
			{
				ExitLock(orderedNames[i]);
			}
        }
	}
}
















//using System;
//using System.Collections;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Threading;
//using System.Xml;

//namespace DeliveryRiskCalculator.Utils
//{
//	/// <summary>
//	/// like NamedTimedLocker, but lock a list of names
//	/// 
//	/// var locker = new MultiNamedTimedLocker<string>();
//	/// using (locker.Lock(new[] {"hiep", "nhu"}, 100))
//	/// {
//	///              synchronized code
//	/// }
//	/// </summary>
//	public class MultiNamedTimedLocker<T> : IDisposable where T : IComparable
//	{
//		private readonly ConcurrentDictionary<T, LockRef> _refs = new ConcurrentDictionary<T, LockRef>();
//		private class LockRef
//		{
//			public int IsLocked = 0;
//		}

//		public class Locker : IDisposable
//		{
//			private readonly MultiNamedTimedLocker<T> _owner;
//			/// <summary>
//			/// unique ordered names
//			/// </summary>
//			private readonly T[] _uniqueOderedNames;
//			private readonly int _lastIndexLocked;

//			public Locker(MultiNamedTimedLocker<T> owner, T[] uniqueNames, int timeOutPerNameInMiliSecond)
//			{
//				_owner = owner;
//				_uniqueOderedNames = uniqueNames;
//				Array.Sort(_uniqueOderedNames); //avoid deadlock

//				for (var i = 0; i < _uniqueOderedNames.Length; i++)
//				{
//					_owner.EnterLock(uniqueNames[i], timeOutPerNameInMiliSecond);
//					_lastIndexLocked = i;
//				}
//			}

//			public void Dispose()
//			{
//				for (var i = _lastIndexLocked; i >= 0; i--)
//				{
//					_owner.ExitLock(_uniqueOderedNames[i]);
//				}
//			}
//		}

//		private void EnterLock(T name, int timeOut)
//		{
//			var reference = GetReference(name);
//			var sw = Stopwatch.StartNew();
//			//var sts = new SpinWait();
//			do
//			{
//				if (Interlocked.Exchange(ref reference.IsLocked, 1) == 0)
//				{
//					return;
//				}
//				Thread.Sleep(1);//Thread leaves this code here...
//				//this thread can do some other work
//				// Another thread can take his time slice
//				//And when the Another thread inished,
//				//this thread c\omes to next line which is while..
//			} while (sw.ElapsedMilliseconds <= timeOut);
//			throw new TimeoutException("Failed acquired lock on " + name + " after " + timeOut + " ms");
            

//			//if (Monitor.TryEnter(reference, timeOut))
//			//{
//			//    reference.IsLocked = true;
//			//}
//			//else
//			//{
//			//    throw new TimeoutException("Failed acquired lock on " + name + " after " + timeOut + " ms");
//			//}
//		}

//		private void ExitLock(T name)
//		{
//			var reference = GetReference(name);
//			if (Interlocked.CompareExchange(ref reference.IsLocked, 0, 1) != 1)
//			{
//				throw new Exception("You are trying to release somthg that u never acquired.");
//			}
//		}

//		public Locker Lock(T[] names, int timeOutPerNameInMiliSecond)
//		{
//			if (names == null || !names.Any())
//			{
//				throw new ArgumentNullException("names", "empty argument");
//			}
//			var uniqueNames = names.Distinct().ToArray();
//			return new Locker(this, uniqueNames, timeOutPerNameInMiliSecond);
//		}

//		private LockRef GetReference(T name)
//		{
//			return _refs.GetOrAdd(name, s => new LockRef());
//		}

//		public int CountLockedNames()
//		{
//			return _refs.Values.Count(r => r.IsLocked == 1);
//		}

//		public void Dispose()
//		{
//			var orderedNames = _refs.Keys.ToArray();
//			Array.Sort(orderedNames);

//			for (var i = orderedNames.Length - 1; i >= 0; i--)
//			{
//				ExitLock(orderedNames[i]);
//			}
//		}
//	}
//}



