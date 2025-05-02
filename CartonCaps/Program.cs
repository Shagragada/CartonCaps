using CartonCaps.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTheme(ScalarTheme.BluePlanet)
            .WithTitle("Carton Caps Documentation")
            .WithDefaultHttpClient(ScalarTarget.Swift, ScalarClient.Nsurlsession);
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
