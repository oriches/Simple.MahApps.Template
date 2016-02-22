namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Windows.Data;
    using Collections;
    using Commands;
    using Extensions;
    using Models;
    using Rest;
    using Services;

    public sealed class MainViewModel : BaseViewModel, IMainViewModel
    {
        private static readonly Resource[] EmptyResources = new Resource[0];

        private readonly IRestClient _restClient;
        private readonly ISchedulerService _schedulerService;
        private readonly RangeObservableCollection<Resource> _resources;
        private readonly ListCollectionView _collectionView;

        private string _serverStatus;
        private bool _isServerOnline;

        public MainViewModel(IDiagnosticsViewModel diagnosticsViewModel, IMessageService messageService, IRestClient restClient, ISchedulerService schedulerService)
        {
            _restClient = restClient;
            _schedulerService = schedulerService;
            Diagnostics = diagnosticsViewModel;

            _resources = new RangeObservableCollection<Resource>();
            _collectionView = new ListCollectionView(_resources);

            RefreshCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            ObserveServerHeartbeat()
                .ObserveOn(_schedulerService.Dispatcher)
                .Do(x =>
                {
                    ServerStatus = x.IsOnline
                        ? $"Online ({x.Timestamp})"
                        : (x.HasTimedOut ? "Offline (Timed out)" : "Offline (Error)");

                    IsServerOnline = x.IsOnline;
                })
                .DistinctUntilChanged(x => x.IsOnline)
                .SelectMany(x => ObserveRefresh(), (x, y) => x)
                .SelectMany(ObserveResources, (x, y) => new { Status = x, Resources = y })
                .ObserveOn(_schedulerService.Dispatcher)
                .Subscribe(x =>
                {
                    _resources.Clear();
                    _resources.AddRange(x.Resources);
                })
                .DisposeWith(this);
        }

        public IEnumerable Resources => _collectionView;

        public string ServerHeartbeatUrl => Constants.Server.Hearbeat.Url.ToString();

        public bool IsServerOnline
        {
            get { return _isServerOnline; }
            set
            {
                _isServerOnline = value;
                OnPropertyChanged(() => IsServerOnline);
            }
        }

        public string ServerStatus
        {
            get { return _serverStatus; }
            set { SetPropertyAndNotify(ref _serverStatus, value, () => ServerStatus); }
        }

        public ReactiveCommand<object> RefreshCommand { get; }

        public IDiagnosticsViewModel Diagnostics { get; }

        private IObservable<Status> ObserveServerHeartbeat()
        {
            return Observable.Timer(DateTimeOffset.Now, Constants.Server.Hearbeat.Interval, _schedulerService.TaskPool)
                .SelectMany(x => _restClient.GetAsync<Heartbeat>(Constants.Server.Hearbeat.Url).ToObservable())
                .Select(x => new Status(x.Resource.Timestamp))
                .Timeout(Constants.Server.Hearbeat.Timeout, _schedulerService.TaskPool)
                .Catch<Status, Exception>(x => ObserveServerHeartbeat().StartWith(new Status(x)));
        }

        private IObservable<Resource[]> ObserveResources(Status status)
        {
            return status.IsOnline
                ? _restClient.GetAsync<IEnumerable<Resource>>(Constants.Server.ResourcesUrl).ToObservable()
                    .Select(x => x.Resource.ToArray())
                : Observable.Return(EmptyResources);
        }

        private IObservable<Unit> ObserveRefresh()
        {
            return RefreshCommand.ActivateGestures()
                .AsUnit()
                .StartWith(Unit.Default);
        } 
    }
}
