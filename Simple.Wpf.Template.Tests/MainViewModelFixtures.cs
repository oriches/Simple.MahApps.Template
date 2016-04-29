namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    public sealed class MainViewModelFixtures : BaseViewModelFixtures
    {
        [SetUp]
        public void Setup()
        {
            _diagnosticsService = new Mock<IDiagnosticsService>();
            _diagnosticsService.Setup(x => x.Cpu).Returns(Observable.Never<int>);
            _diagnosticsService.Setup(x => x.Memory).Returns(Observable.Never<Memory>);
            _diagnosticsService.Setup(x => x.Log).Returns(Observable.Never<string>);

            _restClient = new Mock<IRestClient>();

            _messageService = new Mock<IMessageService>();

            _diagnostics = new Mock<IDiagnosticsViewModel>();

            var addResourceViewModel = new Mock<IAddResourceViewModel>();
            _addResourceFactory = x => addResourceViewModel.Object;

            var metadataViewModel = new Mock<IMetadataViewModel>();
            _metadataFactory = x => metadataViewModel.Object;
        }

        private Mock<IMessageService> _messageService;
        private Mock<IDiagnosticsService> _diagnosticsService;
        private Mock<IDiagnosticsViewModel> _diagnostics;
        private Mock<IRestClient> _restClient;

        private Func<IEnumerable<Metadata>, IAddResourceViewModel> _addResourceFactory;
        private Func<Metadata, IMetadataViewModel> _metadataFactory;

        [Test]
        public void is_offline_when_request_times_out()
        {
            // ARRANGE
            var heartbeatResponse = new Mock<IRestResponse<Heartbeat>>();
            heartbeatResponse.Setup(x => x.Resource).Returns(new Heartbeat("some timestamp"));

            var task = Task.Delay(Constants.Server.Hearbeat.Timeout.Add(TimeSpan.FromMilliseconds(100)))
                .ContinueWith(x => heartbeatResponse.Object);

            _restClient.Setup(x => x.GetAsync<Heartbeat>(It.Is<Uri>(y => y == Constants.Server.Hearbeat.Url)))
                .Returns(task);

            // ACT
            var viewModel = new MainViewModel(_addResourceFactory,
                _metadataFactory, 
                _diagnostics.Object,
                _messageService.Object,
                _restClient.Object,
                SchedulerService);

            TestScheduler.AdvanceBy(Constants.Server.Hearbeat.Interval.Add(TimeSpan.FromMilliseconds(100)));

            // ASSERT
            Assert.That(viewModel.IsServerOnline, Is.False);
        }

        [Test, Ignore("Unknown threading issue")]
        public void is_offline_when_server_throws_an_error()
        {
            // ARRANGE
            var heartbeatResponse = new Mock<IRestResponse<Heartbeat>>();
            heartbeatResponse.Setup(x => x.Resource).Throws(new Exception("Foo"));

            _restClient.Setup(x => x.GetAsync<Heartbeat>(It.Is<Uri>(y => y == Constants.Server.Hearbeat.Url)))
                .Returns(Task.FromResult(heartbeatResponse.Object));

            // ACT
            var viewModel = new MainViewModel(_addResourceFactory,
                _metadataFactory,
                _diagnostics.Object,
                _messageService.Object,
                _restClient.Object,
                SchedulerService);

            TestScheduler.AdvanceBy(Constants.Server.Hearbeat.Interval.Add(TimeSpan.FromMilliseconds(100)));

            // ASSERT
            Assert.That(viewModel.IsServerOnline, Is.False);
        }

        [Test]
        public void is_online()
        {
            // ARRANGE
            var heartbeatResponse = new Mock<IRestResponse<Heartbeat>>();
            heartbeatResponse.Setup(x => x.Resource).Returns(new Heartbeat("some timestamp"));

            _restClient.Setup(x => x.GetAsync<Heartbeat>(It.IsAny<Uri>()))
                .Returns(Task.FromResult(heartbeatResponse.Object));

            var resourceResponse = new Mock<IRestResponse<IEnumerable<Metadata>>>();
            resourceResponse.Setup(x => x.Resource).Returns(new Metadata[0]);

            _restClient.Setup(x => x.GetAsync<IEnumerable<Metadata>>(It.Is<Uri>(y => y == Constants.Server.MetadataUrl)))
                .Returns(Task.FromResult(resourceResponse.Object));

            // ACT
            var viewModel = new MainViewModel(_addResourceFactory,
                _metadataFactory,
                _diagnostics.Object,
                _messageService.Object,
                _restClient.Object,
                SchedulerService);

            TestScheduler.AdvanceBy(Constants.Server.Hearbeat.Interval.Add(TimeSpan.FromMilliseconds(100)));

            // ASSERT
            Assert.That(viewModel.IsServerOnline, Is.True);
        }

        [Test]
        public void populates_metadata()
        {
            // ARRANGE
            var heartbeatResponse = new Mock<IRestResponse<Heartbeat>>();
            heartbeatResponse.Setup(x => x.Resource).Returns(new Heartbeat("some timestamp"));

            _restClient.Setup(x => x.GetAsync<Heartbeat>(It.Is<Uri>(y => y == Constants.Server.Hearbeat.Url)))
                .Returns(Task.FromResult(heartbeatResponse.Object));

            var metadata = new[]
                            {
                                new Metadata(new Uri("http://localhost/foo"), true)
                            };

            var metadataViewModel = new Mock<IMetadataViewModel>();
            metadataViewModel.Setup(x => x.Metadata).Returns(metadata.First());
            metadataViewModel.Setup(x => x.Deleted).Returns(Observable.Never<Unit>());

            _metadataFactory = x => metadataViewModel.Object;

            var resourceResponse = new Mock<IRestResponse<IEnumerable<Metadata>>>();
            resourceResponse.Setup(x => x.Resource).Returns(metadata);

            _restClient.Setup(x => x.GetAsync<IEnumerable<Metadata>>(It.Is<Uri>(y => y == Constants.Server.MetadataUrl)))
                .Returns(Task.FromResult(resourceResponse.Object));

            // ACT
            var viewModel = new MainViewModel(_addResourceFactory,
                _metadataFactory,
                _diagnostics.Object,
                _messageService.Object,
                _restClient.Object,
                SchedulerService);

            TestScheduler.AdvanceBy(Constants.Server.Hearbeat.Interval.Add(TimeSpan.FromMilliseconds(100)));

            // ASSERT
            var result = viewModel.Metadata.Cast<IMetadataViewModel>().Select(x => x.Metadata);

            Assert.That(result, Is.Not.Empty);
            Assert.That(result.First().Url, Is.EqualTo(metadata.First().Url));
            Assert.That(result.First().Immutable, Is.EqualTo(metadata.First().Immutable));
        }

        [Test]
        public void adds_metadata()
        {
            // ARRANGE
            var heartbeatResponse = new Mock<IRestResponse<Heartbeat>>();
            heartbeatResponse.Setup(x => x.Resource).Returns(new Heartbeat("some timestamp"));

            _restClient.Setup(x => x.GetAsync<Heartbeat>(It.IsAny<Uri>()))
                .Returns(Task.FromResult(heartbeatResponse.Object));

            var resourceResponse = new Mock<IRestResponse<IEnumerable<Metadata>>>();
            resourceResponse.Setup(x => x.Resource).Returns(new Metadata[0]);

            _restClient.Setup(x => x.GetAsync<IEnumerable<Metadata>>(It.Is<Uri>(y => y == Constants.Server.MetadataUrl)))
                .Returns(Task.FromResult(resourceResponse.Object));

            var addResourceViewModel = new Mock<IAddResourceViewModel>();
            addResourceViewModel.Setup(x => x.Added).Returns(Observable.Return(Unit.Default));
            _addResourceFactory = x => addResourceViewModel.Object;

            // ACT
            var viewModel = new MainViewModel(_addResourceFactory,
                _metadataFactory,
                _diagnostics.Object,
                _messageService.Object,
                _restClient.Object,
                SchedulerService);

            TestScheduler.AdvanceBy(Constants.Server.Hearbeat.Interval.Add(TimeSpan.FromMilliseconds(100)));

            viewModel.AddCommand.Execute(null);

            // ASSERT
            addResourceViewModel.VerifyAll();
        }
    }
}