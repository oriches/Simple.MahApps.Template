namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using Autofac.Features.OwnedInstances;
    using Commands;
    using Extensions;
    using Services;

    public sealed class MainViewModel : BaseViewModel, IMainViewModel
    {
        public MainViewModel(Func<Owned<IDateOfBirthViewModel>> dateOfBirthFactory,
            IDiagnosticsViewModel diagnosticsViewModel,
            IOverlayService overlayService,
            IMessageService messageService)
        {
            Diagnostics = diagnosticsViewModel;

            MessageCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            MessageCommand
                .ActivateGestures()
                .Subscribe(x =>
                {
                    var owned = dateOfBirthFactory();
                    messageService.Post("Date of Birth", owned.Value, owned);
                })
                .DisposeWith(this);
        }

        public ReactiveCommand<object> MessageCommand { get; }

        public IDiagnosticsViewModel Diagnostics { get; }
    }
}
