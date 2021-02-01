using System.Collections.Generic;
using System.Windows.Input;
using RustyFirewallControl.Common;
using RustyFirewallControl.UI.Mvvm;

namespace RustyFirewallControl.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IFirewallClient firewallClient;
        private readonly List<PageViewModelBase> pages;
        private FilteringProfile filteringProfile;
        private FirewallStatus firewallStatus;
        private PageViewModelBase selectedPage;

        public MainWindowViewModel()
            : this((IFirewallClient)ServiceLocator.Instance.GetService(typeof(IFirewallClient)))
        {
        }

        public MainWindowViewModel(IFirewallClient firewallClient)
        {
            this.firewallClient = firewallClient;
            pages = new List<PageViewModelBase>
            {
                new DashboardPageViewModel(),
                new ProfilesPageViewModel(),
                new OptionsPageViewModel(),
            };

            selectedPage = pages[0];
            ChangeProfileCommand = new RelayCommand<FilteringProfile>(ChangeProfile);
        }

        public ICommand ChangeProfileCommand { get; }

        public FilteringProfile FilteringProfile
        {
            get => filteringProfile;
            private set => SetProperty(ref filteringProfile, value);
        }

        public FirewallStatus FirewallStatus
        {
            get => firewallStatus;
            private set
            {
                if (SetProperty(ref firewallStatus, value))
                {
                    FilteringProfile = value.FilteringProfile;
                }
            }
        }

        public IReadOnlyCollection<PageViewModelBase> Pages
            => pages;

        public PageViewModelBase SelectedPage
        {
            get => selectedPage;
            set => SetProperty(ref selectedPage, value);
        }

        public void Initialize()
        {
            FirewallStatus = firewallClient.Status;
        }

        public void OnLoaded()
        {
            firewallClient.StatusChanged += FirewallClient_StatusChanged;
            firewallClient.StartStatusListener();
        }

        public void OnUnloaded()
        {
            firewallClient.StopStatusListener();
            firewallClient.StatusChanged -= FirewallClient_StatusChanged;
        }

        private void ChangeProfile(FilteringProfile profile)
        {
            firewallClient.SetFilteringProfile(profile);
            FirewallStatus = firewallClient.Status;
        }

        private void FirewallClient_StatusChanged(FirewallStatus firewallStatus)
        {
            FirewallStatus = firewallStatus;
        }
    }
}