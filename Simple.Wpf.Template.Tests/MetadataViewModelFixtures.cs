namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using Models;
    using Moq;
    using NUnit.Framework;
    using Rest;
    using Services;
    using ViewModels;

    [TestFixture]
    public sealed class MetadataViewModelFixtures : BaseViewModelFixtures
    {
        private Mock<IRestClient> _restClient;
        private Mock<IMessageService> _messageService;
        private Func<Metadata, IModifyResourceViewModel> _modifyResourceFactory;
        private Mock<IModifyResourceViewModel> _modifyResourceViewModel;
        private Mock<IExceptionViewModel> _exceptionViewModel;
        private Func<Exception, IExceptionViewModel> _exceptionFactory;

        [SetUp]
        public void Setup()
        {
            _restClient = new Mock<IRestClient>();

            var result = new Mock<IRestResponse>();
            var task = Task.FromResult(result.Object);

            _restClient = new Mock<IRestClient>();
            _restClient.Setup(x => x.DeleteAsync(It.IsAny<Uri>())).Returns(() => task).Verifiable();

            _messageService = new Mock<IMessageService>();
            _messageService.Setup(x => x.Post(It.IsAny<string>(), It.IsAny<ICloseableViewModel>()));

            _modifyResourceViewModel = new Mock<IModifyResourceViewModel>();
            _modifyResourceFactory = x => _modifyResourceViewModel.Object;

            _exceptionViewModel = new Mock<IExceptionViewModel>();
            _exceptionViewModel.Setup(x => x.Closed).Returns(Observable.Return(Unit.Default));
            _exceptionViewModel.Setup(x => x.Dispose());
            _exceptionFactory = x => _exceptionViewModel.Object;
        }

        [Test]
        public void delete()
        {
            // ARRANGE
            var metadata = new Metadata(new Uri("http://localhost/test/1"), true);

            var viewModel = new MetadataViewModel(metadata, _modifyResourceFactory, _exceptionFactory, _restClient.Object, _messageService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            var deleted = false;
            viewModel.Deleted.Subscribe(x => deleted = true);

            // ACT
            viewModel.DeleteCommand.Execute(null);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(deleted, Is.True);

            _restClient.VerifyAll();
        }

        [Test]
        public void delete_after_failed_delete()
        {
            // ARRANGE
            var metadata = new Metadata(new Uri("http://localhost/test/1"), true);

            _restClient.Setup(x => x.DeleteAsync(It.IsAny<Uri>())).Throws(new Exception("Foo"));

            var viewModel = new MetadataViewModel(metadata, _modifyResourceFactory, _exceptionFactory, _restClient.Object, _messageService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            var deleted = 0;
            viewModel.Deleted.Subscribe(x => deleted++);

            // ACT
            viewModel.DeleteCommand.Execute(null);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            var result = new Mock<IRestResponse>();
            var task = Task.FromResult(result.Object);
            _restClient.Setup(x => x.DeleteAsync(It.IsAny<Uri>())).Returns(() => task).Verifiable();

            viewModel.DeleteCommand.Execute(null);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(deleted, Is.EqualTo(1));

            _restClient.VerifyAll();
        }

        [Test]
        public void modify()
        {
            // ARRANGE
            var metadata = new Metadata(new Uri("http://localhost/test/1"), true);

            var viewModel = new MetadataViewModel(metadata, _modifyResourceFactory, _exceptionFactory, _restClient.Object, _messageService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            viewModel.ModifyCommand.Execute(null);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            _messageService.VerifyAll();
        }
    }
}