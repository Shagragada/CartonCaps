using CartonCaps.Data;
using CartonCaps.IData;
using CartonCaps.IServices;
using CartonCaps.Services;
using Microsoft.OpenApi.Models;

namespace CartonCaps.Extensions;

public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddSwaggerGen();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "CartonCaps API",
                    Version = "v1",
                    Description = "Referral system API documentation",
                }
            );
        });

        services.AddScoped<IReferralService, ReferralService>();
        services.AddScoped<ISharedLinkService, SharedLinkService>();
        services.AddScoped<IDataProvider, FakeDataProvider>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddSingleton<ITemplateService>(provider => new TemplateService(
            Path.Combine(Directory.GetCurrentDirectory(), "MessageTemplate")
        ));
    }
}
