using CartonCaps.Dtos;
using CartonCaps.Models;

namespace CartonCaps.IServices;

public interface IReferralService
{
    IEnumerable<GetReferralResponse> GetReferrals(int userId);
    GetReferralResponse CreateReferral(int userId);
    User? ValidateReferralCode(string referralCode);
}
