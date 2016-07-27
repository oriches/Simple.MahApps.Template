namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Disposables;
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
        private readonly Func<Metadata, IMetadataViewModel> _metadataFactory;

        private readonly ListCollectionView _collectionView;
        private readonly IMessageService _messageService;
        private readonly RangeObservableCollection<IMetadataViewModel> _metadata;
        private readonly IRestClient _restClient;
        private readonly ISchedulerService _schedulerService;

        private bool _isServerOnline;
        private string _serverStatus;

        public MainViewModel(Func<IEnumerable<Metadata>, IAddResourceViewModel> addResourceFactory,
            Func<Metadata, IMetadataViewModel> metadataFactory,
            IDiagnosticsViewModel diagnosticsViewModel,
            IMessageService messageService,
            IRestClient restClient,
            ISchedulerService schedulerService)
        {
            _addResourceFactory = addResourceFactory;
            _metadataFactory = metadataFactory;
            _messageService = messageService;
            _restClient = restClient;
            _schedulerService = schedulerService;

            Diagnostics = diagnosticsViewModel;

            _metadata = new RangeObservableCollection<IMetadataViewModel>();
            _collectionView = new ListCollectionView(_metadata);

            RefreshCommand = ReactiveCommand.Create(this.ObservePropertyChanged(x => IsServerOnline).Select(x => IsServerOnline))
                .DisposeWith(this);

            AddCommand = ReactiveCommand.Create(this.ObservePropertyChanged(x => IsServerOnline).Select(x => IsServerOnline))
                .DisposeWith(this);
            
            ObserveServerHeartbeat()
                .ObserveOn(_schedulerService.Dispatcher)
                .Do(UpdateStatus)
                .DistinctUntilChanged(x => x.IsOnline)
                .Where(x => x.IsOnline)
                .AsUnit()
                .Merge(ObserveRefresh(), _schedulerService.Dispatcher)
                .ActivateGestures()
                .SelectMany(x => ObserveMetadata(), (x, y) => y)
                .ObserveOn(_schedulerService.Dispatcher)
                .Subscribe(ProcessMetadata)
                .DisposeWith(this);

            ObserveResourceAdded()
                .Merge(ObserveResourceDeleted(), _schedulerService.Dispatcher)
                .ObserveOn(schedulerService.Dispatcher)
                .Subscribe(x => RefreshCommand.Execute(null))
                .DisposeWith(this);

            Disposable.Create(DisposeOfMetadata)
                .DisposeWith(this);
        }

        public IEnumerable Metadata => _collectionView;

        public string ServerHeartbeatUrl => Constants.Server.Hearbeat.Url.ToString();

        public bool IsServerOnline
        {
            get { return _isServerOnline; }
            private set
            {
                _isServerOnline = value;
                OnPropertyChanged(() => IsServerOnline);
            }
        }

        public string ServerStatus
        {
            get { return _serverStatus; }
            private set { SetPropertyAndNotify(ref _serverStatus, value, () => ServerStatus); }
        }

        public ReactiveCommand<object> AddCommand { get; }

        public ReactiveCommand<object> RefreshCommand { get; }

        public IDiagnosticsViewModel Diagnostics { get; }

        private IObservable<Status> ObserveServerHeartbeat()
        {
            return Observable.Timer(Constants.Server.Hearbeat.Interval, _schedulerService.TaskPool)
                .SelectMany(x => _restClient.GetAsync<Heartbeat>(Constants.Server.Hearbeat.Url).ToObservable())
                .Select(x => new Status(x.Resource.Timestamp))
                .Timeout(Constants.Server.Hearbeat.Timeout, _schedulerService.TaskPool)
                .Catch<Status, Exception>(x => ObserveServerHeartbeat().StartWith(new Status(x)));
        }

        private void UpdateStatus(Status status)
        {
            ServerStatus = status.IsOnline
                ? $"Online ({status.Timestamp})"
                : (status.HasTimedOut ? "Offline (Timed out)" : "Offline (Error)");

            IsServerOnline = status.IsOnline;
        }

        private void DisposeOfMetadata()
        {
            var items = _metadata.ToArray();

            _metadata.Clear();

            items.ForEach(x => x.Dispose());
        }

        private void ProcessMetadata(IEnumerable<IMetadataViewModel> viewModels)
        {
            DisposeOfMetadata();

            _metadata.AddRange(viewModels);
        }

        private IObservable<IEnumerable<IMetadataViewModel>> ObserveMetadata()
        {
            return _restClient.GetAsync<IEnumerable<Metadata>>(Constants.Server.MetadataUrl)
                .ToObservable()
                .Take(1)
                .Select(x => x.Resource.ToArray())
                .Catch<Metadata[], Exception>(x => Observable.Return(EmptyMetadatas))
                .Select(x => x.Select(y => _metadataFactory(y)));
        }

        private IObservable<Unit> ObserveRefresh()
        {
            return RefreshCommand.AsUnit();
        }

        private IObservable<Unit> ObserveResourceAdded()
        {
            return AddCommand.ActivateGestures()
                .SelectMany(x =>
                {
                    var viewModel = _addResourceFactory(_metadata.Select(y => y.Metadata));
                    _messageService.Post("Add Resource", viewModel);

                    return viewModel.Added;
                }, (x, y) => y);
        }

        private IObservable<Unit> ObserveResourceDeleted()
        {
            return _metadata.ObserveCollectionChanged()
                .AsUnit()
                .SelectMany(x => _metadata.Select(y => y.Deleted), (x, y) => y)
                .Merge();
        }
    }
}