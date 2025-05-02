using Ardalis.Result;
using CartonCaps.Dtos;
using CartonCaps.IData;
using CartonCaps.IServices;
using CartonCaps.Models;

namespace CartonCaps.Services;

public class ReferralService : IReferralService
{
    private readonly IMockData _mockData;
    private readonly ILogger<ReferralService> _logger;

    public ReferralService(IMockData mockData, ILogger<ReferralService> logger)
    {
        _mockData = mockData;
        _logger = logger;
    }

    public GetReferralResponse CreateReferral(int userId)
    {
        throw new NotImplementedException();
    }

    public Result<IEnumerable<GetReferralResponse>> GetReferrals(int userId)
    {
        try
        {
            // Fetch referrals and users from mock data
            var referrals = _mockData.GetReferrals();
            var users = _mockData.GetUsers();

            // Gets referrals for the specified user
            var result =
                from referral in referrals
                where referral.ReferrerId == userId
                join referredUser in users on referral.ReferredId equals referredUser.Id
                select new GetReferralResponse(
                    referral.Id,
                    $"{referredUser.FirstName} {referredUser.LastName.FirstOrDefault()}.",
                    referral.Status.ToString(),
                    referral.CompletedDate
                );

            return Result<IEnumerable<GetReferralResponse>>.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving referrals for user {UserId}", userId);
            return Result<IEnumerable<GetReferralResponse>>.Error(
                "An error occurred while retrieving referrals."
            );
        }
    }

    public Result<User> ValidateReferralCode(string referralCode)
    {
        try
        {
            // Check for empty referral code
            if (string.IsNullOrWhiteSpace(referralCode))
                return Result<User>.Error("Referral code is required.");

            // Get user with the given referral code
            var user = _mockData
                .GetUsers()
                .FirstOrDefault(r =>
                    r.ReferralCode.Equals(referralCode, StringComparison.OrdinalIgnoreCase)
                );

            if (user == null)
                return Result<User>.Error("Invalid referral code.");

            return Result<User>.Success(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error validating referral code {ReferralCode}", referralCode);
            return Result<User>.Error("An error occurred while validating the referral code.");
        }
    }
}
