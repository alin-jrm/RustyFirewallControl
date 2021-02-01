using System;
using System.ComponentModel;
using System.Windows;
using RustyFirewallControl.Common;
using RustyFirewallControl.UI.Mvvm;
using RustyFirewallControl.UI.ViewModels;

namespace RustyFirewallControl.UI
{
    public partial class MainWindow : Window
    {
        private TrayIcon trayIcon;

        public MainWindow()
        {
            InitializeComponent();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            ViewModel.Initialize();

            Loaded += (_, __) => ViewModel.OnLoaded();
            Unloaded += (_, __) => ViewModel.OnUnloaded();
        }

        protected MainWindowViewModel ViewModel
            => (MainWindowViewModel)Resources["ViewModel"];

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            trayIcon = new TrayIcon
            {
                ExitCommand = new RelayCommand(ExitApp),
                ShowCommand = new RelayCommand(ReShow),
                ChangeProfileCommand = ViewModel.ChangeProfileCommand,
                Profile = ViewModel.FilteringProfile,
            };
            trayIcon.Initialize();
        }

        private void ExitApp()
        {
            trayIcon.UnInitialize();
            Application.Current.Shutdown();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.FilteringProfile))
            {
                trayIcon.Profile = ViewModel.FilteringProfile;
            }
        }

        private void ProfilesPageProfileChanged(FilteringProfile profile)
        {
            ViewModel.ChangeProfileCommand.Execute(profile);
        }

        private void ReShow()
        {
            ShowInTaskbar = true;
            Show();
            Activate();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            ShowInTaskbar = false;
            Hide();
            e.Cancel = true;
        }
    }
}