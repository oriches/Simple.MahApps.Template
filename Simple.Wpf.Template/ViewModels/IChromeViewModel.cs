using Simple.Wpf.Template.Commands;

namespace Simple.Wpf.Template.ViewModels
{
    public interface IChromeViewModel : IViewModel
    {
        IMainViewModel Main { get; }
        ReactiveCommand<object> CloseOverlayCommand { get; }
        bool HasOverlay { get; }
        string OverlayHeader { get; }
        BaseViewModel Overlay { get; }
    }
}