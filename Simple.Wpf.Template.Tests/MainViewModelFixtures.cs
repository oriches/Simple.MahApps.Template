namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Data.Common;
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
    public sealed class MainViewModelFixtures
    {
        private Mock<IOverlayService> _overlayService;
        private Mock<IMessageService> _messageService;
        private Mock<IDiagnosticsService> _diagnosticsService;
        private TestScheduler _testScheduler;
        private MockSchedulerService _schedulerService;
        private Mock<IGestureService> _gestureService;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);

            _diagnosticsService = new Mock<IDiagnosticsService>();
            _diagnosticsService.Setup(x => x.Fps).Returns(Observable.Never<int>);
            _diagnosticsService.Setup(x => x.Cpu).Returns(Observable.Never<int>);
            _diagnosticsService.Setup(x => x.Memory).Returns(Observable.Never<Memory>);
            _diagnosticsService.Setup(x => x.Log).Returns(Observable.Never<string>);
            _overlayService = new Mock<IOverlayService>();
            _messageService = new Mock<IMessageService>();
            _gestureService = new Mock<IGestureService>();
        }

        [Test]
        public void requests_diagnostics_overlay()
        {
            // ARRANGE
            var diagnosticsViewModel = new DiagnosticsViewModel(_diagnosticsService.Object, _schedulerService);
            var lifetime = Disposable.Create(() => { });
            var owned = new Owned<DiagnosticsViewModel>(diagnosticsViewModel, lifetime);
            
            var viewModel = new MainViewModel(() => owned, null, _overlayService.Object, _messageService.Object);

            _overlayService.Setup(x => x.Post(It.Is<string>(y => y == "Diagnostics"),
                                              It.Is<BaseViewModel>(y => y == diagnosticsViewModel),
                                              It.Is<IDisposable>(y => y == owned)))
                                              .Verifiable();

            // ACT
            viewModel.DiagnosticsCommand.Execute(null);

            // ASSERT
            _overlayService.Verify();
        }

        [Test]
        public void requests_date_of_birth_message()
        {
            // ARRANGE
            var dateOfBirthViewModel = new DateOfBirthViewModel(_gestureService.Object);
            var lifetime = Disposable.Create(() => { });
            var owned = new Owned<DateOfBirthViewModel>(dateOfBirthViewModel, lifetime);

            var viewModel = new MainViewModel(null, () => owned, _overlayService.Object, _messageService.Object);

            _messageService.Setup(x => x.Post(It.Is<string>(y => y == "Diagnostics"),
                                              It.Is<CloseableViewModel>(y => y == dateOfBirthViewModel),
                                              It.Is<IDisposable>(y => y == owned)))
                                              .Verifiable();

            // ACT
            viewModel.MessageCommand.Execute(null);

            // ASSERT
            _overlayService.Verify();
        }

        [Test]
        public void disposing_clears_commands()
        {
            // ARRANGE
            var viewModel = new MainViewModel(null, null, _overlayService.Object, _messageService.Object);

            // ACT
            viewModel.Dispose();

            // ASSERT
            var commandProperties = TestHelper.PropertiesImplementingInterface<ICommand>(viewModel);
            commandProperties.ForEach(x => Assert.That(x.GetValue(viewModel, null), Is.Null));
        }
    }
}
