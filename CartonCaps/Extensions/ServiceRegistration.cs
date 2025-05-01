namespace CartonCaps.Extensions;

public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
    }
}
