using Ardalis.Result;
using CartonCaps.Controllers;
using CartonCaps.Dtos;
using CartonCaps.Enums;
using CartonCaps.IServices;
using CartonCaps.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartonCaps.Test.Controllers;

public class ReferralControllerTest
{
    private readonly Mock<IReferralService> _referralService = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly ReferralController _controller;

    public ReferralControllerTest()
    {
        _controller = new ReferralController(_referralService.Object, _currentUserService.Object);
    }

    [Fact]
    public void GetReferrals_ReturnsOk_WithReferralList_WhenSuccessful()
    {
        // Arrange
        var userId = 1;
        var referralList = new List<GetReferralResponse>
        {
            new GetReferralResponse(
                1,
                "John D.",
                ReferralStatus.Completed.ToString(),
                DateTime.UtcNow
            ),
        };

        _currentUserService.Setup(x => x.UserId).Returns(userId);
        _referralService
            .Setup(x => x.GetReferrals(userId))
            .Returns(Result<List<GetReferralResponse>>.Success(referralList));

        // Act
        var result = _controller.GetReferrals();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(referralList);
    }

    [Fact]
    public void GetReferrals_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var userId = 1;
        var error = "Error fetching referrals";

        _currentUserService.Setup(x => x.UserId).Returns(userId);
        _referralService
            .Setup(x => x.GetReferrals(userId))
            .Returns(Result<List<GetReferralResponse>>.Error(error));

        // Act
        var result = _controller.GetReferrals();

        // Assert
        var badResult = result as BadRequestObjectResult;
        badResult.Should().NotBeNull();
        badResult!.Value.Should().BeEquivalentTo(error);
    }

    [Fact]
    public void ValidateReferralCode_ReturnsOk_WhenCodeIsValid()
    {
        // Arrange
        var code = "ABC123";
        var user = new User { Id = 1, ReferralCode = code };

        _referralService
            .Setup(x => x.ValidateReferralCode(code))
            .Returns(Result<User>.Success(user));

        // Act
        var result = _controller.ValidateReferralCode(code);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public void ValidateReferralCode_ReturnsBadRequest_WhenCodeIsInvalid()
    {
        // Arrange
        var code = "INVALID";
        var error = "Invalid referral code.";

        _referralService
            .Setup(x => x.ValidateReferralCode(code))
            .Returns(Result<User>.Error(error));

        // Act
        var result = _controller.ValidateReferralCode(code);

        // Assert
        var badResult = result as BadRequestObjectResult;
        badResult.Should().NotBeNull();
        badResult!.Value.Should().BeEquivalentTo(error);
    }
}
