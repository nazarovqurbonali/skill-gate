namespace WebUI.Infrastructure.Middlewares;

public static class RegisterMiddlewares
{
    public static async Task<WebApplication> MapMiddlewares(this WebApplication app)
    {
        {
            using IServiceScope scope = app.Services.CreateScope();
            Seeder seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
            await seeder.SeedAsync();
        }

        app.UseHttpLogging();
        app.UseHttpsRedirection();
        app.UseExceptionHandler("/error");
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        await app.RunAsync();

        return app;
    }
}