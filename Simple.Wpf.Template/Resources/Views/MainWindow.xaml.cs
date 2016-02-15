using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.Resources.Views
{
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
                    Content = x.ViewModel
                };

                _disposable.Disposable = x.ViewModel.Closed
                    .Select(y => this.HideMetroDialogAsync(dialog).ToObservable())
                    .Subscribe(z =>
                    {
                        using (x.Lifetime)
                        {
                            dialog.Content = null;
                        }
                    });

                this.ShowMetroDialogAsync(dialog);
            });
        }
    }
}
