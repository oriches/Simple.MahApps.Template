namespace Simple.Wpf.Template.Tests
{
    using System.Reactive.Disposables;
    using System.Reactive.Subjects;
    using System.Windows.Input;
    using Extensions;
    using Moq;
    using NUnit.Framework;
    using Services;
    using ViewModels;

    [TestFixture]
    public sealed class ChromeViewModelFixtures : BaseViewModelFixtures
    {
        private Mock<IOverlayService> _overlayService;
        private Subject<OverlayViewModel> _show;

        private MainViewModel _mainViewModel;
        private Mock<IDiagnosticsViewModel> _diagnostics;

        [SetUp]
        public void Setup()
        {
            _overlayService = new Mock<IOverlayService>();

            _show = new Subject<OverlayViewModel>();
            _overlayService.Setup(x => x.Show).Returns(_show);

            _diagnostics = new Mock<IDiagnosticsViewModel>();
            
            var messageService = new Mock<IMessageService>();
            _mainViewModel = new MainViewModel(() => null, _diagnostics.Object, _overlayService.Object, messageService.Object);
        }

        [Test]
        public void no_overlay_when_created()
        {
            // ARRANGE
            // ACT
            var viewModel = new ChromeViewModel(_mainViewModel, _overlayService.Object);
            
            // ASSERT
            Assert.That(viewModel.HasOverlay, Is.False);
            Assert.That(viewModel.OverlayHeader, Is.Empty);
            Assert.That(viewModel.Overlay, Is.Null);
        }

        [Test]
        public void shows_overlay()
        {
            // ARRANGE
            var viewModel = new ChromeViewModel(_mainViewModel, _overlayService.Object);
            var contentViewModel = new Mock<BaseViewModel>();
            var overlayViewModel = new OverlayViewModel("header 1", contentViewModel.Object, Disposable.Empty);
            
            // ACT
            _show.OnNext(overlayViewModel);

            // ASSERT
            Assert.That(viewModel.HasOverlay, Is.True);
            Assert.That(viewModel.OverlayHeader, Is.EqualTo("header 1"));
            Assert.That(viewModel.Overlay, Is.EqualTo(contentViewModel.Object));
        }

        [Test]
        public void clears_overlay()
        {
            // ARRANGE
            var viewModel = new ChromeViewModel(_mainViewModel, _overlayService.Object);
            var contentViewModel = new Mock<BaseViewModel>();
            var overlayViewModel = new OverlayViewModel("header 1", contentViewModel.Object, Disposable.Empty);

            _show.OnNext(overlayViewModel);

            // ACT
            viewModel.CloseOverlayCommand.Execute(null);

            // ASSERT
            Assert.That(viewModel.HasOverlay, Is.False);
            Assert.That(viewModel.OverlayHeader, Is.Empty);
            Assert.That(viewModel.Overlay, Is.Null);
        }

        [Test]
        public void replaces_overlay()
        {
            // ARRANGE
            var viewModel = new ChromeViewModel(_mainViewModel, _overlayService.Object);

            var contentViewModel1 = new Mock<BaseViewModel>();
            var overlayViewModel1 = new OverlayViewModel("header 1", contentViewModel1.Object, Disposable.Empty);

            var contentViewModel2 = new Mock<BaseViewModel>();
            var overlayViewModel2 = new OverlayViewModel("header 2", contentViewModel2.Object, Disposable.Empty);

            // ACT
            _show.OnNext(overlayViewModel1);
            _show.OnNext(overlayViewModel2);

            // ASSERT
            Assert.That(viewModel.HasOverlay, Is.True);
            Assert.That(viewModel.OverlayHeader, Is.EqualTo("header 2"));
            Assert.That(viewModel.Overlay, Is.EqualTo(contentViewModel2.Object));
        }
    }
}