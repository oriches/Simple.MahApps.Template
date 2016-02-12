namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Reactive.Disposables;
    using NUnit.Framework;
    using Services;
    using ViewModels;

    [TestFixture]
    public sealed class MessageServiceFixtures: BaseServiceFixtures
    {
        [Test]
        public void posts_message_with_lifetime()
        {
            // ARRANGE
            var contentViewModel = new DateOfBirthViewModel();

            var lifetime = Disposable.Empty;

            var service = new MessageService();

            MessageViewModel messageViewModel = null;
            service.Show.Subscribe(x => messageViewModel = x);

            // ACT
            service.Post("header 1", contentViewModel, lifetime);

            // ASSERT
            Assert.That(messageViewModel.HasLifetime, Is.True);
            Assert.That(messageViewModel.Lifetime, Is.EqualTo(lifetime));
            Assert.That(messageViewModel.Header, Is.EqualTo("header 1"));
            Assert.That(messageViewModel.ViewModel, Is.EqualTo(contentViewModel));
        }

        [Test]
        public void posts_overlay_without_lifetime()
        {
            // ARRANGE
            var contentViewModel = new DateOfBirthViewModel();

            var service = new MessageService();

            MessageViewModel messageViewModel = null;
            service.Show.Subscribe(x => messageViewModel = x);

            // ACT
            service.Post("header 1", contentViewModel, null);

            // ASSERT
            Assert.That(messageViewModel.HasLifetime, Is.False);
            Assert.That(messageViewModel.Lifetime, Is.Null);
            Assert.That(messageViewModel.Header, Is.EqualTo("header 1"));
            Assert.That(messageViewModel.ViewModel, Is.EqualTo(contentViewModel));
        }
    }
}