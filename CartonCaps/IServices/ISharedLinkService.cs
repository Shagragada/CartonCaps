using Ardalis.Result;
using CartonCaps.Dtos;

namespace CartonCaps.IServices;

public interface ISharedLinkService
{
    Result<SharedLinkResponse> GenerateSharedLink(string baseUrl, string referralCode);
    Result<bool> ValidateSharedLink(string token);
}
