using CartonCaps.IData;
using CartonCaps.IServices;

namespace CartonCaps.Services;

// Out of scope
public class RedemptionService : IRedemptionService
{
    private readonly IDataProvider _dataProvider;

    public RedemptionService(IDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

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
