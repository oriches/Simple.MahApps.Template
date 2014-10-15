namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Subjects;
    using NLog;
    using ViewModels;

    public sealed class MessageService : IMessageService, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;
        private readonly Subject<MessageViewModel> _show;

        public MessageService()
        {
            _show = new Subject<MessageViewModel>();

            _disposable = Disposable.Create(() =>
            {
                _show.OnCompleted();
                _show.Dispose();
            });
        }

        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        public void Post(string header, CloseableViewModel viewModel, IDisposable lifetime)
        {
            _show.OnNext(new MessageViewModel(header, viewModel, lifetime));
        }

        public IObservable<MessageViewModel> Show { get { return _show; } }
    }
}