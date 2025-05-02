using CartonCaps.Models;

namespace CartonCaps.IData;

public interface IMockData
{
    List<Referral> GetReferrals();
    List<User> GetUsers();
}
