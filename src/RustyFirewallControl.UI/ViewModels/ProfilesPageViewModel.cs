using System;
using System.Windows.Input;
using RustyFirewallControl.Common;
using RustyFirewallControl.UI.Mvvm;
using RustyFirewallControl.UI.Properties;

namespace RustyFirewallControl.UI.ViewModels
{
    public class ProfilesPageViewModel : PageViewModelBase
    {
        private FilteringProfile profile;

        public ProfilesPageViewModel()
        {
            Title = Resources.ProfilesPage_Title;
            Icon = "Profiles";
            ProfileSelected = new RelayCommand(() => ProfileChanged?.Invoke(Profile));
        }

        public event Action<FilteringProfile> ProfileChanged;

        public FilteringProfile Profile
        {
            get => profile;
            set => SetProperty(ref profile, value);
        }

        public ICommand ProfileSelected { get; }
    }
}
