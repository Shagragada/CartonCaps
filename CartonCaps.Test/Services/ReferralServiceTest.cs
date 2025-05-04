using CartonCaps.Enums;
using CartonCaps.IData;
using CartonCaps.IServices;
using CartonCaps.Models;
using CartonCaps.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CartonCaps.Test.Services;

public class ReferralServiceTest
{
    private readonly ReferralService _referralService;
    private readonly Mock<IDataProvider> _mockData = new();
    private readonly Mock<ILogger<ReferralService>> _logger = new();
    private readonly Mock<IAccountService> _mockAccountService = new();

    public ReferralServiceTest()
    {
        _referralService = new ReferralService(
            _mockData.Object,
            _logger.Object,
            _mockAccountService.Object
        );
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
        _mockAccountService
            .Setup(x => x.GetUserByReferralCode(referralCode))
            .Returns(
                new User
                {
                    Id = 1,
                    ReferralCode = referralCode,
                    FirstName = "Michael",
                    LastName = "Live",
                    Zipcode = "12345",
                }
            );

        // Act
        var result = _referralService.ValidateReferralCode(referralCode);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReferralCode.Should().Be(referralCode);
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
        result.Errors.Should().Contain("User not found for the provided referral code.");
    }

    [Fact]
    public void ValidateReferralCode_ReturnsError_WhenCodeIsEmpty()
    {
        // Act
        var result = _referralService.ValidateReferralCode(string.Empty);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Referral code is required.");
    }
}
