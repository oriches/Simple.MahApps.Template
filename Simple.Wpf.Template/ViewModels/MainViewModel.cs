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
        private static readonly Metadata[] EmptyMetadatas = new Metadata[0];

        private readonly Func<IEnumerable<Metadata>, IAddResourceViewModel> _addResourceFactory;
        private readonly ListCollectionView _collectionView;
        private readonly IMessageService _messageService;
        private readonly RangeObservableCollection<Metadata> _metadata;
        private readonly IRestClient _restClient;
        private readonly ISchedulerService _schedulerService;
        private bool _isServerOnline;

        private string _serverStatus;

        public MainViewModel(Func<IEnumerable<Metadata>, IAddResourceViewModel> addResourceFactory,
            IDiagnosticsViewModel diagnosticsViewModel,
            IMessageService messageService,
            IRestClient restClient,
            ISchedulerService schedulerService)
        {
            _addResourceFactory = addResourceFactory;
            _messageService = messageService;
            _restClient = restClient;
            _schedulerService = schedulerService;

            Diagnostics = diagnosticsViewModel;

            _metadata = new RangeObservableCollection<Metadata>();
            _collectionView = new ListCollectionView(_metadata);

            RefreshCommand = ReactiveCommand.Create(
                this.ObservePropertyChanged(x => IsServerOnline).Select(x => IsServerOnline))
                .DisposeWith(this);

            AddCommand = ReactiveCommand.Create(
                this.ObservePropertyChanged(x => IsServerOnline).Select(x => IsServerOnline))
                .DisposeWith(this);

            AddCommand.ActivateGestures()
                .SelectMany(x => ObserveResourceAdded(), (x, y) => y)
                .Subscribe(x => RefreshCommand.Execute(null))
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
                .Where(x => x.IsOnline)
                .SelectMany(x => ObserveRefresh(), (x, y) => x)
                .SelectMany(x => ObserveMetadata(), (x, y) => y)
                .ObserveOn(_schedulerService.Dispatcher)
                .Subscribe(x =>
                           {
                               _metadata.Clear();
                               _metadata.AddRange(x);
                           })
                .DisposeWith(this);
        }

        public IEnumerable Metadata => _collectionView;

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

        public ReactiveCommand<object> AddCommand { get; }

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

        private IObservable<Metadata[]> ObserveMetadata()
        {
            return _restClient.GetAsync<IEnumerable<Metadata>>(Constants.Server.MetadataUrl)
                .ToObservable()
                .Select(x => x.Resource.ToArray())
                .Catch<Metadata[], Exception>(x => Observable.Return(EmptyMetadatas))
                .Take(1);
        }

        private IObservable<Unit> ObserveRefresh()
        {
            return RefreshCommand.ActivateGestures()
                .AsUnit()
                .StartWith(Unit.Default);
        }

        private IObservable<Unit> ObserveResourceAdded()
        {
            var viewModel = _addResourceFactory(_metadata);
            _messageService.Post("Add Resource", viewModel);

            return viewModel.Confirmed;
        }
    }
}