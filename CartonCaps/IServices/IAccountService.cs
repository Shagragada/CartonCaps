using CartonCaps.Dtos;
using CartonCaps.Models;

namespace CartonCaps.IServices;

public interface IAccountService
{
    User? GetUserByReferralCode(string referralCode);
    User CreateUser(CreateUserRequest request);
    bool RedeemReferralCode(string userEmail);
}
