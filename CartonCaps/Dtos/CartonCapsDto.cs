using CartonCaps.MessageTemplate;

namespace CartonCaps.Dtos;

public record GetReferralResponse(
    int Id,
    string ReferredName,
    string Status,
    DateTime? CompletedDate
);

public record SharedLinkResponse(
    string referralLink,
    SharedMessageTemplate Email,
    SharedMessageTemplate Sms
);

public record ReferralDetectionRequest(string ReferralCode);

public record ReferralDetectionResponse(
    bool IsReferred,
    string? ReferralCode,
    int? ReferrerId,
    string? ReferredBy
);
