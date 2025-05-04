using CartonCaps.Models;

namespace CartonCaps.IData;

public interface IDataProvider
{
    List<Referral> GetReferrals();
    List<User> GetUsers();
    void SaveUser(User user);
    List<string> GetCreatedAccounts();
    void AddEmailToCreaetedAccount(string email);
}
