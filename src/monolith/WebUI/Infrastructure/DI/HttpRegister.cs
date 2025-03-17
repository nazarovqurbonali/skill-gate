namespace WebUI.Infrastructure.DI;

public static class HttpRegister
{
    public static WebApplicationBuilder AddHttpServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpLogging(options => { options.LoggingFields = HttpLoggingFields.All; });

        return builder;
    }
}