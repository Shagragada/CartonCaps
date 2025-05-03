using CartonCaps.IData;
using CartonCaps.IServices;
using CartonCaps.Models;

namespace CartonCaps.Services;

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
}
