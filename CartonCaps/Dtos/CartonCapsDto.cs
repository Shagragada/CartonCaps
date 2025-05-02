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
    SharedMessageTemplate email,
    SharedMessageTemplate sms
);
