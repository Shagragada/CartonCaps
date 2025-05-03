using Ardalis.Result;
using CartonCaps.Dtos;
using CartonCaps.IData;
using CartonCaps.IServices;
using CartonCaps.Models;

namespace CartonCaps.Services;

public class ReferralService : IReferralService
{
    private readonly IDataProvider _providerData;
    private readonly ILogger<ReferralService> _logger;
    private readonly IAccountService _accountService;

    public ReferralService(
        IDataProvider dataProvider,
        ILogger<ReferralService> logger,
        IAccountService accountService
    )
    {
        _providerData = dataProvider;
        _logger = logger;
        _accountService = accountService;
    }

    public Result<List<GetReferralResponse>> GetReferrals(int userId)
    {
        try
        {
            // Fetch referrals and users from mock data
            var referrals = _providerData.GetReferrals();
            var users = _providerData.GetUsers();

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

            return Result<List<GetReferralResponse>>.Success([.. result]);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving referrals for user {UserId}", userId);
            return Result<List<GetReferralResponse>>.Error(
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
            var user = _accountService.GetUserByReferralCode(referralCode);

            //User not found for the provided referral code
            if (user == null)
                return Result<User>.NotFound("User not found for the provided referral code.");

            return Result<User>.Success(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error validating referral code {ReferralCode}", referralCode);
            return Result<User>.Error("An error occurred while validating the referral code.");
        }
    }
}
