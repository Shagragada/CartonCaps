using Ardalis.Result;
using CartonCaps.Dtos;

namespace CartonCaps.IServices;

public interface ISharedLinkService
{
    Result<SharedLinkResponse> GenerateSharedLink(string baseUrl);
    Result<bool> ValidateSharedLink(string token);
}
