using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Simple.Rest.Common;
using Simple.Wpf.Template.Models;
using Simple.Wpf.Template.Services;
using Simple.Wpf.Template.ViewModels;

namespace Simple.Wpf.Template.Tests
{
    [TestFixture]
    public sealed class MessageServiceFixtures : BaseServiceFixtures
    {
        [SetUp]
        public void Setup() => _restClient = new Mock<IRestClient>();

        private Mock<IRestClient> _restClient;

        [Test]
        public void posts_message_with_lifetime()
        {
            // ARRANGE
            var contentViewModel = new AddResourceViewModel(Enumerable.Empty<Metadata>(), _restClient.Object);

            var service = new MessageService();

            Message message = null;
            service.Show.Subscribe(x => message = x);

            // ACT
            service.Post("header 1", contentViewModel);

            // ASSERT
            Assert.That(message.Header, Is.EqualTo("header 1"));
            Assert.That(message.ViewModel, Is.EqualTo(contentViewModel));
        }

        [Test]
        public void posts_overlay_without_lifetime()
        {
            // ARRANGE
            var contentViewModel = new AddResourceViewModel(Enumerable.Empty<Metadata>(), _restClient.Object);

            var service = new MessageService();

            Message message = null;
            service.Show.Subscribe(x => message = x);

            // ACT
            service.Post("header 1", contentViewModel);

            // ASSERT
            Assert.That(message.Header, Is.EqualTo("header 1"));
            Assert.That(message.ViewModel, Is.EqualTo(contentViewModel));
        }
    }
}