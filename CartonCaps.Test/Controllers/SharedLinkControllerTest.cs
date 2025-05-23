using System.Security.Claims;
using Ardalis.Result;
using CartonCaps.Controllers;
using CartonCaps.Dtos;
using CartonCaps.Enums;
using CartonCaps.IServices;
using CartonCaps.MessageTemplate;
using CartonCaps.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartonCaps.Test.Controllers;

public class SharedLinkControllerTest
{
    private readonly Mock<ISharedLinkService> _sharedLinkService = new();
    private readonly Mock<IReferralService> _referralService = new();
    private readonly SharedLinkController _controller;

    public SharedLinkControllerTest()
    {
        _controller = new SharedLinkController(_sharedLinkService.Object, _referralService.Object);
        SetUserWithReferralClaim(_controller, "REF123");
    }

    private static void SetUserWithReferralClaim(
        SharedLinkController controller,
        string referralCode
    )
    {
        var claims = new List<Claim> { new Claim("ReferralCode", referralCode) };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims)),
            },
        };
    }

    [Theory]
    [InlineData([OsPlatform.Android, SharingMedium.Email])]
    [InlineData([OsPlatform.Android, SharingMedium.SMS])]
    [InlineData([OsPlatform.iOS, SharingMedium.Email])]
    [InlineData([OsPlatform.iOS, SharingMedium.SMS])]
    [InlineData([OsPlatform.Web, SharingMedium.Email])]
    [InlineData([OsPlatform.Web, SharingMedium.SMS])]
    public void GenerateSharedLink_ReturnsOk_WhenServiceSucceeds(
        OsPlatform platform,
        SharingMedium medium
    )
    {
        // Arrange
        var request = new SharedLinkRequest(platform, medium);
        var baseUrl = platform switch
        {
            OsPlatform.Android => "app://android.livefront.com/referral",
            OsPlatform.iOS => "app://ios.livefront.com/referral",
            OsPlatform.Web => "https://web.livefront.com/referral",
            _ => null,
        };
        var referralCode = "REF123";
        var expectedResponse = new SharedLinkResponse(
            $"{baseUrl}?referral_code={referralCode}",
            new SharedMessageTemplate("Subject", "Message")
        );

        _sharedLinkService
            .Setup(s => s.GenerateSharedLink(request, referralCode))
            .Returns(Result<SharedLinkResponse>.Success(expectedResponse));

        // Act
        var result = _controller.GenerateSharedLink(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public void GenerateSharedLink_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var request = new SharedLinkRequest(OsPlatform.iOS, SharingMedium.Email);
        var errors = new[] { "Failed to generate link." };

        _sharedLinkService
            .Setup(s => s.GenerateSharedLink(request, "REF123"))
            .Returns(Result<SharedLinkResponse>.Error(errors.First()));

        // Act
        var result = _controller.GenerateSharedLink(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result as BadRequestObjectResult;
        badResult.Should().NotBeNull();
        badResult!.Value.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void DetectReferral_ReturnsNotFound_WhenCodeIsNullOrEmpty()
    {
        // Act
        var result = _controller.DetectReferral(string.Empty);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = result as NotFoundObjectResult;
        notFound!
            .Value.Should()
            .BeEquivalentTo(
                new ReferralDetectionResponse(
                    IsReferred: false,
                    ReferralCode: null,
                    ReferrerId: null,
                    ReferrerName: null
                )
            );
    }

    [Fact]
    public void DetectReferral_ReturnsNotFound_WhenValidationFails()
    {
        // Arrange
        var code = "INVALID";

        _referralService
            .Setup(s => s.ValidateReferralCode(code))
            .Returns(Result<User>.Error("Invalid code"));

        // Act
        var result = _controller.DetectReferral(code);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = result as NotFoundObjectResult;
        notFound!
            .Value.Should()
            .BeEquivalentTo(
                new ReferralDetectionResponse(
                    IsReferred: false,
                    ReferralCode: null,
                    ReferrerId: null,
                    ReferrerName: null
                )
            );
    }

    [Fact]
    public void DetectReferral_ReturnsOk_WhenReferralDetected()
    {
        // Arrange
        var code = "VALID123";
        var user = new User
        {
            Id = 2,
            FirstName = "Spider",
            LastName = "Man",
        };

        _referralService
            .Setup(s => s.ValidateReferralCode(code))
            .Returns(Result<User>.Success(user));

        // Act
        var result = _controller.DetectReferral(code);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!
            .Value.Should()
            .BeEquivalentTo(
                new ReferralDetectionResponse(
                    IsReferred: true,
                    ReferralCode: code,
                    ReferrerId: 2,
                    ReferrerName: "Spider M."
                )
            );
    }
}
