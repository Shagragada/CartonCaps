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
    private readonly IConfiguration _configuration;

    public SharedLinkController(
        ISharedLinkService sharedLinkService,
        ICurrentUserService currentUserService,
        IConfiguration configuration
    )
    {
        _sharedLinkService = sharedLinkService;
        _currentUserService = currentUserService;
        _configuration = configuration;
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

    [HttpPost("validate-shared-link")]
    public IActionResult ValidateSharedLink([FromBody] string token)
    {
        var result = _sharedLinkService.ValidateSharedLink(token);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Errors);
    }
}
