using System;
using System.Windows;
using System.Windows.Controls;
using RustyFirewallControl.Common;
using RustyFirewallControl.UI.ViewModels;

namespace RustyFirewallControl.UI.Views
{
    public partial class DashboardPage : UserControl
    {
        public static readonly DependencyProperty FirewallStatusProperty = DependencyProperty.Register(
            "FirewallStatus", typeof(FirewallStatus), typeof(DashboardPage), new PropertyMetadata(default(FirewallStatus), OnFirewallStatusChanged));

        public DashboardPage()
        {
            InitializeComponent();
        }

        public FirewallStatus FirewallStatus
        {
            get { return (FirewallStatus)GetValue(FirewallStatusProperty); }
            set { SetValue(FirewallStatusProperty, value); }
        }

        protected DashboardPageViewModel ViewModel
            => (DashboardPageViewModel)Resources["ViewModel"];

        private static void OnFirewallStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }

            ((DashboardPage)d).ViewModel.FirewallStatus = (FirewallStatus)e.NewValue;
        }
    }
}
