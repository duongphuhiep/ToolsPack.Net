using System.Threading;

namespace ToolsPack.Thread
{
	/// <summary>
	/// Wrapper of System.Threading.Timer that provides Start and Stop methods.
	/// </summary>
	public class TimerWrapper
	{
		//private static readonly ILog Log = log4net.LogManager.GetLogger(typeof(TimerWrapper));

		private readonly object myLock = new object();

		private volatile Timer _timer;

		private volatile bool _stopRequested;

		public TimerWrapper(TimerCallback callback, int period, string name)
		{
			Name = name;
			Callback = callback;
			State = null;
			DueTime = 0;
			Period = period;
		}

		public TimerCallback Callback { get; private set; }
		public object State { get; private set; }
		public int DueTime { get; private set; }
		public int Period { get; private set; }
		public string Name { get; set; }
		public bool StopRequested { get { return _stopRequested; } }

		public void CallbackWrapper(object state)
		{
			if (_stopRequested)
			{
				//Log.DebugFormat("Timer {0} ignored callback because of stop request.", Name);
				return;
			}

			Callback(state);
		}

		public void Start()
		{
			lock (myLock)
			{
				StopNonThreadSafe();
				_stopRequested = false;
				_timer = new Timer(CallbackWrapper, State, DueTime, Period);
				//Log.DebugFormat("Timer {0} started.", Name);
			}
		}

		public void Stop()
		{
			lock (myLock)
			{
				StopNonThreadSafe();
				_stopRequested = true;
				//Log.DebugFormat("Timer {0} stopped", Name);
			}
		}

		private void StopNonThreadSafe()
		{
			if (_timer != null)
			{
				_timer.Dispose();
			}
		}
	}
}
