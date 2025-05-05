using CartonCaps.Dtos;
using CartonCaps.IData;
using CartonCaps.IServices;
using CartonCaps.Models;

namespace CartonCaps.Services;

// Out of scope
public class AccountService : IAccountService
{
    private readonly IDataProvider _dataProvider;

    public AccountService(IDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public User? GetUserByReferralCode(string referralCode)
    {
        var user = _dataProvider
            .GetUsers()
            .FirstOrDefault(r =>
                r.ReferralCode.Equals(referralCode, StringComparison.OrdinalIgnoreCase)
            );
        return user;
    }

    // Out of scope
    public User CreateUser(CreateUserRequest request)
    {
        var newUser = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth,
            Zipcode = request.Zipcode,
            ReferralCode = $"REF{new Random().Next(1000, 9999)}",
        };

        _dataProvider.SaveUser(newUser);
        RedeemReferralCode(request.Email);
        return newUser;
    }

    // Out of scope
    public bool RedeemReferralCode(string userEmail)
    {
        var email = _dataProvider
            .GetCreatedAccounts()
            .FirstOrDefault(r => r.Contains(userEmail, StringComparison.OrdinalIgnoreCase));

        if (email == null)
        {
            //User is new, give reward
            _dataProvider.AddEmailToCreaetedAccount(userEmail);
            return true;
        }

        //User account was once created, deny reward
        return false;
    }
}
