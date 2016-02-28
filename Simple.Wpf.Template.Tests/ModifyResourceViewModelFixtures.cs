namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Threading.Tasks;
    using Models;
    using Moq;
    using NUnit.Framework;
    using Rest;
    using ViewModels;

    [TestFixture]
    public sealed class ModifyResourceViewModelFixtures : BaseViewModelFixtures
    {
        private Mock<IRestClient> _restClient;
        private Metadata _metadata;

        [SetUp]
        public void Setup()
        {
            _metadata = new Metadata(new Uri("http://localhost/test/1"), true);

            _restClient = new Mock<IRestClient>();

            var getResult = new Mock<IRestResponse<object>>();
            getResult.Setup(x => x.Resource).Returns(new Resource("{\"foo\" : \"bar\"}"));
            var getTask = Task.FromResult(getResult.Object);
            _restClient.Setup(x => x.GetAsync<object>(It.IsAny<Uri>())).Returns(() => getTask).Verifiable();

            var putResult = new Mock<IRestResponse>();
            var putTask = Task.FromResult(putResult.Object);
            _restClient.Setup(x => x.PutAsync(It.IsAny<Uri>(), It.IsAny<Resource>())).Returns(() => putTask).Verifiable();
        }

        [Test]
        public void can_not_add_resource_when_json_invalid()
        {
            // ARRANGE
            var viewModel = new ModifyResourceViewModel(_metadata, _restClient.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            viewModel.Json = "{";

            // ACT
            var result = viewModel.ConfirmCommand.CanExecute(null);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(result, Is.False);
        }

        [Test]
        public void modifies_resource_when_confirmed()
        {
            // ARRANGE
            var viewModel = new ModifyResourceViewModel(_metadata, _restClient.Object, SchedulerService);
            
            viewModel.Confirmed.Subscribe();

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            viewModel.Json = "{}";

            // ACT
            viewModel.ConfirmCommand.Execute(null);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            _restClient.VerifyAll();
        }
    }
}