namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive.Linq;
    using Autofac.Features.OwnedInstances;
    using Commands;
    using Extensions;
    using Services;

    public sealed class MainViewModel : BaseViewModel, IMainViewModel
    {
        private string _birthday;

        public MainViewModel(Func<Owned<IDateOfBirthViewModel>> dateOfBirthFactory,
            IDiagnosticsViewModel diagnosticsViewModel,
            IOverlayService overlayService,
            IMessageService messageService)
        {
            Diagnostics = diagnosticsViewModel;

            DobCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            DobCommand
                .ActivateGestures()
                .Subscribe(x =>
                {
                    var owned = dateOfBirthFactory();

                    owned.Value
                        .Confirmed
                        .Take(1)
                        .Subscribe(y => UpdateBirthday(owned.Value))
                        .DisposeWith(this);

                    messageService.Post("Date of Birth", owned.Value, owned);
                })
                .DisposeWith(this);
        }

        private void UpdateBirthday(IDateOfBirthViewModel viewModel)
        {
            Birthday = new DateTime(viewModel.Year.Value, viewModel.Month.Value, viewModel.Day.Value)
                .ToLongDateString();
        }

        public string Birthday
        {
            get { return _birthday; }
            set { SetPropertyAndNotify(ref _birthday, value, () => Birthday); }
        }

        public ReactiveCommand<object> DobCommand { get; }

        public IDiagnosticsViewModel Diagnostics { get; }
    }
}
