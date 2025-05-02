using CartonCaps.Data;
using CartonCaps.Dtos;
using CartonCaps.IData;
using CartonCaps.IServices;
using CartonCaps.Models;

namespace CartonCaps.Services;

public class ReferralService : IReferralService
{
    private readonly IMockData _mockData;

    public ReferralService(IMockData mockData)
    {
        _mockData = mockData;
    }

    public GetReferralResponse CreateReferral(int userId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<GetReferralResponse> GetReferrals(int userId)
    {
        var referrals = _mockData.GetReferrals();
        var users = _mockData.GetUsers();

        var result =
            from referral in referrals
            where referral.ReferrerId == userId
            join referredUser in users on referral.ReferredId equals referredUser.Id
            select new GetReferralResponse(
                referral.Id,
                $"{referredUser.FirstName} {referredUser.LastName.FirstOrDefault()}.",
                referral.Status,
                referral.CompletedDate
            );
        return result;
    }

    public User? ValidateReferralCode(string referralCode)
    {
        if (string.IsNullOrWhiteSpace(referralCode))
            return null;
        var user = _mockData.GetUsers().FirstOrDefault(r => r.ReferralCode == referralCode);
        return user;
    }
}
