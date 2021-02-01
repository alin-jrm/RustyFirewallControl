using System;
using System.Windows;
using System.Windows.Controls;
using RustyFirewallControl.Common;
using RustyFirewallControl.UI.ViewModels;

namespace RustyFirewallControl.UI.Views
{
    public partial class ProfilesPage : UserControl
    {
        public static readonly DependencyProperty ProfileProperty = DependencyProperty.Register(
            nameof(Profile), typeof(FilteringProfile), typeof(ProfilesPage), new PropertyMetadata(default(FilteringProfile), OnProfileChanged));

        public ProfilesPage()
        {
            InitializeComponent();
        }

        public event Action<FilteringProfile> ProfileChanged
        {
            add
            {
                ViewModel.ProfileChanged += value;
            }

            remove
            {
                ViewModel.ProfileChanged -= value;
            }
        }

        public FilteringProfile Profile
        {
            get { return (FilteringProfile)GetValue(ProfileProperty); }
            set { SetValue(ProfileProperty, value); }
        }

        protected ProfilesPageViewModel ViewModel
            => (ProfilesPageViewModel)Resources["ViewModel"];

        private static void OnProfileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProfilesPage)d).ViewModel.Profile = (FilteringProfile)e.NewValue;
        }
    }
}