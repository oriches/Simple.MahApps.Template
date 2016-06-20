namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reactive.Disposables;
    using System.Threading;
    using NLog;

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

            var message = string.Format("{0}, thread_id = {1}, duration = {2}ms",
                _context,
                Thread.CurrentThread.ManagedThreadId,
                _stopWatch.ElapsedMilliseconds);

            Debug.WriteLine(message);
            _logger.Debug(message);
        }

        public static IDisposable Measure(Logger logger, string context, params object[] args)
        {
            if (!logger.IsDebugEnabled)
            {
                return Disposable.Empty;
            }

            if (args != null)
            {
                context = string.Format(CultureInfo.InvariantCulture, context, args);
            }

            return new Duration(logger, context);
        }
    }
}