namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive.Subjects;
    using Extensions;
    using Models;
    using ViewModels;

    public sealed class MessageService : DisposableObject, IMessageService
    {
        private readonly Subject<MessageViewModel> _show;

        public MessageService()
        {
            using (Duration.Measure(Logger, "Constructor - " + GetType().Name))
            {
                _show = new Subject<MessageViewModel>()
                    .DisposeWith(this);
            }
        }

        public void Post(string header, ICloseableViewModel viewModel, IDisposable lifetime)
        {
            _show.OnNext(new MessageViewModel(header, viewModel, lifetime));
        }

        public IObservable<MessageViewModel> Show => _show;
    }
}