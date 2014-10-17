namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive.Disposables;
    using System.Windows.Input;
    using Autofac.Features.OwnedInstances;
    using Commands;
    using NLog;
    using Services;

    public sealed class MainViewModel : BaseViewModel, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;

        public MainViewModel(Func<Owned<DiagnosticsViewModel>> diagnosticsFactory,
            Func<Owned<DateOfBirthViewModel>> dateOfBirthFactory,
            IOverlayService overlayService,
            IMessageService messageService)
        {
            DiagnosticsCommand = new RelayCommand(() =>
            {
                var owned = diagnosticsFactory();
                overlayService.Post("Diagnostics", owned.Value, owned);
            });
            MessageCommand = new RelayCommand(() =>
            {
                var owned = dateOfBirthFactory();
                messageService.Post("Date of Birth", owned.Value, owned);
            });
            
            _disposable = Disposable.Create(() =>
            {
                DiagnosticsCommand = null;
                MessageCommand = null;
            });
        }

        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        public ICommand DiagnosticsCommand { get; private set; }

        public ICommand MessageCommand { get; private set; }
    }
}
