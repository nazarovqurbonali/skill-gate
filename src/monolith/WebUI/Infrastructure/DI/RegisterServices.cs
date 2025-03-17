namespace WebUI.Infrastructure.DI;

public static class RegisterServices
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.AddFileServices();
        builder.AddHttpServices();
        builder.AddAuthServices();
        builder.AddEmailServices();
        builder.Services.AddProblemDetails();
        builder.Services.AddCustomServices();
        builder.Services.AddControllersWithViews();

        return builder;
    }
}