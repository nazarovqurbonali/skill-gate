namespace WebUI.Infrastructure.Middlewares;

public static class RegisterMiddlewares
{
    public static async Task<WebApplication> MapMiddlewares(this WebApplication app)
    {
        // {
        //     using IServiceScope scope = app.Services.CreateScope();
        //     Seeder seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
        //     await seeder.SeedAsync();
        // }
        
        app.UseExceptionHandler("/error/500"); 
        app.UseStatusCodePagesWithReExecute("/error/{0}");
        app.UseHttpLogging();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
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