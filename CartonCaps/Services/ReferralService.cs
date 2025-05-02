using CartonCaps.Data;
using CartonCaps.Dtos;
using CartonCaps.IServices;

namespace CartonCaps.Services;

public class ReferralService : IReferralService
{
    public GetReferralResponse CreateReferral(int userId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<GetReferralResponse> GetReferrals(int userId)
    {
        var result =
            from referral in MockData.Referrals
            where referral.ReferrerId == userId
            join referredUser in MockData.Users on referral.ReferredId equals referredUser.Id
            select new GetReferralResponse(
                referral.Id,
                $"{referredUser.FirstName} {referredUser.LastName.FirstOrDefault()}.",
                referral.Status,
                referral.CompletedDate
            );
        return result;
    }

    public GetReferralResponse ValidateReferralCode(string referralCode)
    {
        var referral = MockData.Users.FirstOrDefault(r => r.ReferralCode == referralCode);
        if (referral == null)
            throw new ArgumentException("Invalid referral code.");

        return null;
    }
}
