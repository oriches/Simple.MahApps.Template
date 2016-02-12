namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive.Subjects;
    using Extensions;
    using ViewModels;

    public sealed class MessageService : BaseService, IMessageService
    {
        private readonly Subject<MessageViewModel> _show;

        public MessageService()
        {
            _show = new Subject<MessageViewModel>()
                .DisposeWith(this);
        }

        public void Post(string header, ICloseableViewModel viewModel, IDisposable lifetime)
        {
            _show.OnNext(new MessageViewModel(header, viewModel, lifetime));
        }

        public IObservable<MessageViewModel> Show => _show;
    }
}