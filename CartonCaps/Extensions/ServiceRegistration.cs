using CartonCaps.Data;
using CartonCaps.IData;
using CartonCaps.IServices;
using CartonCaps.Services;

namespace CartonCaps.Extensions;

public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();

        services.AddScoped<IReferralService, ReferralService>();
        services.AddScoped<ISharedLinkService, SharedLinkService>();
        services.AddScoped<IDataProvider, FakeDataProvider>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddSingleton<ITemplateService>(provider => new TemplateService(
            Path.Combine(Directory.GetCurrentDirectory(), "MessageTemplate")
        ));
    }
}
