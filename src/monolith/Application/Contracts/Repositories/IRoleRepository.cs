namespace Application.Contracts.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Result<Role?>> GetRoleByNameAsync(string roleName, CancellationToken token = default);
    Task<Result<bool>> CheckExistingRoleAsync(string roleName, CancellationToken token = default);
}