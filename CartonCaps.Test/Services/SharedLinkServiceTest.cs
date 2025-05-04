using Ardalis.Result;
using CartonCaps.Enums;
using CartonCaps.IServices;
using CartonCaps.Models;
using CartonCaps.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CartonCaps.Test.Services;

public class SharedLinkServiceTest
{
    private readonly SharedLinkService _service;
    private readonly Mock<IReferralService> _referralService = new();
    private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _configuration = new();
    private readonly Mock<ILogger<SharedLinkService>> _logger = new();

    public SharedLinkServiceTest()
    {
        _service = new SharedLinkService(
            _logger.Object,
            _referralService.Object,
            _configuration.Object
        );
    }

    [Fact]
    public void GenerateSharedLink_ReturnsSuccess_WhenInputIsValid()
    {
        // Arrange
        var os = OsPlatform.Android;
        var code = "ABC123";
        var baseUrl = "app://android.livefront.com/referral";

        // Mock configuration for Android platform
        _configuration.Setup(c => c["ReferralLinks:Android"]).Returns(baseUrl);

        // Mock referral validation to be successful
        _referralService
            .Setup(r => r.ValidateReferralCode(code))
            .Returns(Result<User>.Success(new User { ReferralCode = code }));

        // Act
        var result = _service.GenerateSharedLink(os, code);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.referralLink.Should().Be($"{baseUrl}?referral_code={code}");
        result.Value.Email.Should().NotBeNull();
        result.Value.Sms.Should().NotBeNull();
    }

    [Fact]
    public void GenerateSharedLink_ReturnsError_WhenReferralCodeIsEmpty()
    {
        // Arrange
        var os = OsPlatform.iOS;

        // Mock config for iOS
        _configuration
            .Setup(c => c["ReferralLinks:iOS"])
            .Returns("app://ios.livefront.com/referral");

        // Act
        var result = _service.GenerateSharedLink(os, string.Empty);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Referral code is required.");
    }

    [Fact]
    public void GenerateSharedLink_ReturnsError_WhenReferralCodeIsInvalid()
    {
        // Arrange
        var os = OsPlatform.Web;
        var code = "INVALID";
        var error = "Invalid referral code.";
        _configuration
            .Setup(c => c["ReferralLinks:Web"])
            .Returns("https://web.livefront.com/referral");

        _referralService
            .Setup(r => r.ValidateReferralCode(code))
            .Returns(Result<User>.Error(error));

        // Act
        var result = _service.GenerateSharedLink(os, code);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }

    [Fact]
    public void GenerateSharedLink_ReturnsError_WhenPlatformConfigIsMissing()
    {
        // Arrange
        var os = OsPlatform.Android;
        var code = "ABC123";

        // Missing Android config (returns null)
        _configuration.Setup(c => c["ReferralLinks:Android"]).Returns(string.Empty);

        // Act
        var result = _service.GenerateSharedLink(os, code);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Invalid OS platform.");
    }
}
