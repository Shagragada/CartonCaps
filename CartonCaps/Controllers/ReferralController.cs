using Ardalis.Result;
using CartonCaps.Extensions;
using CartonCaps.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReferralController : ControllerBase
{
    private readonly IReferralService _referralService;

    public ReferralController(IReferralService referralService)
    {
        _referralService = referralService;
    }

    [HttpGet("get-referrals")]
    public IActionResult GetReferrals()
    {
        var result = _referralService.GetReferrals(User.GetUserId());
        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors);
    }

    [HttpPost("validate-code")]
    public IActionResult ValidateReferralCode([FromQuery] string referralCode)
    {
        var result = _referralService.ValidateReferralCode(referralCode);

        if (result.IsSuccess)
            return Ok(result.Value);

        if (result.IsNotFound())
            return NotFound(result.Errors);

        return BadRequest(result.Errors);
    }
}
