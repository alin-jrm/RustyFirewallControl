using RustyFirewallControl.Common;
using RustyFirewallControl.UI.Properties;

namespace RustyFirewallControl.UI.ViewModels
{
    public class DashboardPageViewModel : PageViewModelBase
    {
        private FirewallStatus firewallStatus;
        private string inboundAction = "Allow";
        private string isFirewallEnabled = "Off";
        private string location = "Public";
        private string outboundAction = "Allow";
        private FilteringProfile profile = FilteringProfile.NoFiltering;

        public DashboardPageViewModel()
        {
            Title = Resources.DashboardPage_Title;
            Icon = "Dashboard";
            Profile = FilteringProfile.MediumFiltering;
        }

        public FirewallStatus FirewallStatus
        {
            get => firewallStatus;
            set
            {
                SetProperty(ref firewallStatus, value);
                Profile = value.FilteringProfile;
                IsFirewallEnabled = Convert(value.IsEnabled);
                InboundAction = Convert(value.IsEnabled, value.InboundAction);
                OutboundAction = Convert(value.IsEnabled, value.OutboundAction);
                Location = Convert(value.NetworkProfile);
            }
        }

        public string InboundAction
        {
            get => inboundAction;
            set => SetProperty(ref inboundAction, value);
        }

        public string IsFirewallEnabled
        {
            get => isFirewallEnabled;
            set => SetProperty(ref isFirewallEnabled, value);
        }

        public string Location
        {
            get => location;
            set => SetProperty(ref location, value);
        }

        public string OutboundAction
        {
            get => outboundAction;
            set => SetProperty(ref outboundAction, value);
        }

        public FilteringProfile Profile
        {
            get => profile;
            set => SetProperty(ref profile, value);
        }

        private static string Convert(NetworkProfile networkProfile)
            => networkProfile switch
            {
                NetworkProfile.Private => "Private",
                NetworkProfile.Public => "Public",
                NetworkProfile.Domain => "Domain",
                _ => "N/A"
            };

        private static string Convert(bool isEnabled, FirewallAction inboundAction)
            => !isEnabled
                ? "Allow All"
                : inboundAction switch
                {
                    FirewallAction.Allow => "Allow",
                    FirewallAction.Block => "Block",
                    FirewallAction.BlockAll => "Block All",
                    _ => "N/A"
                };

        private static string Convert(bool isEnabled)
            => isEnabled ? "On" : "Off";
    }
}