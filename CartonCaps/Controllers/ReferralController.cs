using CartonCaps.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReferralController : ControllerBase
{
    private readonly IReferralService _referralService;
    private readonly ICurrentUserService _currentUserService;

    public ReferralController(
        IReferralService referralService,
        ICurrentUserService currentUserService
    )
    {
        _referralService = referralService;
        _currentUserService = currentUserService;
    }

    [HttpGet("get-referrals")]
    public IActionResult GetReferrals()
    {
        var result = _referralService.GetReferrals(_currentUserService.UserId);
        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors);
    }

    [HttpPost("{userId}/create")]
    public IActionResult CreateReferral(int userId)
    {
        var response = _referralService.CreateReferral(userId);
        return Ok(response);
    }

    [HttpPost("validate-code")]
    public IActionResult ValidateReferralCode([FromQuery] string referralCode)
    {
        var result = _referralService.ValidateReferralCode(referralCode);
        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors);
    }
}
