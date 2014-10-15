namespace Simple.Wpf.Template.Views
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using MahApps.Metro.Controls.Dialogs;
    using Services;

    public partial class MainWindow
    {
        private readonly SerialDisposable _disposable;

        public MainWindow(IMessageService messageService)
        {
            InitializeComponent();

            _disposable = new SerialDisposable();

            messageService.Show.Subscribe(x =>
            {
                var dialog = new MessageDialog
                {
                    Title = x.Header,
                    DialogBody = x.ViewModel,
                };

                _disposable.Disposable = x.ViewModel.CloseRequested
                    .Select(y => this.HideMetroDialogAsync(dialog).ToObservable())
                    .Subscribe(z =>
                    {
                        using (x.Lifetime)
                        {
                            dialog.DialogBody = null;
                        }
                    });

                this.ShowMetroDialogAsync(dialog);
            });
        }
    }
}
