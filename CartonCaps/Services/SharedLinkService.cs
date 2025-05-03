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
            // Determine the base URL based on the OS platform
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
}
