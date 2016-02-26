namespace Simple.Wpf.Template.Resources.Behaviors
{
    using System.Diagnostics;
    using System.Windows.Documents;
    using System.Windows.Interactivity;
    using System.Windows.Navigation;
    using Services;

    public sealed class HyperlinkBehavior : Behavior<Hyperlink>
    {
        private IGestureService _gestureService;

        protected override void OnAttached()
        {
            base.OnAttached();

            _gestureService = BootStrapper.Resolve<IGestureService>();

            AssociatedObject.RequestNavigate += HandleRequestNavigate;
        }

        private void HandleRequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            _gestureService.SetBusy();

            Process.Start(new ProcessStartInfo(args.Uri.AbsoluteUri));
            args.Handled = true;
        }
    }
}
