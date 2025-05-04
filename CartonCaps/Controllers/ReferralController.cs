using Ardalis.Result;
using CartonCaps.Dtos;
using CartonCaps.Extensions;
using CartonCaps.IServices;
using CartonCaps.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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

    [ProducesResponseType(typeof(Ok<List<GetReferralResponse>>), 200)]
    [ProducesResponseType(typeof(BadRequest), 400)]
    [HttpGet("get-referrals")]
    public IActionResult GetReferrals()
    {
        var result = _referralService.GetReferrals(User.GetUserId());
        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors);
    }

    [ProducesResponseType(typeof(Ok<User>), 200)]
    [ProducesResponseType(typeof(NotFound), 404)]
    [ProducesResponseType(typeof(BadRequest), 400)]
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
