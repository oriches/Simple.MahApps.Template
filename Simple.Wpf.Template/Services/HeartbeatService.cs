namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Extensions;

    public sealed class HeartbeatService : BaseService, IHeartbeatService
    {
        private readonly IConnectableObservable<Unit> _listen;

        public HeartbeatService(ISchedulerService schedulerService) : this(Constants.Heartbeat, schedulerService)
        {
        }

        public HeartbeatService(TimeSpan interval, ISchedulerService schedulerService)
        {
            using (Duration.Measure(Logger, "Constructor - " + GetType().Name))
            {
                _listen = Observable.Interval(interval, schedulerService.TaskPool)
                    .Select(x => Unit.Default)
                    .Publish();

                _listen.Connect()
                    .DisposeWith(this);
            }
        }
        
        public IObservable<Unit> Listen => _listen;
    }
}
