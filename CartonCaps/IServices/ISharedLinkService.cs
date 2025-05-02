using CartonCaps.Dtos;

namespace CartonCaps.IServices;

public interface ISharedLinkService
{
    SharedLinkResponse GenerateSharedLink(string baseUrl);
    bool ValidateSharedLink(string token);
}
