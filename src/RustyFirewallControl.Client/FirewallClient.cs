using NetFwTypeLib;
using RustyFirewallControl.Common;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RustyFirewallControl.Client
{
    public class FirewallClient : IFirewallClient
    {
        public const string BlockAllInboundRuleName = "High Filtering profile - Block inbound connections";
        public const string BlockAllOutboundRuleName = "High Filtering profile - Block outbound connections";
        public const string FirewallGroupName = "Rusty firewall control";

        private readonly (NET_FW_ACTION_ Flag, FirewallAction Value)[] actionsMap = new[]
        {
            (Flag: NET_FW_ACTION_.NET_FW_ACTION_ALLOW, Value: FirewallAction.Allow),
            (Flag: NET_FW_ACTION_.NET_FW_ACTION_BLOCK, Value: FirewallAction.Block),
        };

        private readonly INetFwPolicy2 firewallPolicy;

        private readonly (NET_FW_PROFILE_TYPE2_ Flag, NetworkProfile Value)[] profilesMap = new[]
        {
            (Flag: NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE, Value: NetworkProfile.Private),
            (Flag: NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC, Value: NetworkProfile.Public),
            (Flag: NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN, Value: NetworkProfile.Domain),
        };

        private readonly TimeSpan statusPullInterval = TimeSpan.FromSeconds(2);

        private CancellationTokenSource statusCancelationTokenSource;

        public FirewallClient()
        {
            var firewallPolicyType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(firewallPolicyType);
        }

        public event Action<FirewallStatus> StatusChanged;

        public FirewallStatus Status
        {
            get
            {
                var currentProfile = CurrentNetworkProfileFlag();

                return new FirewallStatus
                {
                    IsEnabled = IsFirewallEnabled(),
                    NetworkProfile = Array.Find(profilesMap, p => p.Flag == currentProfile).Value,
                    InboundAction = InboundAction(currentProfile),
                    OutboundAction = OutboundAction(currentProfile),
                };
            }
        }

        public void SetFilteringProfile(FilteringProfile filteringProfile)
        {
            RemoveRule(BlockAllOutboundRuleName);
            switch (filteringProfile)
            {
                case FilteringProfile.NoFiltering:
                    ToggleFirewall(false);
                    break;

                case FilteringProfile.HighFiltering:
                    SetInboundAction(FirewallAction.BlockAll);
                    SetOutboundAction(FirewallAction.BlockAll);
                    ToggleFirewall(true);
                    break;

                case FilteringProfile.LowFiltering:
                    SetInboundAction(FirewallAction.Block);
                    SetOutboundAction(FirewallAction.Allow);
                    ToggleFirewall(true);
                    break;

                default:
                    SetInboundAction(FirewallAction.Block);
                    SetOutboundAction(FirewallAction.Block);
                    ToggleFirewall(true);
                    break;
            }
        }

        public void StartStatusListener()
        {
            StopStatusListener();
            statusCancelationTokenSource = new CancellationTokenSource();

            var cancellationToken = statusCancelationTokenSource.Token;
            Task.Factory.StartNew(async () =>
            {
                var currentStatus = new FirewallStatus();
                while (!cancellationToken.IsCancellationRequested)
                {
                    var status = Status;
                    if (!currentStatus.Equals(status))
                    {
                        StatusChanged?.Invoke(status);
                        currentStatus = status;
                    }
                    await Task.Delay((int)statusPullInterval.TotalMilliseconds, cancellationToken);
                }
            });
        }

        public void StopStatusListener()
        {
            statusCancelationTokenSource?.Cancel();
        }

        private void AddBlockAllOutboundRule()
        {
            var rule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            rule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            rule.Description = BlockAllOutboundRuleName;
            rule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            rule.Enabled = true;
            rule.InterfaceTypes = "All";
            rule.Grouping = FirewallGroupName;
            rule.Name = BlockAllOutboundRuleName;
            firewallPolicy.Rules.Add(rule);
        }

        private NET_FW_PROFILE_TYPE2_ CurrentNetworkProfileFlag()
            => Array.Find(profilesMap, p => IsCurrentProfile(p.Flag)).Flag;

        private void ExecuteActionForAllProfiles(Action<NET_FW_PROFILE_TYPE2_> action)
        {
            foreach (var p in profilesMap.Select(p => p.Flag))
            {
                action(p);
            }
        }

        private FirewallAction InboundAction(NET_FW_PROFILE_TYPE2_ profileFlag)
            => firewallPolicy.BlockAllInboundTraffic[profileFlag] || RuleExists(BlockAllInboundRuleName)
                ? FirewallAction.BlockAll
                : Array.Find(actionsMap, p => p.Flag == firewallPolicy.DefaultInboundAction[profileFlag]).Value;

        private bool IsCurrentProfile(NET_FW_PROFILE_TYPE2_ profileType)
            => (firewallPolicy.CurrentProfileTypes & (int)profileType) != 0;

        private bool IsFirewallEnabled()
        {
            var currentProfileFlag = CurrentNetworkProfileFlag();
            return currentProfileFlag != 0 && firewallPolicy.FirewallEnabled[currentProfileFlag];
        }

        private FirewallAction OutboundAction(NET_FW_PROFILE_TYPE2_ profileFlag)
            => RuleExists(BlockAllOutboundRuleName)
                ? FirewallAction.BlockAll
                : Array.Find(actionsMap, p => p.Flag == firewallPolicy.DefaultOutboundAction[profileFlag]).Value;

        private void RemoveRule(string name)
        {
            firewallPolicy.Rules.Remove(name);
        }

        private bool RuleExists(string name)
        {
            try
            {
                firewallPolicy.Rules.Item(name);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        private void SetInboundAction(FirewallAction action)
        {
            if (action == FirewallAction.BlockAll)
            {
                ExecuteActionForAllProfiles(p => firewallPolicy.BlockAllInboundTraffic[p] = true);
                return;
            }

            var actionFlag = Array.Find(actionsMap, p => p.Value == action).Flag;
            ExecuteActionForAllProfiles(p => firewallPolicy.DefaultInboundAction[p] = actionFlag);
        }

        private void SetOutboundAction(FirewallAction action)
        {
            if (action == FirewallAction.BlockAll)
            {
                AddBlockAllOutboundRule();
                action = FirewallAction.Allow;
            }

            var actionFlag = Array.Find(actionsMap, p => p.Value == action).Flag;
            ExecuteActionForAllProfiles(p => firewallPolicy.DefaultOutboundAction[p] = actionFlag);
        }

        private void ToggleFirewall(bool enable)
        {
            ExecuteActionForAllProfiles(p => ToggleFirewall(p, enable));
        }

        private void ToggleFirewall(NET_FW_PROFILE_TYPE2_ profile, bool enable)
        {
            firewallPolicy.FirewallEnabled[profile] = enable;
        }
    }
}