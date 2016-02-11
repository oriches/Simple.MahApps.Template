namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive.Disposables;
    using System.Windows.Input;
    using Autofac.Features.OwnedInstances;
    using Commands;
    using Services;

    public sealed class MainViewModel : BaseViewModel, IMainViewModel
    {
        private readonly IDisposable _disposable;

        public MainViewModel(Func<Owned<IDateOfBirthViewModel>> dateOfBirthFactory,
            IDiagnosticsViewModel diagnosticsViewModel,
            IOverlayService overlayService,
            IMessageService messageService)
        {
            Diagnostics = diagnosticsViewModel;

            MessageCommand = new RelayCommand(() =>
            {
                var owned = dateOfBirthFactory();
                messageService.Post("Date of Birth", owned.Value, owned);
            });
            
            _disposable = Disposable.Create(() =>
            {
                MessageCommand = null;
            });
        }

        public override void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        public ICommand MessageCommand { get; private set; }

        public IDiagnosticsViewModel Diagnostics { get; private set; }
    }
}
