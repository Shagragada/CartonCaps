using CartonCaps.Enums;
using CartonCaps.MessageTemplate;

namespace CartonCaps.Dtos;

public record GetReferralResponse(
    int Id,
    string ReferredName,
    string Status,
    DateTime? CompletedDate
);

public record SharedLinkRequest(OsPlatform OsPlatform, SharingMedium SharingMedium);

public record SharedLinkResponse(string ReferralLink, SharedMessageTemplate Message);

public record ReferralDetectionRequest(string ReferralCode);

public record ReferralDetectionResponse(
    bool IsReferred,
    string? ReferralCode,
    int? ReferrerId,
    string? ReferredBy
);

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    DateOnly DateOfBirth,
    string Zipcode
);
