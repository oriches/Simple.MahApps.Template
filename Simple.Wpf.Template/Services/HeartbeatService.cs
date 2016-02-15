namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Extensions;
    using NLog;

    public sealed class HeartbeatService : BaseService
    {
        private readonly IConnectableObservable<Unit> _listen;

        public HeartbeatService() : this(Constants.Heartbeat)
        {
        }

        public HeartbeatService(TimeSpan interval)
        {
            _listen = Observable.Interval(interval, TaskPoolScheduler.Default)
                .Select(x => Unit.Default)
                .Publish();

            _listen.Connect()
                .DisposeWith(this);
        }
        
        public IObservable<Unit> Listen => _listen;
    }
}
