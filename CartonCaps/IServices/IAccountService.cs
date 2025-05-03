using CartonCaps.Models;

namespace CartonCaps.IServices;

public interface IAccountService
{
    User? GetUserByReferralCode(string referralCode);
}
