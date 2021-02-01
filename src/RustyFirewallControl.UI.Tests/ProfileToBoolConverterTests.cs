using RustyFirewallControl.Common;
using RustyFirewallControl.UI.Converters;
using System.Windows.Data;
using Xunit;

namespace RustyFirewallControl.UI.Tests
{
    public class ProfileToBoolConverterTests
    {
        [Fact]
        public void ConvertsAFilterProfileToTrueWhenTheGivenIdMatchesTheProfile()
        {
            // Arrange
            var subject = new ProfileToBoolConverter();

            // Act
            var actual = (bool)subject.Convert(FilteringProfile.MediumFiltering, null, "medium", null);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void ConvertsAFilterProfileToFalseWhenTheGivenIdDoesNotMatchTheProfile()
        {
            // Arrange
            var subject = new ProfileToBoolConverter();

            // Act
            var actual = (bool)subject.Convert(FilteringProfile.MediumFiltering, null, "somthingElse", null);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void ConvertsToFalseWhenTheTheValueIsNotAFilteringProfile()
        {
            // Arrange
            var subject = new ProfileToBoolConverter();

            // Act
            var actual = (bool)subject.Convert("somethingElse", null, "somthingElse", null);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void ConvertsToFalseWhenTheGivenIdIsNotAString()
        {
            // Arrange
            var subject = new ProfileToBoolConverter();

            // Act
            var actual = (bool)subject.Convert(FilteringProfile.MediumFiltering, null, 4, null);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void ConvertsBackReturnsTheProfileWhenCalledWithTrueAndIdIsMatching()
        {
            // Arrange
            var subject = new ProfileToBoolConverter();

            // Act
            var actual = (FilteringProfile)subject.ConvertBack(true, null, "medium", null);

            // Assert
            Assert.Equal(FilteringProfile.MediumFiltering, actual);
        }

        [Fact]
        public void ConvertsBackReturnsNoFilteringProfileWhenCalledWithTrueAndTheIdIsNotMatching()
        {
            // Arrange
            var subject = new ProfileToBoolConverter();

            // Act
            var actual = (FilteringProfile)subject.ConvertBack(true, null, "somethigElse", null);

            // Assert
            Assert.Equal(FilteringProfile.NoFiltering, actual);
        }

        [Fact]
        public void ConvertBackReturnsBindingDoNothingWhenCalledWithFalse()
        {
            // Arrange
            var subject = new ProfileToBoolConverter();

            // Act
            var actual = subject.ConvertBack(false, null, "medium", null);

            // Assert
            Assert.Equal(Binding.DoNothing, actual);
        }
    }
}
