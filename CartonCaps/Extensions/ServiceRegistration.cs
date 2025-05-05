using System.Reflection;
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

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        services.AddScoped<IReferralService, ReferralService>();
        services.AddScoped<ISharedLinkService, SharedLinkService>();
        services.AddScoped<IDataProvider, FakeDataProvider>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IRedemptionService, RedemptionService>();
        services.AddSingleton<IMessageTemplateService>(provider => new MessageTemplateService(
            Path.Combine(Directory.GetCurrentDirectory(), "MessageTemplate")
        ));
    }
}
