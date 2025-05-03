using CartonCaps.Models;

namespace CartonCaps.IData;

public interface IDataProvider
{
    List<Referral> GetReferrals();
    List<User> GetUsers();
}
