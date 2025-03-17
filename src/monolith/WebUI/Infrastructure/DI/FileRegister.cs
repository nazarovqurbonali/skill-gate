namespace WebUI.Infrastructure.DI;

public static class FileRegister
{
    public static WebApplicationBuilder AddFileServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<FileSettings>(builder.Configuration.GetSection(ConfigNames.FileSettings));
        builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<FileSettings>>());
        builder.Services.AddSingleton<IFileService>(sp =>
        {
            FileSettings fileSettings = sp.GetRequiredService<FileSettings>();
            ILogger<FileService> logger = sp.GetRequiredService<ILogger<FileService>>();
            IWebHostEnvironment webHostEnvironment = sp.GetRequiredService<IWebHostEnvironment>();

            return new FileService(webHostEnvironment.WebRootPath, Options.Create(fileSettings), logger);
        });

        return builder;
    }
}