namespace Infrastructure.ImplementationContract.Services;

public sealed class RoleService : IRoleService
{
    public async Task<Result<PagedResponse<IEnumerable<Role>>>> GetRolesAsync(RoleFilter filter,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Role>> GetRoleDetailAsync(Guid roleId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Guid>> CreateRoleAsync(Role request, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Guid>> UpdateRoleAsync(Guid roleId, Role request, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResult> DeleteRoleAsync(Guid roleId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}