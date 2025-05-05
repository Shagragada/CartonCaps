using Ardalis.Result;
using CartonCaps.Dtos;

namespace CartonCaps.IServices;

public interface ISharedLinkService
{
    Result<SharedLinkResponse> GenerateSharedLink(
        SharedLinkRequest sharedLinkRequest,
        string referralCode
    );
}
