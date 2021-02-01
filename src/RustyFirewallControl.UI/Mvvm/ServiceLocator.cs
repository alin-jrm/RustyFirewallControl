using System;
using Microsoft.Extensions.DependencyInjection;
using RustyFirewallControl.Common;

namespace RustyFirewallControl.UI.Mvvm
{
    public class ServiceLocator
    {
        private static readonly Lazy<ServiceLocator> LazyInstance
            = new Lazy<ServiceLocator>(() => new ServiceLocator());

        public ServiceLocator()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            Provider = services.BuildServiceProvider();
        }

        public static IServiceProvider Instance
            => LazyInstance.Value.Provider;

        private IServiceProvider Provider { get; }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IFirewallClient>(new Client.FirewallClient());
        }
    }
}
