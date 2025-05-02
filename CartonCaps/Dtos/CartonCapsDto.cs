using CartonCaps.Enums;
using CartonCaps.MessageTemplate;

namespace CartonCaps.Dtos;

public record GetReferralResponse(
    int Id,
    string ReferredName,
    ReferralStatus Status,
    DateTime? CompletedDate
);

public record SharedLinkResponse(Sms SmsMessage, Email EmailMessage);
