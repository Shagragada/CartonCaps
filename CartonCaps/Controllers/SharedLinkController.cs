using CartonCaps.Dtos;
using CartonCaps.Enums;
using CartonCaps.Extensions;
using CartonCaps.IServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SharedLinkController : ControllerBase
{
    private readonly ISharedLinkService _sharedLinkService;
    private readonly IReferralService _referralService;

    public SharedLinkController(
        ISharedLinkService sharedLinkService,
        IReferralService referralService
    )
    {
        _sharedLinkService = sharedLinkService;
        _referralService = referralService;
    }

    [ProducesResponseType(typeof(Ok<SharedLinkResponse>), 200)]
    [ProducesResponseType(typeof(BadRequest), 400)]
    [HttpPost("generate-shared-link")]
    public IActionResult GenerateSharedLink(OsPlatform osPlatform)
    {
        var result = _sharedLinkService.GenerateSharedLink(osPlatform, User.GetReferralCode());
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Errors);
    }

    [ProducesResponseType(typeof(Ok<ReferralDetectionResponse>), 200)]
    [ProducesResponseType(typeof(NotFound), 404)]
    [ProducesResponseType(typeof(BadRequest), 400)]
    [HttpPost("detect-referral")]
    public IActionResult DetectReferral([FromQuery] string referralCode)
    {
        // If ReferralCode is null or empty, user is not referred
        if (string.IsNullOrWhiteSpace(referralCode))
            return NotFound(
                new ReferralDetectionResponse(
                    IsReferred: false,
                    ReferralCode: null,
                    ReferrerId: null,
                    ReferredBy: null
                )
            );

        // Check if referral code belongs to a valid user
        var validationResult = _referralService.ValidateReferralCode(referralCode);

        // If user is not valid, return response indicating no referral
        if (!validationResult.IsSuccess)
        {
            return NotFound(
                new ReferralDetectionResponse(
                    IsReferred: false,
                    ReferralCode: null,
                    ReferrerId: null,
                    ReferredBy: null
                )
            );
        }

        // Return response indicating referral was detected
        return Ok(
            new ReferralDetectionResponse(
                IsReferred: true,
                ReferralCode: referralCode,
                ReferrerId: validationResult.Value.Id,
                ReferredBy: $"{validationResult.Value.FirstName} {validationResult.Value.LastName.FirstOrDefault()}."
            )
        );
    }
}
