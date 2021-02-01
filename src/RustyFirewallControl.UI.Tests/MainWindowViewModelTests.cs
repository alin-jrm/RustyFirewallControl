using Moq;
using RustyFirewallControl.Common;
using RustyFirewallControl.UI.ViewModels;
using Xunit;

namespace RustyFirewallControl.UI.Tests
{
    public class MainWindowViewModelTests
    {
        [Fact]
        public void RetrievesTheFirewallStatusWhenInitialized()
        {
            // Arrange
            var client = new Mock<IFirewallClient>();
            var status = new FirewallStatus
            {
                IsEnabled = true,
                InboundAction = FirewallAction.Block,
                OutboundAction = FirewallAction.Allow,
                NetworkProfile = NetworkProfile.Private
            };
            client.Setup(f => f.Status).Returns(status);
            var subject = new MainWindowViewModel(client.Object);

            // Act
            subject.Initialize();

            // Assert
            Assert.Equal(status, subject.FirewallStatus);
        }

        [Fact]
        public void TheFilteringProfileIsRetriedWhenInitialized()
        {
            // Arrange
            var client = new Mock<IFirewallClient>();
            var status = new FirewallStatus
            {
                IsEnabled = true,
                InboundAction = FirewallAction.Block,
                OutboundAction = FirewallAction.Allow,
                NetworkProfile = NetworkProfile.Private
            };
            client.Setup(f => f.Status).Returns(status);
            var subject = new MainWindowViewModel(client.Object);

            // Act
            subject.Initialize();

            // Assert
            Assert.Equal(status.FilteringProfile, subject.FilteringProfile);
        }

        [Fact]
        public void RegistersItselfForFirewallStatusChangeEventsOnLoaded()
        {
            // Arrange
            var client = new Mock<IFirewallClient>();
            var status = new FirewallStatus
            {
                IsEnabled = true,
            };
            client.Setup(f => f.Status).Returns(status);
            var subject = new MainWindowViewModel(client.Object);
            subject.Initialize();

            // Act
            subject.OnLoaded();
            client.Raise(f => f.StatusChanged += null, new FirewallStatus { IsEnabled = false });

            // Assert
            Assert.False(subject.FirewallStatus.IsEnabled);
        }

        [Fact]
        public void ChangeProfileCommandUseTheFirewallClientToUpdateTheProfile()
        {
            // Arrange
            var client = new Mock<IFirewallClient>();
            var status = new FirewallStatus
            {
                IsEnabled = true,
            };
            client.Setup(f => f.Status).Returns(status);
            var subject = new MainWindowViewModel(client.Object);
            subject.Initialize();

            // Act
            subject.ChangeProfileCommand.Execute(FilteringProfile.LowFiltering);

            // Assert
            client.Verify(f => f.SetFilteringProfile(FilteringProfile.LowFiltering));
        }
    }
}
