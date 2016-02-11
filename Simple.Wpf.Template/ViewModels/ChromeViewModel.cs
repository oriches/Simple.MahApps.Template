namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive.Disposables;
    using System.Windows.Input;
    using Commands;
    using Extensions;
    using NLog;
    using Services;

    public sealed class ChromeViewModel : BaseViewModel, IChromeViewModel
    {
        private readonly IDisposable _disposable;

        private OverlayViewModel _overlay;
        
        public ChromeViewModel(IMainViewModel main, IOverlayService overlayService)
        {
            Main = main;

            overlayService.Show
                .Subscribe(UpdateOverlay)
                .DisposeWith(this);

            CloseOverlayCommand = new RelayCommand(ClearOverlay);

            _disposable = new CompositeDisposable(new []
            {
                Disposable.Create(() =>
                {
                    CloseOverlayCommand = null;
                })
            });
        }
        
        public override void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                base.Dispose();
                _disposable.Dispose();
            }
        }
        
        public IMainViewModel Main { get; private set; }
        
        public ICommand CloseOverlayCommand { get; private set; }

        public bool HasOverlay => _overlay != null;

        public string OverlayHeader => _overlay != null ? _overlay.Header : string.Empty;

        public BaseViewModel Overlay => _overlay?.ViewModel;

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
