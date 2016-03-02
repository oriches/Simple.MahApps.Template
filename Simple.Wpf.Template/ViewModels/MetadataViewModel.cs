namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using Commands;
    using Extensions;
    using Models;
    using Rest;
    using Services;

    public sealed class MetadataViewModel : BaseViewModel, IMetadataViewModel
    {
        private readonly Func<Exception, IExceptionViewModel> _exceptionFactory;
        private readonly IRestClient _restClient;
        private readonly IMessageService _messageService;
        private readonly ISchedulerService _schedulerService;

        public MetadataViewModel(Metadata metadata,
            Func<Metadata, IModifyResourceViewModel> modifyResourceFactory,
            Func<Exception, IExceptionViewModel> exceptionFactory,
            IRestClient restClient,
            IMessageService messageService,
            ISchedulerService schedulerService)
        {
            _exceptionFactory = exceptionFactory;
            _restClient = restClient;
            _messageService = messageService;
            _schedulerService = schedulerService;
            Metadata = metadata;

            ModifyCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            DeleteCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            ModifyCommand.ActivateGestures()
                .Select(x => modifyResourceFactory(metadata))
                .Subscribe(x => messageService.Post("Modify Resource", x))
                .DisposeWith(this);

            Deleted = ObserveDelete();
        }

        public Metadata Metadata { get; }

        public Uri Url => Metadata.Url;

        public bool Editable => !Metadata.Immutable;

        public ReactiveCommand<object> ModifyCommand { get; }

        public ReactiveCommand<object> DeleteCommand { get; }

        public IObservable<Unit> Deleted { get; }

        private IObservable<Unit> ObserveDelete()
        {
            return DeleteCommand.ActivateGestures()
                .SelectMany(x => _restClient.DeleteAsync(Url).ToObservable().Take(1), (x, y) => y)
                .AsUnit()
                .Catch<Unit, Exception>(x =>
                {
                    _schedulerService.Dispatcher.Schedule(() => _messageService.Post(Constants.UI.ExceptionTitle, _exceptionFactory(x)));

                    return ObserveDelete();
                });
        } 
    }
}