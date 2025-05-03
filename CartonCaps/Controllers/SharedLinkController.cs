using CartonCaps.Dtos;
using CartonCaps.Enums;
using CartonCaps.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SharedLinkController : ControllerBase
{
    private readonly ISharedLinkService _sharedLinkService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IReferralService _referralService;

    public SharedLinkController(
        ISharedLinkService sharedLinkService,
        ICurrentUserService currentUserService,
        IReferralService referralService
    )
    {
        _sharedLinkService = sharedLinkService;
        _currentUserService = currentUserService;
        _referralService = referralService;
    }

    /// <summary>
    /// Generates a shareable referral link for the specified platform
    /// </summary>
    /// <param name="osPlatform">
    /// The target platform for the shared link:
    /// 0 (iOS), 1 (Android), or 2 (Web)
    /// </param>
    /// <returns>A shareable link object</returns>
    [HttpPost("generate-shared-link")]
    public IActionResult GenerateSharedLink(OsPlatform osPlatform)
    {
        var result = _sharedLinkService.GenerateSharedLink(
            osPlatform,
            _currentUserService.ReferralCode
        );
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Errors);
    }

    [HttpPost("detect-referral")]
    public IActionResult DetectReferral([FromBody] ReferralDetectionRequest request)
    {
        // If ReferralCode is null or empty, user is not referred
        if (string.IsNullOrWhiteSpace(request.ReferralCode))
            return Ok(
                new ReferralDetectionResponse(
                    IsReferred: false,
                    ReferralCode: null,
                    ReferrerId: null,
                    ReferredBy: null
                )
            );

        // Check if referral code belongs to a valid user
        var validationResult = _referralService.ValidateReferralCode(request.ReferralCode);

        // If user is not valid, return response indicating no referral
        if (!validationResult.IsSuccess)
        {
            return Ok(
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
                ReferralCode: request.ReferralCode,
                ReferrerId: validationResult.Value.Id,
                ReferredBy: $"{validationResult.Value.FirstName} {validationResult.Value.LastName.FirstOrDefault()}."
            )
        );
    }
}
