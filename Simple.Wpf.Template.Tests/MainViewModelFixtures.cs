namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows.Input;
    using Autofac.Features.OwnedInstances;
    using Extensions;
    using Microsoft.Reactive.Testing;
    using Models;
    using Moq;
    using NUnit.Framework;
    using Services;
    using ViewModels;

    [TestFixture]
    public sealed class MainViewModelFixtures : BaseViewModelFixtures
    {
        private Mock<IOverlayService> _overlayService;
        private Mock<IMessageService> _messageService;
        private Mock<IDiagnosticsService> _diagnosticsService;
        private Mock<IGestureService> _gestureService;
        private Mock<IDiagnosticsViewModel> _diagnostics;

        [SetUp]
        public void Setup()
        {
            _diagnosticsService = new Mock<IDiagnosticsService>();
            _diagnosticsService.Setup(x => x.Cpu).Returns(Observable.Never<int>);
            _diagnosticsService.Setup(x => x.Memory).Returns(Observable.Never<Memory>);
            _diagnosticsService.Setup(x => x.Log).Returns(Observable.Never<string>);
            _overlayService = new Mock<IOverlayService>();
            _messageService = new Mock<IMessageService>();
            _gestureService = new Mock<IGestureService>();

            _diagnostics = new Mock<IDiagnosticsViewModel>();
        }

        [Test]
        public void requests_date_of_birth_message()
        {
            // ARRANGE
            var dateOfBirthViewModel = new Mock<IDateOfBirthViewModel>();
            var lifetime = Disposable.Create(() => { });
            var owned = new Owned<IDateOfBirthViewModel>(dateOfBirthViewModel.Object, lifetime);

            var viewModel = new MainViewModel(() => owned, _diagnostics.Object,  _overlayService.Object, _messageService.Object);

            _messageService.Setup(x => x.Post(It.Is<string>(y => y == "Diagnostics"),
                                              It.Is<CloseableViewModel>(y => y == dateOfBirthViewModel.Object),
                                              It.Is<IDisposable>(y => y == owned)))
                                              .Verifiable();

            // ACT
            viewModel.MessageCommand.Execute(null);

            // ASSERT
            _overlayService.Verify();
        }
    }
}
