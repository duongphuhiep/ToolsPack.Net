using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;
using log4net.Core;
using ToolsPack.Displayer;

namespace ToolsPack.Log4net
{
	/// <summary>
	/// Micro benchmark a block of code, add elapsed time in each log message and the total elapsed time of the code block
	/// 
	///            private static readonly ILog Log = LogManager.GetLogger(typeof(MyClass));
	/// 
	///            using (var etw = ElapsedTimeWatcher.Create(Log, "checkIntraday"))
	///            {
	///                Thread.Sleep(100);
	///                etw.Debug("step 1");
	/// 
	///                Thread.Sleep(200);
	///                etw.Debug("step 2");
	/// 
	///                Thread.Sleep(300);
	///                etw.Info("final step)");
	/// 
	///                Thread.Sleep(400);
	///            }
	/// </summary>
	public class ElapsedTimeWatcher : IDisposable
	{
		private bool _autoJumpContext = false;
		private int _autoJumpContextToInfo = 5;
		private int _autoJumpContextToWarning = 10;

		private bool _autoJump = false;
		private int _autoJumpToInfo = 2;
		private int _autoJumpToWarning = 5;

		private LoggerLevel _sumLogLevel = LoggerLevel.Debug;

		private readonly ILog _log;
		private readonly Stopwatch _scopeSw;
		private readonly Stopwatch _unitarySw;
		private readonly string _scopeId;
		private readonly string _context;

		/// <summary>
		/// scopeId is DisplayMicroed on every log messages.
		/// context is DisplayMicroed at the end to tell total time spent on the scope
		/// </summary>
		private ElapsedTimeWatcher(ILog log, string scopeId, string context, string spaceBeforeLog)
		{
			_log = log;
			_scopeId = spaceBeforeLog + "  " + scopeId;
			_context = spaceBeforeLog + context;
			_scopeSw = Stopwatch.StartNew();
			_unitarySw = Stopwatch.StartNew();
		}

		#region Fluent API

		public static ElapsedTimeWatcher Create(ILog log, string scopeId, string context = null, string spaceBeforeLog = null)
		{
			if (String.IsNullOrWhiteSpace(context))
			{
				context = scopeId;
			}
			spaceBeforeLog = spaceBeforeLog ?? "";
			return new ElapsedTimeWatcher(log, scopeId, context, spaceBeforeLog);
		}

		/// <summary>
		/// For the last log (the total elpased time log)
		/// The log level automaticly jump up to INFO or WARN if the elapsed time exceed the threshold
		/// </summary>
		public ElapsedTimeWatcher AutoJumpSumLog(int miliSecondToInfo=5, int miliSecondToWarning=10)
		{
			_autoJumpContext = true;
			_autoJumpContextToInfo = miliSecondToInfo;
			_autoJumpContextToWarning = miliSecondToWarning;
			return this;
		}

		/// <summary>
		/// The log level automaticly jump up to INFO or WARN if the elapsed time exceed the threshold
		/// </summary>
		public ElapsedTimeWatcher AutoJump(int miliSecondToInfo = 2, int miliSecondToWarning = 5)
		{
			_autoJump = true;
			_autoJumpToInfo = miliSecondToInfo;
			_autoJumpToWarning = miliSecondToWarning;
			return this;
		}


		/// <summary>
		/// Set log level of the last message (on disposal)
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public ElapsedTimeWatcher SumMessageLevel(LoggerLevel level)
		{
			_sumLogLevel = level;
			return this;
		}

		public ElapsedTimeWatcher SumMessageInfo()
		{
			return SumMessageLevel(LoggerLevel.Info);
		}

		#endregion

		public void RestartScopeStopwatch()
		{
			_scopeSw.Stop();
			_scopeSw.Reset();
			_scopeSw.Start();
		}

		/// <summary>
		/// Restart the unitary stopwatch
		/// </summary>
		public void Restart()
		{
			_unitarySw.Stop();
			_unitarySw.Reset();
			_unitarySw.Start();
		}

		private LoggerLevel? GetLevel()
		{
			if (_scopeSw.ElapsedMilliseconds >= _autoJumpContextToWarning)
			{
				return LoggerLevel.Warn;
			}
			else if (_scopeSw.ElapsedMilliseconds >= _autoJumpContextToInfo)
			{
				return LoggerLevel.Info;
			}
			return null;
		}
		
		private LoggerLevel? GetLevelInScope()
		{
			if (_unitarySw.ElapsedMilliseconds >= _autoJumpToWarning)
			{
				return LoggerLevel.Warn;
			}
			else if (_unitarySw.ElapsedMilliseconds >= _autoJumpToInfo)
			{
				return LoggerLevel.Info;
			}
			return null;
		}

		public void Dispose()
		{
			if (_autoJumpContext)
			{
				var level = GetLevel();
				if (level.HasValue)
				{
					if (level.Value == LoggerLevel.Warn)
					{
						_log.Warn(_context + " - Total elapsed " + _scopeSw.DisplayMicro());
					}
					else
					{
						_log.Info(_context + " - Total elapsed " + _scopeSw.DisplayMicro());
					}
					return;
				}
			}

			switch (_sumLogLevel)
			{
				case LoggerLevel.Info:
					_log.Info(_context + " - Total elapsed " + _scopeSw.DisplayMicro());
					break;
				case LoggerLevel.Warn:
					_log.Warn(_context + " - Total elapsed " + _scopeSw.DisplayMicro());
					break;
				case LoggerLevel.Error:
					_log.Error(_context + " - Total elapsed " + _scopeSw.DisplayMicro());
					break;
				case LoggerLevel.Fatal:
					_log.Fatal(_context + " - Total elapsed " + _scopeSw.DisplayMicro());
					break;
				default:
					_log.Debug(_context + " - Total elapsed " + _scopeSw.DisplayMicro());
					break;
			}
		}

		#region ILog

		public ILogger Logger
		{
			get { return _log.Logger; }
		}

		public void Debug(object message)
		{
			if (!_log.IsDebugEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue)
				{
					if (level.Value == LoggerLevel.Warn)
					{
						_log.Warn(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message);
					}
					else
					{
						_log.Info(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message);
					}
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.Debug(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void Debug(object message, Exception exception)
		{
			if (!_log.IsDebugEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue)
				{
					if (level.Value == LoggerLevel.Warn)
					{
						_log.Warn(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message, exception);
					}
					else
					{
						_log.Info(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message, exception);
					}
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.Debug(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message, exception);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void DebugFormat(string format, params object[] args)
		{
			if (!_log.IsDebugEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue)
				{
					if (level.Value == LoggerLevel.Warn)
					{
						_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, args);
					}
					else
					{
						_log.InfoFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, args);
					}
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.DebugFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, args);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void DebugFormat(string format, object arg0)
		{
			if (!_log.IsDebugEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue)
				{
					if (level.Value == LoggerLevel.Warn)
					{
						_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0);
					}
					else
					{
						_log.InfoFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0);
					}
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.DebugFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void DebugFormat(string format, object arg0, object arg1)
		{
			if (!_log.IsDebugEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue)
				{
					if (level.Value == LoggerLevel.Warn)
					{
						_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1);
					}
					else
					{
						_log.InfoFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1);
					}
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.DebugFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void DebugFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!_log.IsDebugEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue)
				{
					if (level.Value == LoggerLevel.Warn)
					{
						_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1, arg2);
					}
					else
					{
						_log.InfoFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1, arg2);
					}
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.DebugFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!_log.IsDebugEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue)
				{
					if (level.Value == LoggerLevel.Warn)
					{
						_log.WarnFormat(provider, format, args);
					}
					else
					{
						_log.InfoFormat(provider, format, args);
					}
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.DebugFormat(provider, format, args);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void Info(object message)
		{
			if (!_log.IsInfoEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue && level.Value == LoggerLevel.Warn)
				{
					_log.Warn(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message);
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.Info(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void Info(object message, Exception exception)
		{
			if (!_log.IsInfoEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue && level.Value == LoggerLevel.Warn)
				{
					_log.Warn(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message, exception);
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.Info(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message, exception);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void InfoFormat(string format, params object[] args)
		{
			if (!_log.IsInfoEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue && level.Value == LoggerLevel.Warn)
				{
					_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, args);
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.InfoFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, args);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void InfoFormat(string format, object arg0)
		{
			if (!_log.IsInfoEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue && level.Value == LoggerLevel.Warn)
				{
					_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0);
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.InfoFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void InfoFormat(string format, object arg0, object arg1)
		{
			if (!_log.IsInfoEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue && level.Value == LoggerLevel.Warn)
				{
					_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1);
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.InfoFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void InfoFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!_log.IsInfoEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue && level.Value == LoggerLevel.Warn)
				{
					_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1, arg2);
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.InfoFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!_log.IsInfoEnabled) return;
			_unitarySw.Stop();
			if (_autoJump)
			{
				var level = GetLevelInScope();
				if (level.HasValue && level.Value == LoggerLevel.Warn)
				{
					_log.WarnFormat(provider, format, args);
					_unitarySw.Reset(); _unitarySw.Start();
					return;
				}
			}
			_log.InfoFormat(provider, format, args);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void Warn(object message)
		{
			if (!_log.IsWarnEnabled) return;
			_unitarySw.Stop();
			_log.Warn(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void Warn(object message, Exception exception)
		{
			if (!_log.IsWarnEnabled) return;
			_unitarySw.Stop();
			_log.Warn(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message, exception);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void WarnFormat(string format, params object[] args)
		{
			if (!_log.IsWarnEnabled) return;
			_unitarySw.Stop();
			_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, args);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void WarnFormat(string format, object arg0)
		{
			if (!_log.IsWarnEnabled) return;
			_unitarySw.Stop();
			_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void WarnFormat(string format, object arg0, object arg1)
		{
			if (!_log.IsWarnEnabled) return;
			_unitarySw.Stop();
			_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void WarnFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!_log.IsWarnEnabled) return;
			_unitarySw.Stop();
			_log.WarnFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!_log.IsWarnEnabled) return;
			_unitarySw.Stop();
			_log.WarnFormat(provider, format, args);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void Error(object message)
		{
			if (!_log.IsErrorEnabled) return;
			_unitarySw.Stop();
			_log.Error(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void Error(object message, Exception exception)
		{
			if (!_log.IsErrorEnabled) return;
			_unitarySw.Stop();
			_log.Error(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message, exception);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void ErrorFormat(string format, params object[] args)
		{
			if (!_log.IsErrorEnabled) return;
			_unitarySw.Stop();
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, args);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void ErrorFormat(string format, object arg0)
		{
			if (!_log.IsErrorEnabled) return;
			_unitarySw.Stop();
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void ErrorFormat(string format, object arg0, object arg1)
		{
			if (!_log.IsErrorEnabled) return;
			_unitarySw.Stop();
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!_log.IsErrorEnabled) return;
			_unitarySw.Stop();
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!_log.IsErrorEnabled) return;
			_unitarySw.Stop();
			_log.ErrorFormat(provider, format, args);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void Fatal(object message)
		{
			if (!_log.IsFatalEnabled) return;
			_unitarySw.Stop();
			_log.Fatal(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void Fatal(object message, Exception exception)
		{
			if (!_log.IsFatalEnabled) return;
			_unitarySw.Stop();
			_log.Fatal(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + message, exception);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void FatalFormat(string format, params object[] args)
		{
			if (!_log.IsFatalEnabled) return;
			_unitarySw.Stop();
			_log.FatalFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, args);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void FatalFormat(string format, object arg0)
		{
			if (!_log.IsFatalEnabled) return;
			_unitarySw.Stop();
			_log.FatalFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void FatalFormat(string format, object arg0, object arg1)
		{
			if (!_log.IsFatalEnabled) return;
			_unitarySw.Stop();
			_log.FatalFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void FatalFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!_log.IsFatalEnabled) return;
			_unitarySw.Stop();
			_log.FatalFormat(_scopeId + " - " + _unitarySw.DisplayMicro() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!_log.IsFatalEnabled) return;
			_unitarySw.Stop();
			_log.FatalFormat(provider, format, args);
			_unitarySw.Reset(); _unitarySw.Start();
		}

		public bool IsDebugEnabled
		{
			get { return _log.IsDebugEnabled; }
		}

		public bool IsInfoEnabled
		{
			get { return _log.IsInfoEnabled; }
		}

		public bool IsWarnEnabled
		{
			get { return _log.IsWarnEnabled; }
		}

		public bool IsErrorEnabled
		{
			get { return _log.IsErrorEnabled; }
		}

		public bool IsFatalEnabled
		{
			get { return _log.IsFatalEnabled; }
		}

		#endregion

		public enum LoggerLevel
		{
			Debug, Info, Warn, Error, Fatal
		}
	}
}
