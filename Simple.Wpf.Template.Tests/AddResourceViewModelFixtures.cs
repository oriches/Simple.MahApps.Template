using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Simple.Rest.Common;
using Simple.Wpf.Template.Models;
using Simple.Wpf.Template.ViewModels;

namespace Simple.Wpf.Template.Tests
{
    [TestFixture]
    public sealed class AddResourceViewModelFixtures : BaseViewModelFixtures
    {
        [SetUp]
        public void Setup()
        {
            var result = new Mock<IRestResponse<Resource>>();
            var task = Task.FromResult(result.Object);

            _restClient = new Mock<IRestClient>();
            _restClient.Setup(x => x.PostAsync(It.IsAny<Uri>(), It.IsAny<Resource>()))
                .Returns(() => task)
                .Verifiable();
        }

        private Mock<IRestClient> _restClient;

        [Test]
        public void can_not_add_resource_when_json_invalid()
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
        public void can_not_add_resource_when_path_is_empty()
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

            viewModel.Added.Subscribe();

            // ACT
            viewModel.ConfirmCommand.Execute(null);

            // ASSERT
            _restClient.VerifyAll();
        }
    }
}