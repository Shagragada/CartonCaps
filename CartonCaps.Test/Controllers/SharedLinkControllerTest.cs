using System.Security.Claims;
using Ardalis.Result;
using CartonCaps.Controllers;
using CartonCaps.Dtos;
using CartonCaps.Enums;
using CartonCaps.IServices;
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
        SetUserWithReferralClaim(_controller, "ABC123");
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

    [Fact]
    public void GenerateSharedLink_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var osPlatform = OsPlatform.Android;
        var expectedLink = new SharedLinkResponse("https://app.link/abc", null, null);

        _sharedLinkService
            .Setup(s => s.GenerateSharedLink(osPlatform, "ABC123"))
            .Returns(Result<SharedLinkResponse>.Success(expectedLink));

        // Act
        var result = _controller.GenerateSharedLink(osPlatform);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedLink);
    }

    [Fact]
    public void GenerateSharedLink_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var osPlatform = OsPlatform.iOS;
        var errors = new[] { "Failed to generate link." };

        _sharedLinkService
            .Setup(s => s.GenerateSharedLink(osPlatform, "ABC123"))
            .Returns(Result<SharedLinkResponse>.Error(errors.FirstOrDefault()));

        // Act
        var result = _controller.GenerateSharedLink(osPlatform);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result as BadRequestObjectResult;
        badResult!.Value.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void DetectReferral_ReturnsNotFound_WhenCodeIsNullOrEmpty()
    {
        // Arrange
        var request = new ReferralDetectionRequest(ReferralCode: string.Empty);

        // Act
        var result = _controller.DetectReferral(request);

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
                    ReferredBy: null
                )
            );
    }

    [Fact]
    public void DetectReferral_ReturnsNotFound_WhenValidationFails()
    {
        // Arrange
        var code = "INVALID";
        var request = new ReferralDetectionRequest(ReferralCode: code);

        _referralService
            .Setup(s => s.ValidateReferralCode(code))
            .Returns(Result<User>.Error("Invalid code"));

        // Act
        var result = _controller.DetectReferral(request);

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
                    ReferredBy: null
                )
            );
    }

    [Fact]
    public void DetectReferral_ReturnsOk_WhenReferralDetected()
    {
        // Arrange
        var code = "VALID123";
        var request = new ReferralDetectionRequest(ReferralCode: code);

        var user = new User
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
        };

        _referralService
            .Setup(s => s.ValidateReferralCode(code))
            .Returns(Result<User>.Success(user));

        // Act
        var result = _controller.DetectReferral(request);

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
                    ReferredBy: "Jane S."
                )
            );
    }
}
