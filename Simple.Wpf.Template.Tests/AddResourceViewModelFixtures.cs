namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Moq;
    using NUnit.Framework;
    using Rest;
    using ViewModels;

    [TestFixture]
    public sealed class AddResourceViewModelFixtures : BaseServiceFixtures
    {
        private Mock<IRestClient> _restClient;

        [SetUp]
        public void Setup()
        {
            var result = new Mock<Rest.IRestResponse<Resource>>();
            var task = Task.FromResult(result.Object);

            _restClient = new Mock<IRestClient>();
            _restClient.Setup(x => x.PostAsync(It.IsAny<Uri>(), It.IsAny<Resource>())).Returns(() => task).Verifiable();
        }

        [Test]
        public void can_not_add_resoruce_when_json_invalid()
        {
            // ARRANGE
            var metadata = Enumerable.Empty<Metadata>();

            var viewModel = new AddResourceViewModel(metadata, _restClient.Object)
            {
                Path = "test/1",
                Json = "{"
            };

            // ACT
            var result = viewModel.ConfirmCommand.CanExecute(null);

            // ASSERT
            Assert.That(result, Is.False);
        }

        [Test]
        public void can_not_add_resoruce_when_path_is_empty()
        {
            // ARRANGE
            var metadata = Enumerable.Empty<Metadata>();

            var viewModel = new AddResourceViewModel(metadata, _restClient.Object)
            {
                Path = "",
                Json = "{}"
            };

            // ACT
            var result = viewModel.ConfirmCommand.CanExecute(null);

            // ASSERT
            Assert.That(result, Is.False);
        }

        [Test]
        public void adds_resource_when_confirmed()
        {
            // ARRANGE
            var metadata = Enumerable.Empty<Metadata>();

            var viewModel = new AddResourceViewModel(metadata, _restClient.Object)
                            {
                                Path = "test/1",
                                Json = "{}"
                            };

            // ACT
            viewModel.ConfirmCommand.Execute(null);

            // ASSERT
            _restClient.VerifyAll();
        }
    }
}