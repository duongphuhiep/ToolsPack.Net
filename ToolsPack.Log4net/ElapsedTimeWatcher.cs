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
	/// Micro benchmark a block of code
	/// 
	///            using (var etw = new ElapsedTimeWatcher(Log, "checkIntraday"))
	///            {
	///                Thread.Sleep(100);
	///                etw.Debug("step 1");
	///                Thread.Sleep(200);
	///                etw.Debug("step 2");
	///                Thread.Sleep(300);
	///                etw.Info("final step)");
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
			_log.Debug(_scopeId + " - " + _unitarySw.Display() + " - " + message);
			_unitarySw.Restart();
		}

		public void Debug(object message, Exception exception)
		{
			_log.Debug(_scopeId + " - " + _unitarySw.Display() + " - " + message, exception);
			_unitarySw.Restart();
		}

		public void DebugFormat(string format, params object[] args)
		{
			_log.DebugFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, args);
			_unitarySw.Restart();
		}

		public void DebugFormat(string format, object arg0)
		{
			_log.DebugFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0);
			_unitarySw.Restart();
		}

		public void DebugFormat(string format, object arg0, object arg1)
		{
			_log.DebugFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1);
			_unitarySw.Restart();
		}

		public void DebugFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.DebugFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Restart();
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.DebugFormat(provider, format, args);
			_unitarySw.Restart();
		}

		public void Info(object message)
		{
			_log.Info(_scopeId + " - " + _unitarySw.Display() + " - " + message);
			_unitarySw.Restart();
		}

		public void Info(object message, Exception exception)
		{
			_log.Info(_scopeId + " - " + _unitarySw.Display() + " - " + message, exception);
			_unitarySw.Restart();
		}

		public void InfoFormat(string format, params object[] args)
		{
			_log.InfoFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, args);
			_unitarySw.Restart();
		}

		public void InfoFormat(string format, object arg0)
		{
			_log.InfoFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0);
			_unitarySw.Restart();
		}

		public void InfoFormat(string format, object arg0, object arg1)
		{
			_log.InfoFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1);
			_unitarySw.Restart();
		}

		public void InfoFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.InfoFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Restart();
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.InfoFormat(provider, format, args);
			_unitarySw.Restart();
		}

		public void Warn(object message)
		{
			_log.Warn(_scopeId + " - " + _unitarySw.Display() + " - " + message);
			_unitarySw.Restart();
		}

		public void Warn(object message, Exception exception)
		{
			_log.Warn(_scopeId + " - " + _unitarySw.Display() + " - " + message, exception);
			_unitarySw.Restart();
		}

		public void WarnFormat(string format, params object[] args)
		{
			_log.WarnFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, args);
			_unitarySw.Restart();
		}

		public void WarnFormat(string format, object arg0)
		{
			_log.WarnFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0);
			_unitarySw.Restart();
		}

		public void WarnFormat(string format, object arg0, object arg1)
		{
			_log.WarnFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1);
			_unitarySw.Restart();
		}

		public void WarnFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.WarnFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Restart();
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.WarnFormat(provider, format, args);
			_unitarySw.Restart();
		}

		public void Error(object message)
		{
			_log.Error(_scopeId + " - " + _unitarySw.Display() + " - " + message);
			_unitarySw.Restart();
		}

		public void Error(object message, Exception exception)
		{
			_log.Error(_scopeId + " - " + _unitarySw.Display() + " - " + message, exception);
			_unitarySw.Restart();
		}

		public void ErrorFormat(string format, params object[] args)
		{
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, args);
			_unitarySw.Restart();
		}

		public void ErrorFormat(string format, object arg0)
		{
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0);
			_unitarySw.Restart();
		}

		public void ErrorFormat(string format, object arg0, object arg1)
		{
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1);
			_unitarySw.Restart();
		}

		public void ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.ErrorFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Restart();
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.ErrorFormat(provider, format, args);
			_unitarySw.Restart();
		}

		public void Fatal(object message)
		{
			_log.Fatal(_scopeId + " - " + _unitarySw.Display() + " - " + message);
			_unitarySw.Restart();
		}

		public void Fatal(object message, Exception exception)
		{
			_log.Fatal(_scopeId + " - " + _unitarySw.Display() + " - " + message, exception);
			_unitarySw.Restart();
		}

		public void FatalFormat(string format, params object[] args)
		{
			_log.FatalFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, args);
			_unitarySw.Restart();
		}

		public void FatalFormat(string format, object arg0)
		{
			_log.FatalFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0);
			_unitarySw.Restart();
		}

		public void FatalFormat(string format, object arg0, object arg1)
		{
			_log.FatalFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1);
			_unitarySw.Restart();
		}

		public void FatalFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.FatalFormat(_scopeId + " - " + _unitarySw.Display() + " - " + format, arg0, arg1, arg2);
			_unitarySw.Restart();
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
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
