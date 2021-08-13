using System;
using System.Diagnostics;
using System.Globalization;
using System.Reactive.Disposables;
using System.Threading;
using NLog;

namespace Simple.Wpf.Template.Services
{
    public sealed class Duration : IDisposable
    {
        private readonly string _context;
        private readonly Logger _logger;
        private readonly Stopwatch _stopWatch;

        private Duration(Logger logger, string context)
        {
            _context = context;
            _stopWatch = new Stopwatch();
            _logger = logger;

            _stopWatch.Start();
        }

        public void Dispose()
        {
            _stopWatch.Stop();

            var message =
                $"{_context}, thread_id = {Thread.CurrentThread.ManagedThreadId}, duration = {_stopWatch.ElapsedMilliseconds}ms";

            Debug.WriteLine(message);
            _logger.Debug(message);
        }

        public static IDisposable Measure(Logger logger, string context, params object[] args)
        {
            if (!logger.IsDebugEnabled) return Disposable.Empty;

            if (args != null) context = string.Format(CultureInfo.InvariantCulture, context, args);

            return new Duration(logger, context);
        }
    }
}