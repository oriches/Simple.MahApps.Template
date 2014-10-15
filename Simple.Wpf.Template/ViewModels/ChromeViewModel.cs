namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive.Disposables;
    using System.Windows.Input;
    using Commands;
    using NLog;
    using Services;

    public sealed class ChromeViewModel : BaseViewModel, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;

        private OverlayViewModel _overlay;
        
        public ChromeViewModel(MainViewModel main, IOverlayService overlayService)
        {
            Main = main;

            CloseOverlayCommand = new RelayCommand(ClearOverlay);

            _disposable = new CompositeDisposable(new []
            {
                overlayService.Show.Subscribe(UpdateOverlay),
                
                Disposable.Create(() =>
                {
                    CloseOverlayCommand = null;
                })
            });
        }
        
        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        public string Title { get { return "Main"; } }

        public MainViewModel Main { get; private set; }
        
        public ICommand CloseOverlayCommand { get; private set; }

        public bool HasOverlay { get { return _overlay != null; } }

        public string OverlayHeader { get { return _overlay != null ? _overlay.Header : string.Empty; } }

        public BaseViewModel Overlay { get { return _overlay != null ? _overlay.ViewModel : null; } }

        private void ClearOverlay()
        {
            using(_overlay.Lifetime)
            {
                UpdateOverlayImpl(null);
            }
        }

        private void UpdateOverlay(OverlayViewModel overlay)
        {
            using (SuspendNotifications())
            {
                if (_overlay != null)
                {
                    ClearOverlay();
                }

                UpdateOverlayImpl(overlay);
            }
        }

        private void UpdateOverlayImpl(OverlayViewModel overlay)
        {
            _overlay = overlay;

            OnPropertyChanged(() => HasOverlay);
            OnPropertyChanged(() => Overlay);
            OnPropertyChanged(() => OverlayHeader);
        }
    }
}
