namespace RustyFirewallControl.Common
{
    public class FirewallStatus
    {
        public bool IsEnabled { get; set; }

        public FilteringProfile FilteringProfile
        {
            get
            {
                if (!IsEnabled || (InboundAction == FirewallAction.Allow && OutboundAction == FirewallAction.Allow))
                {
                    return FilteringProfile.NoFiltering;
                }

                if (InboundAction == FirewallAction.BlockAll && OutboundAction == FirewallAction.BlockAll)
                {
                    return FilteringProfile.HighFiltering;
                }

                return OutboundAction == FirewallAction.Allow
                        ? FilteringProfile.LowFiltering
                        : FilteringProfile.MediumFiltering;
            }
        }

        public NetworkProfile NetworkProfile { get; set; }

        public FirewallAction InboundAction { get; set; }

        public FirewallAction OutboundAction { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is FirewallStatus other)
            {
                return other.IsEnabled == IsEnabled
                    && other.FilteringProfile == FilteringProfile
                    && other.NetworkProfile == NetworkProfile
                    && other.InboundAction == InboundAction
                    && other.OutboundAction == OutboundAction;
            }

            return Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return (IsEnabled, FilteringProfile, NetworkProfile, InboundAction, OutboundAction).GetHashCode();
        }

    }
}
