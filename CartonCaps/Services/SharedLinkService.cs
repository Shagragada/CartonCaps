using System.Web;
using Ardalis.Result;
using CartonCaps.Dtos;
using CartonCaps.Enums;
using CartonCaps.IData;
using CartonCaps.IServices;
using CartonCaps.MessageTemplate;

namespace CartonCaps.Services;

public class SharedLinkService : ISharedLinkService
{
    private readonly IMockData _mockData;
    private readonly ILogger<SharedLinkService> _logger;
    private readonly IReferralService _referralService;
    private readonly IConfiguration _configuration;

    public SharedLinkService(
        IMockData mockData,
        ILogger<SharedLinkService> logger,
        IReferralService referralService,
        IConfiguration configuration
    )
    {
        _mockData = mockData;
        _logger = logger;
        _referralService = referralService;
        _configuration = configuration;
    }

    public Result<SharedLinkResponse> GenerateSharedLink(OsPlatform osPlatform, string referralCode)
    {
        try
        {
            var baseUrl = osPlatform switch
            {
                OsPlatform.Web => _configuration["ReferralLinks:Web"],
                OsPlatform.Android => _configuration["ReferralLinks:Android"],
                OsPlatform.iOS => _configuration["ReferralLinks:iOS"],
                _ => null,
            };

            // Check for empty base Url
            if (string.IsNullOrWhiteSpace(baseUrl))
                return Result<SharedLinkResponse>.Error("Invalid OS platform.");

            // Check for empty referral code
            if (string.IsNullOrWhiteSpace(referralCode))
                return Result<SharedLinkResponse>.Error("Referral code is required.");

            // Check referral code validity
            var validationResult = _referralService.ValidateReferralCode(referralCode);
            if (!validationResult.IsSuccess)
                return Result<SharedLinkResponse>.Error("Invalid referral code.");

            var referralLink = $"{baseUrl}?referral_code={referralCode}";

            // Generate SMS and Email messages using the referral link
            var smsMessage = SmsTemplate.Create(referralLink);
            var emailMessage = EmailTemplate.Create(referralLink);

            return Result<SharedLinkResponse>.Success(
                new SharedLinkResponse(referralLink, emailMessage, smsMessage)
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

    //Todo: Update this method to return a complex type
    public Result<bool> ValidateSharedLink(string referralLink)
    {
        try
        {
            // Check for empty referral link
            if (string.IsNullOrWhiteSpace(referralLink))
                return Result<bool>.Error("Referral link is required.");

            // Parse the referral link to extract the referral code
            var uri = new Uri(referralLink);
            var query = HttpUtility.ParseQueryString(uri.Query);
            var referralCode = query.Get("referral_code");

            if (string.IsNullOrWhiteSpace(referralCode))
                return false;

            // Check if the referral code is associated with a user
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
