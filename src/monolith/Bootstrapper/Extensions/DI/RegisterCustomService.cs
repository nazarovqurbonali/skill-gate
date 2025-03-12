using Application.Contracts.Services;
using Infrastructure.DataAccess.Seed;
using Application.Contracts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.ImplementationContract.Services;
using Infrastructure.ImplementationContract.Repositories;

namespace Bootstrapper.Extensions.DI;

public static class RegisterCustomService
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<Seeder>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IIdentityService, IdentityService>();
        return services;
    }
}