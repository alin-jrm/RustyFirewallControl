using System;
using System.Threading.Tasks;

namespace RustyFirewallControl.Common
{
    public interface IFirewallClient
    {
        event Action<FirewallStatus> StatusChanged;

        FirewallStatus Status { get; }

        void SetFilteringProfile(FilteringProfile filteringProfile);

        void StartStatusListener();

        void StopStatusListener();
    }
}
