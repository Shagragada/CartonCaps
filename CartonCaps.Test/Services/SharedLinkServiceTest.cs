using Ardalis.Result;
using CartonCaps.Dtos;
using CartonCaps.Enums;
using CartonCaps.IServices;
using CartonCaps.MessageTemplate;
using CartonCaps.Models;
using CartonCaps.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace CartonCaps.Test.Services;

public class SharedLinkServiceTest
{
    private readonly SharedLinkService _service;
    private readonly Mock<IReferralService> _referralService = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<ILogger<SharedLinkService>> _logger = new();
    private readonly Mock<IMessageTemplateService> _templateService = new();

    public SharedLinkServiceTest()
    {
        _service = new SharedLinkService(
            _logger.Object,
            _referralService.Object,
            _configuration.Object,
            _templateService.Object
        );
    }

    [Theory]
    [InlineData([OsPlatform.Android, SharingMedium.Email])]
    [InlineData([OsPlatform.Android, SharingMedium.SMS])]
    [InlineData([OsPlatform.iOS, SharingMedium.Email])]
    [InlineData([OsPlatform.iOS, SharingMedium.SMS])]
    [InlineData([OsPlatform.Web, SharingMedium.Email])]
    [InlineData([OsPlatform.Web, SharingMedium.SMS])]
    public void GenerateSharedLink_ReturnsSuccess_WhenInputIsValid(
        OsPlatform platform,
        SharingMedium medium
    )
    {
        // Arrange
        var request = new SharedLinkRequest(platform, medium);
        var code = "ABC123";
        var baseUrl = platform switch
        {
            OsPlatform.Android => "app://android.livefront.com/referral",
            OsPlatform.iOS => "app://ios.livefront.com/referral",
            OsPlatform.Web => "https://web.livefront.com/referral",
            _ => null,
        };

        var config = platform switch
        {
            OsPlatform.Android => "Android",
            OsPlatform.iOS => "iOS",
            OsPlatform.Web => "Web",
            _ => null,
        };

        var expectedLink = $"{baseUrl}?referral_code={code}";

        _configuration.Setup(c => c[$"ReferralLinks:{config}"]).Returns(baseUrl);

        _referralService
            .Setup(r => r.ValidateReferralCode(code))
            .Returns(Result<User>.Success(new User { ReferralCode = code }));

        var template = new SharedMessageTemplate("subject", "body");
        _templateService.Setup(t => t.CreateEmail(expectedLink)).Returns(template);
        _templateService.Setup(t => t.CreateSms(expectedLink)).Returns(template);

        // Act
        var result = _service.GenerateSharedLink(request, code);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ReferralLink.Should().Be(expectedLink);
        result.Value.Message.Should().Be(template);
    }

    [Fact]
    public void GenerateSharedLink_ReturnsError_WhenReferralCodeIsEmpty()
    {
        // Arrange
        var request = new SharedLinkRequest(OsPlatform.iOS, SharingMedium.SMS);
        _configuration
            .Setup(c => c["ReferralLinks:iOS"])
            .Returns("app://ios.livefront.com/referral");

        // Act
        var result = _service.GenerateSharedLink(request, string.Empty);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Referral code is required.");
    }

    [Fact]
    public void GenerateSharedLink_ReturnsError_WhenReferralCodeIsInvalid()
    {
        // Arrange
        var request = new SharedLinkRequest(OsPlatform.Web, SharingMedium.SMS);
        var code = "INVALID";
        var error = "Invalid referral code.";

        _configuration
            .Setup(c => c["ReferralLinks:Web"])
            .Returns("https://web.livefront.com/referral");
        _referralService
            .Setup(r => r.ValidateReferralCode(code))
            .Returns(Result<User>.Error(error));

        // Act
        var result = _service.GenerateSharedLink(request, code);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }

    [Fact]
    public void GenerateSharedLink_ReturnsError_WhenPlatformConfigIsMissing()
    {
        // Arrange
        var request = new SharedLinkRequest(OsPlatform.Android, SharingMedium.Email);
        var code = "ABC123";

        _configuration.Setup(c => c["ReferralLinks:Android"]).Returns(string.Empty);

        // Act
        var result = _service.GenerateSharedLink(request, code);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Invalid OS platform.");
    }
}
