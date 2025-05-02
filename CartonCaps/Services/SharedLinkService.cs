using System.Web;
using Ardalis.Result;
using CartonCaps.Dtos;
using CartonCaps.IData;
using CartonCaps.IServices;
using CartonCaps.MessageTemplate;

namespace CartonCaps.Services;

public class SharedLinkService : ISharedLinkService
{
    private readonly IMockData _mockData;
    private readonly ILogger<SharedLinkService> _logger;
    private readonly IReferralService _referralService;

    public SharedLinkService(
        IMockData mockData,
        ILogger<SharedLinkService> logger,
        IReferralService referralService
    )
    {
        _mockData = mockData;
        _logger = logger;
        _referralService = referralService;
    }

    public Result<SharedLinkResponse> GenerateSharedLink(string baseUrl, string referralCode)
    {
        try
        {
            // Check for empty base Url
            if (string.IsNullOrWhiteSpace(baseUrl))
                return Result<SharedLinkResponse>.Error("Base URL is required.");

            // Check for empty referral code
            if (string.IsNullOrWhiteSpace(referralCode))
                return Result<SharedLinkResponse>.Error("Referral code is required.");

            // Check referral code validity
            var validationResult = _referralService.ValidateReferralCode(referralCode);
            if (!validationResult.IsSuccess)
                return Result<SharedLinkResponse>.Error("Invalid referral code.");

            var referralLink = $"{baseUrl}?referral_code={referralCode}";

            var smsMessage = new Sms(referralLink);
            var emailMessage = new Email(referralLink);

            return Result<SharedLinkResponse>.Success(
                new SharedLinkResponse(smsMessage, emailMessage)
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error generating shared link.");
            return Result<SharedLinkResponse>.Error(
                "An error occurred while generating the shared link."
            );
        }
    }

    public Result<bool> ValidateSharedLink(string referralLink)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(referralLink))
                return Result<bool>.Error("Referral link is required.");

            var uri = new Uri(referralLink);
            var query = HttpUtility.ParseQueryString(uri.Query);
            var referralCode = query.Get("referral_code");

            if (string.IsNullOrWhiteSpace(referralCode))
                return false;
            var user = _mockData.GetUsers().FirstOrDefault(r => r.ReferralCode == referralCode);

            return user != null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error validating shared link {ReferralLink}.", referralLink);
            return Result<bool>.Error("An error occurred while validating the shared link.");
        }
    }
}
