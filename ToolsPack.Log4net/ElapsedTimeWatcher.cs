using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;
using log4net.Core;
using ToolsPack.Parser;

namespace ToolsPack.Log4net
{
	/// <summary>
	/// Micro benchmark a block of code, add elapsed time in each log message and the total elapsed time of the code block
	/// 
	///            private static readonly ILog Log = LogManager.GetLogger(typeof(MyClass));
	/// 
	///            using (var etw = new ElapsedTimeWatcher(Log, "checkIntraday"))
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
	public class ElapsedTimeWatcher
	{
		private readonly ILog _log;
		private readonly Stopwatch _scopeSw;
		private readonly Stopwatch _unitarySw;
		private readonly string _scopeId;

		public ElapsedTimeWatcher(ILog log, string scopeId)
		{
			_log = log;
			_scopeId = scopeId;
			_scopeSw = Stopwatch.StartNew();
			_unitarySw = Stopwatch.StartNew();
		}

		public void RestartScopeStopwatch()
		{
			_scopeSw.Restart();
		}

		public void RestartUnitaryStopwatch()
		{
			_unitarySw.Restart();
		}

		public void Dispose()
		{
			_log.Debug(_scopeId + ": Total elapsed " + _scopeSw.Display());
		}

		public ILogger Logger
		{
			get { return _log.Logger; }
		}

		public void Debug(object message)
		{
			if (!_log.IsDebugEnabled) return;
			_log.Debug(_scopeId + " - " + _unitarySw.Display() + " - " + message);
			_unitarySw.Restart();
		}

		public void Debug(object message, Exception exception)
		{
			if (!_log.IsDebugEnabled) return;
			_log.Debug(_scopeId + " - " + _unitarySw.Display() + " - " + message, exception);
			_unitarySw.Restart();
		}

		public void DebugFormat(string format, params object[] args)
		{
			if (!_log.IsDebugEnabled) return;
			_log.DebugFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, args);
			_unitarySw.Restart();
		}

		public void DebugFormat(string format, object arg0)
		{
			if (!_log.IsDebugEnabled) return;
			_log.DebugFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0);
			_unitarySw.Restart();
		}

		public void DebugFormat(string format, object arg0, object arg1)
		{
			if (!_log.IsDebugEnabled) return;
			_log.DebugFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1);
			_unitarySw.Restart();
		}

		public void DebugFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!_log.IsDebugEnabled) return;
			_log.DebugFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Restart();
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!_log.IsDebugEnabled) return;
			_log.DebugFormat(provider, format, args);
			_unitarySw.Restart();
		}

		public void Info(object message)
		{
			if (!_log.IsInfoEnabled) return;
			_log.Info(_scopeId + " - " + _unitarySw.Display() + " - " + message);
			_unitarySw.Restart();
		}

		public void Info(object message, Exception exception)
		{
			if (!_log.IsInfoEnabled) return;
			_log.Info(_scopeId + " - " + _unitarySw.Display() + " - " + message, exception);
			_unitarySw.Restart();
		}

		public void InfoFormat(string format, params object[] args)
		{
			if (!_log.IsInfoEnabled) return;
			_log.InfoFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, args);
			_unitarySw.Restart();
		}

		public void InfoFormat(string format, object arg0)
		{
			if (!_log.IsInfoEnabled) return;
			_log.InfoFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0);
			_unitarySw.Restart();
		}

		public void InfoFormat(string format, object arg0, object arg1)
		{
			if (!_log.IsInfoEnabled) return;
			_log.InfoFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1);
			_unitarySw.Restart();
		}

		public void InfoFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!_log.IsInfoEnabled) return;
			_log.InfoFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Restart();
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!_log.IsInfoEnabled) return;
			_log.InfoFormat(provider, format, args);
			_unitarySw.Restart();
		}

		public void Warn(object message)
		{
			if (!_log.IsWarnEnabled) return;
			_log.Warn(_scopeId + " - " + _unitarySw.Display() + " - " + message);
			_unitarySw.Restart();
		}

		public void Warn(object message, Exception exception)
		{
			if (!_log.IsWarnEnabled) return;
			_log.Warn(_scopeId + " - " + _unitarySw.Display() + " - " + message, exception);
			_unitarySw.Restart();
		}

		public void WarnFormat(string format, params object[] args)
		{
			if (!_log.IsWarnEnabled) return;
			_log.WarnFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, args);
			_unitarySw.Restart();
		}

		public void WarnFormat(string format, object arg0)
		{
			if (!_log.IsWarnEnabled) return;
			_log.WarnFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0);
			_unitarySw.Restart();
		}

		public void WarnFormat(string format, object arg0, object arg1)
		{
			if (!_log.IsWarnEnabled) return;
			_log.WarnFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1);
			_unitarySw.Restart();
		}

		public void WarnFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!_log.IsWarnEnabled) return;
			_log.WarnFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Restart();
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!_log.IsWarnEnabled) return;
			_log.WarnFormat(provider, format, args);
			_unitarySw.Restart();
		}

		public void Error(object message)
		{
			if (!_log.IsErrorEnabled) return;
			_log.Error(_scopeId + " - " + _unitarySw.Display() + " - " + message);
			_unitarySw.Restart();
		}

		public void Error(object message, Exception exception)
		{
			if (!_log.IsErrorEnabled) return;
			_log.Error(_scopeId + " - " + _unitarySw.Display() + " - " + message, exception);
			_unitarySw.Restart();
		}

		public void ErrorFormat(string format, params object[] args)
		{
			if (!_log.IsErrorEnabled) return;
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, args);
			_unitarySw.Restart();
		}

		public void ErrorFormat(string format, object arg0)
		{
			if (!_log.IsErrorEnabled) return;
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0);
			_unitarySw.Restart();
		}

		public void ErrorFormat(string format, object arg0, object arg1)
		{
			if (!_log.IsErrorEnabled) return;
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1);
			_unitarySw.Restart();
		}

		public void ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!_log.IsErrorEnabled) return;
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Restart();
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!_log.IsErrorEnabled) return;
			_log.ErrorFormat(provider, format, args);
			_unitarySw.Restart();
		}

		public void Fatal(object message)
		{
			if (!_log.IsFatalEnabled) return;
			_log.Fatal(_scopeId + " - " + _unitarySw.Display() + " - " + message);
			_unitarySw.Restart();
		}

		public void Fatal(object message, Exception exception)
		{
			if (!_log.IsFatalEnabled) return;
			_log.Fatal(_scopeId + " - " + _unitarySw.Display() + " - " + message, exception);
			_unitarySw.Restart();
		}

		public void FatalFormat(string format, params object[] args)
		{
			if (!_log.IsFatalEnabled) return;
			_log.FatalFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, args);
			_unitarySw.Restart();
		}

		public void FatalFormat(string format, object arg0)
		{
			if (!_log.IsFatalEnabled) return;
			_log.FatalFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0);
			_unitarySw.Restart();
		}

		public void FatalFormat(string format, object arg0, object arg1)
		{
			if (!_log.IsFatalEnabled) return;
			_log.FatalFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1);
			_unitarySw.Restart();
		}

		public void FatalFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!_log.IsFatalEnabled) return;
			_log.FatalFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Restart();
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!_log.IsFatalEnabled) return;
			_log.FatalFormat(provider, format, args);
			_unitarySw.Restart();
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
	}
}
