namespace WebUI.Infrastructure.DI;

public static class AuthRegister
{
    public static WebApplicationBuilder AddAuthService(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LogoutPath = "/auth/logout";
                options.LoginPath = "/auth/login";
                options.AccessDeniedPath = "/access-denied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.ClaimsIssuer = "skill-gate issuer";
                options.Cookie.Name = "skill-gate";
                options.Cookie.HttpOnly = true;
                options.SlidingExpiration = true;
            });
        return builder;
    }
}