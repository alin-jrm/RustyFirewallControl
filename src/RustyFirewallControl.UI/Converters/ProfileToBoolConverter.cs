using System;
using System.Globalization;
using System.Windows.Data;
using RustyFirewallControl.Common;

namespace RustyFirewallControl.UI.Converters
{
    public class ProfileToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FilteringProfile profile && parameter is string id)
            {
                return profile switch
                {
                    FilteringProfile.NoFiltering => id == "none",
                    FilteringProfile.LowFiltering => id == "low",
                    FilteringProfile.MediumFiltering => id == "medium",
                    FilteringProfile.HighFiltering => id == "high",
                    _ => false
                };
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter is string id)
            {
                return id switch
                {
                    "low" => FilteringProfile.LowFiltering,
                    "medium" => FilteringProfile.MediumFiltering,
                    "high" => FilteringProfile.HighFiltering,
                    _ => FilteringProfile.NoFiltering,
                };
            }

            return Binding.DoNothing;
        }
    }
}
