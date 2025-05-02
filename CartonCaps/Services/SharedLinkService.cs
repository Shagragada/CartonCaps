using System.Web;
using CartonCaps.Data;
using CartonCaps.Dtos;
using CartonCaps.IServices;
using CartonCaps.MessageTemplate;

namespace CartonCaps.Services;

public class SharedLinkService : ISharedLinkService
{
    private readonly ICurrentUserService _currentUserService;

    public SharedLinkService(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public SharedLinkResponse GenerateSharedLink(string baseUrl)
    {
        var referralCode = _currentUserService.ReferralCode;
        var referralLink = $"{baseUrl}?referral_code={referralCode}";

        var smsMessage = new Sms(referralLink);
        var emailMessage = new Email(referralLink);

        return new SharedLinkResponse(smsMessage, emailMessage);
    }

    public bool ValidateSharedLink(string referralLink)
    {
        if (string.IsNullOrWhiteSpace(referralLink))
            return false;

        var uri = new Uri(referralLink);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var referralCode = query.Get("referral_code");

        if (string.IsNullOrWhiteSpace(referralCode))
            return false;
        var user = MockData.Users.FirstOrDefault(r => r.ReferralCode == referralCode);

        return user != null;
    }
}
