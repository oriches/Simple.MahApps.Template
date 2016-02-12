namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive.Subjects;
    using Extensions;
    using ViewModels;

    public sealed class OverlayService : BaseService, IOverlayService
    {
        private readonly Subject<OverlayViewModel> _show;
        
        public OverlayService()
        {
            _show = new Subject<OverlayViewModel>()
                .DisposeWith(this);
        }
        
        public void Post(string header, BaseViewModel viewModel, IDisposable lifetime)
        {
            _show.OnNext(new OverlayViewModel(header, viewModel, lifetime));
        }

        public IObservable<OverlayViewModel> Show => _show;
    }
}