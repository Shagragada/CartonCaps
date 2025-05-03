using CartonCaps.Enums;
using CartonCaps.IData;
using CartonCaps.Models;
using CartonCaps.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CartonCaps.Test.Services;

public class ReferralServiceTest
{
    private readonly ReferralService _referralService;
    private readonly Mock<IMockData> _mockData = new();
    private readonly Mock<ILogger<ReferralService>> _logger = new();

    public ReferralServiceTest()
    {
        _referralService = new ReferralService(_mockData.Object, _logger.Object);
    }

    [Fact]
    public void GetReferrals_ReturnsReferralResponses_WhenUserHasReferrals()
    {
        // Arrange
        var userId = 1;
        _mockData
            .Setup(x => x.GetReferrals())
            .Returns(
                [
                    new()
                    {
                        Id = 1,
                        ReferrerId = 1,
                        ReferredId = 2,
                        Status = ReferralStatus.Completed,
                        CompletedDate = DateTime.UtcNow,
                    },
                ]
            );

        _mockData
            .Setup(x => x.GetUsers())
            .Returns(
                [
                    new()
                    {
                        Id = 2,
                        FirstName = "Jenny",
                        LastName = "Sackey",
                    },
                ]
            );

        // Act
        var result = _referralService.GetReferrals(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.Should().ContainSingle(r => r.ReferredName == "Jenny S.");
    }

    [Fact]
    public void ValidateReferralCode_ReturnsUser_WhenValidCodeIsProvided()
    {
        // Arrange
        var referralCode = "REF123";
        _mockData.Setup(x => x.GetUsers()).Returns([new() { Id = 1, ReferralCode = "REF123" }]);

        // Act
        var result = _referralService.ValidateReferralCode(referralCode);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReferralCode.Should().Be("REF123");
    }

    [Fact]
    public void ValidateReferralCode_ReturnsError_WhenCodeIsInvalid()
    {
        // Arrange
        _mockData.Setup(x => x.GetUsers()).Returns(new List<User>());

        // Act
        var result = _referralService.ValidateReferralCode("invalid-code");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Invalid referral code.");
    }

    [Fact]
    public void ValidateReferralCode_ReturnsError_WhenCodeIsEmpty()
    {
        // Act
        var result = _referralService.ValidateReferralCode("");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Referral code is required.");
    }
}
