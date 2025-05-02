using CartonCaps.IServices;

namespace CartonCaps.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }

    public string? ReferralCode
    {
        get
        {
            var referralCodeClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(
                "ReferralCode"
            );
            return referralCodeClaim?.Value;
        }
    }
}
