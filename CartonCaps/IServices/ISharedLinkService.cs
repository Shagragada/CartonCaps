using Ardalis.Result;
using CartonCaps.Dtos;
using CartonCaps.Enums;

namespace CartonCaps.IServices;

public interface ISharedLinkService
{
    Result<SharedLinkResponse> GenerateSharedLink(OsPlatform osPlatform, string referralCode);
}
