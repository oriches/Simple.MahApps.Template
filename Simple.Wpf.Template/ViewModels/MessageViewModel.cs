using System;

namespace Simple.Wpf.Template.ViewModels
{
    public sealed class MessageViewModel : OverlayViewModel<ICloseableViewModel>
    {
        public MessageViewModel(string header, ICloseableViewModel viewModel, IDisposable lifetime)
            : base(header, viewModel, lifetime)
        {
        }
    }
}