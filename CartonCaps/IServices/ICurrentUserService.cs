namespace CartonCaps.IServices;

public interface ICurrentUserService
{
    // Gets the current user's ID.
    int? UserId { get; }

    // Gets the current user's referral code.
    string? ReferralCode { get; }

    // Gets the current user's first name.
    string? FirstName { get; }
}
