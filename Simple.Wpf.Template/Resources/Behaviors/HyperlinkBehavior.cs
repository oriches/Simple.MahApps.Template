using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Navigation;
using Microsoft.Xaml.Behaviors;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.Resources.Behaviors
{
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