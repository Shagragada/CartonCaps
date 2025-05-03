using Ardalis.Result;
using CartonCaps.Dtos;
using CartonCaps.Models;

namespace CartonCaps.IServices;

public interface IReferralService
{
    Result<List<GetReferralResponse>> GetReferrals(int userId);
    GetReferralResponse CreateReferral(int userId);
    Result<User> ValidateReferralCode(string referralCode);
}
