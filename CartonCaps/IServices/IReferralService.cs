using CartonCaps.Dtos;

namespace CartonCaps.IServices;

public interface IReferralService
{
    IEnumerable<GetReferralResponse> GetReferrals(int userId);
    GetReferralResponse CreateReferral(int userId);
    GetReferralResponse ValidateReferralCode(string referralCode);
}
