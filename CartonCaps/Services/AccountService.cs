using CartonCaps.Dtos;
using CartonCaps.IData;
using CartonCaps.IServices;
using CartonCaps.Models;

namespace CartonCaps.Services;

public class AccountService : IAccountService
{
    private readonly IDataProvider _dataProvider;
    private readonly IReferralService _referralService;

    public AccountService(IDataProvider dataProvider, IReferralService referralService)
    {
        _dataProvider = dataProvider;
        _referralService = referralService;
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
        _referralService.RedeemReferralCode(request.Email);
        return newUser;
    }
}
