using System.Security.Claims;

namespace CartonCaps.Extensions;

public static class UserExtension
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst("UserId");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return 0; // Default or error value if no claim found
    }

    public static string GetReferralCode(this ClaimsPrincipal user)
    {
        var referralCodeClaim = user.FindFirst("ReferralCode");
        return referralCodeClaim?.Value ?? "Ref123"; // Default referral code if no claim found
    }
}
