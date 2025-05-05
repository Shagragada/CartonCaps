using Ardalis.Result;
using CartonCaps.Dtos;
using CartonCaps.Enums;
using CartonCaps.IServices;

namespace CartonCaps.Services;

public class SharedLinkService : ISharedLinkService
{
    private readonly ILogger<SharedLinkService> _logger;
    private readonly IReferralService _referralService;
    private readonly IConfiguration _configuration;
    private readonly IMessageTemplateService _templateService;

    public SharedLinkService(
        ILogger<SharedLinkService> logger,
        IReferralService referralService,
        IConfiguration configuration,
        IMessageTemplateService templateService
    )
    {
        _logger = logger;
        _referralService = referralService;
        _configuration = configuration;
        _templateService = templateService;
    }

    /// Generates a shared link based on the provided request and referral code.
    public Result<SharedLinkResponse> GenerateSharedLink(
        SharedLinkRequest sharedLinkRequest,
        string referralCode
    )
    {
        try
        {
            // Determine the base URL based on the OS platform
            var baseUrl = sharedLinkRequest.OsPlatform switch
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
            var emailMessage = _templateService.CreateEmail(referralLink);
            var smsMessage = _templateService.CreateSms(referralLink);
            var messageResult = sharedLinkRequest.SharingMedium switch
            {
                SharingMedium.Email => emailMessage,
                SharingMedium.SMS => smsMessage,
                _ => null,
            };

            return Result<SharedLinkResponse>.Success(
                new SharedLinkResponse(referralLink, messageResult!)
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
