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

public class ReferralControllerTests
{
    private readonly Mock<IReferralService> _referralService = new();
    private readonly ReferralController _controller;

    public ReferralControllerTests()
    {
        _controller = new ReferralController(_referralService.Object);
        SetUserWithClaims(_controller, 1); // Set default user ID
    }

    private void SetUserWithClaims(ReferralController controller, int userId)
    {
        var claims = new List<Claim> { new Claim("UserId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };
    }

    [Fact]
    public void GetReferrals_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var userId = 1;
        var expectedResult = new List<GetReferralResponse>
        {
            new GetReferralResponse(
                1,
                "John D.",
                ReferralStatus.Completed.ToString(),
                DateTime.UtcNow
            ),
        };

        _referralService
            .Setup(s => s.GetReferrals(userId))
            .Returns(Result<List<GetReferralResponse>>.Success(expectedResult));

        // Act
        var result = _controller.GetReferrals();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void GetReferrals_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var userId = 1;
        var errors = new[] { "Failed to retrieve referrals." };

        _referralService
            .Setup(s => s.GetReferrals(userId))
            .Returns(Result<List<GetReferralResponse>>.Error(errors.FirstOrDefault()));

        // Act
        var result = _controller.GetReferrals();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result as BadRequestObjectResult;
        badResult!.Value.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ValidateReferralCode_ReturnsOk_WhenCodeIsValid()
    {
        // Arrange
        var code = "ABC123";
        var user = new User { Id = 2, ReferralCode = code };

        _referralService
            .Setup(s => s.ValidateReferralCode(code))
            .Returns(Result<User>.Success(user));

        // Act
        var result = _controller.ValidateReferralCode(code);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public void ValidateReferralCode_ReturnsNotFound_WhenCodeIsNotFound()
    {
        // Arrange
        var code = "XYZ404";
        var errors = new[] { "Referral code not found." };

        _referralService
            .Setup(s => s.ValidateReferralCode(code))
            .Returns(Result<User>.NotFound(errors));

        // Act
        var result = _controller.ValidateReferralCode(code);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = result as NotFoundObjectResult;
        notFound!.Value.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ValidateReferralCode_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var code = "INVALID";
        var errors = new[] { "Invalid referral code." };

        _referralService
            .Setup(s => s.ValidateReferralCode(code))
            .Returns(Result<User>.Error(errors.FirstOrDefault()));

        // Act
        var result = _controller.ValidateReferralCode(code);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result as BadRequestObjectResult;
        badResult!.Value.Should().BeEquivalentTo(errors);
    }
}
