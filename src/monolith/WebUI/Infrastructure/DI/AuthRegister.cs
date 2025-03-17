namespace WebUI.Infrastructure.DI;

public static class AuthRegister
{
    public static WebApplicationBuilder AddAuthServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LogoutPath = "/identity/logout";
                options.LoginPath = "/identity/login";
                options.AccessDeniedPath = "/error/403";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.ClaimsIssuer = "skill-gate issuer";
                options.Cookie.Name = "skill-gate";
                options.Cookie.HttpOnly = true;
                options.SlidingExpiration = true;
            });
        return builder;
    }
}