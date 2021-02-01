using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using RustyFirewallControl.Common;
using RustyFirewallControl.UI.Properties;

namespace RustyFirewallControl.UI
{
    public class TrayIcon
    {
        private NotifyIcon notificationIcon;
        private FilteringProfile profile;
        private List<ToolStripMenuItem> profilesMenuItems;

        public ICommand ChangeProfileCommand { get; set; }

        public ICommand ExitCommand { get; set; }

        public FilteringProfile Profile
        {
            get => profile;
            set
            {
                profile = value;
                if (notificationIcon == null)
                {
                    return;
                }

                notificationIcon.Icon = ProfileIcon(value);

                foreach (var item in profilesMenuItems)
                {
                    item.Checked = item.Tag.Equals(Profile);
                }
            }
        }

        public ICommand ShowCommand { get; set; }

        public void Initialize()
        {
            var menu = new ContextMenuStrip();
            var profiles = new ToolStripMenuItem(Resources.TrayProfiles);

            profilesMenuItems = new List<ToolStripMenuItem>
            {
                CreateProfileMenuItem(Resources.HighFiltering, FilteringProfile.HighFiltering, Resources.Green),
                CreateProfileMenuItem(Resources.MediumFiltering, FilteringProfile.MediumFiltering, Resources.Green),
                CreateProfileMenuItem(Resources.LowFiltering, FilteringProfile.LowFiltering, Resources.Orange),
                CreateProfileMenuItem(Resources.NoFiltering, FilteringProfile.NoFiltering, Resources.Red),
            };

            foreach (var item in profilesMenuItems)
            {
                profiles.DropDownItems.Add(item);
            }

            menu.Items.Add(profiles);
            menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add(Resources.TrayIconShow, null, OnShow);
            menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add(Resources.TrayIconExit, null, OnExit);

            notificationIcon = new NotifyIcon
            {
                Visible = true,
                Icon = ProfileIcon(Profile),
                ContextMenuStrip = menu,
            };
            notificationIcon.DoubleClick += OnShow;
        }

        public void UnInitialize()
        {
            notificationIcon.Visible = false;
        }

        private static Icon ProfileIcon(FilteringProfile profile)
            => profile switch
            {
                FilteringProfile.NoFiltering => Resources.Red,
                FilteringProfile.LowFiltering => Resources.Orange,
                FilteringProfile.MediumFiltering => Resources.Green,
                FilteringProfile.HighFiltering => Resources.Green,
                _ => Resources.Gray
            };

        private ToolStripMenuItem CreateProfileMenuItem(string caption, FilteringProfile filteringProfile, Icon icon)
        {
            var item = new ToolStripMenuItem(caption)
            {
                Tag = filteringProfile,
                Image = icon.ToBitmap(),
                Checked = filteringProfile == profile,
                CheckOnClick = false,
            };
            item.Click += OnChangeProfile;
            return item;
        }

        private void OnChangeProfile(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && ChangeProfileCommand?.CanExecute(item.Tag) == true)
            {
                ChangeProfileCommand.Execute(item.Tag);
            }
        }

        private void OnExit(object sender, EventArgs eventArgs)
        {
            if (ExitCommand?.CanExecute(null) == true)
            {
                ExitCommand.Execute(null);
            }
        }

        private void OnShow(object sender, EventArgs e)
        {
            if (ShowCommand?.CanExecute(null) == true)
            {
                ShowCommand.Execute(null);
            }
        }
    }
}