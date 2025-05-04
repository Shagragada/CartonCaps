using CartonCaps.Enums;
using CartonCaps.IData;
using CartonCaps.Models;

namespace CartonCaps.Data;

public class FakeDataProvider : IDataProvider
{
    private readonly List<User> _users;
    private readonly List<Referral> _referrals;

    private readonly List<string> _CreatedAccounts;

    public FakeDataProvider()
    {
        _users =
        [
            new User
            {
                Id = 1,
                FirstName = "Jenny",
                LastName = "Sackey",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Zipcode = "12345",
                ReferralCode = "REF123",
            },
            new User
            {
                Id = 2,
                FirstName = "Archer",
                LastName = "Kumson",
                DateOfBirth = new DateOnly(1992, 2, 2),
                Zipcode = "12345",
                ReferralCode = "REF456",
            },
            new User
            {
                Id = 3,
                FirstName = "Helen",
                LastName = "Yaa",
                DateOfBirth = new DateOnly(1995, 3, 3),
                Zipcode = "12345",
                ReferralCode = "REF789",
            },
        ];

        _referrals =
        [
            new Referral
            {
                Id = 1,
                ReferredId = 2,
                ReferrerId = 1,
                Status = ReferralStatus.Pending,
                CompletedDate = DateTime.UtcNow.AddDays(-1),
            },
            new Referral
            {
                Id = 2,
                ReferredId = 3,
                ReferrerId = 1,
                Status = ReferralStatus.Completed,
                CompletedDate = DateTime.UtcNow.AddDays(-4),
            },
            new Referral
            {
                Id = 3,
                ReferredId = 4,
                ReferrerId = 2,
                Status = ReferralStatus.Completed,
                CompletedDate = DateTime.UtcNow.AddDays(-1),
            },
        ];

        _CreatedAccounts =
        [
            "spider.man@livefront.com",
            "bat.man@livefront.com",
            "super.man@livefront.com",
        ];
    }

    public List<User> GetUsers() => _users;

    public List<Referral> GetReferrals() => _referrals;

    /// Returns a list of all account emails created
    /// It will not be deleted even if a user account is deleted
    /// This way, if a user redeems a referral code, deletes their account
    /// and attempt to create a new account, they can be prevented
    public List<string> GetCreatedAccounts() => _CreatedAccounts;

    public void SaveUser(User user)
    {
        _users.Add(user);
    }

    //Adds user email to created account
    public void AddEmailToCreaetedAccount(string email)
    {
        _CreatedAccounts.Add(email);
    }
}
