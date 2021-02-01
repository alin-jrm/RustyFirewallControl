using RustyFirewallControl.UI.Mvvm;

namespace RustyFirewallControl.UI.ViewModels
{
    public class PageViewModelBase : ViewModelBase
    {
        private string title;
        private string icon;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public string Icon
        {
            get => icon;
            set => SetProperty(ref icon, value);
        }
    }
}
