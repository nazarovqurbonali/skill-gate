using Application.Contracts.Services;
using Infrastructure.DataAccess.Seed;
using Infrastructure.ImplementationContract.Services;

namespace WebUI.Infrastructure.DI;

public static class CustomServiceRegister
{
    public static WebApplicationBuilder AddCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<Seeder>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IUserRoleService, UserRoleService>();
       
        return builder;
    }
}