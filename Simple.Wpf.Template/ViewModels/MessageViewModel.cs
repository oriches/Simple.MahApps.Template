namespace Simple.Wpf.Template.ViewModels
{
    using System;

    public sealed class MessageViewModel : OverlayViewModel<CloseableViewModel>
    {
        public MessageViewModel(string header, CloseableViewModel viewModel, IDisposable lifetime)
            : base(header, viewModel, lifetime)
        {
        }
    }
}