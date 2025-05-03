using CartonCaps.Enums;
using CartonCaps.IData;
using CartonCaps.Models;

namespace CartonCaps.Data;

public class FakeDataProvider : IDataProvider
{
    public List<Referral> GetReferrals() =>
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

    public List<User> GetUsers() =>
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
}
