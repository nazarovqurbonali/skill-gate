namespace WebUI.Infrastructure.DI;

public static class RegisterServices
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.AddHttpService();
        builder.AddAuthService();
        builder.AddEmailService();
        builder.Services.AddProblemDetails();
        builder.Services.AddCustomServices();
        builder.Services.AddControllersWithViews();

        return builder;
    }
}