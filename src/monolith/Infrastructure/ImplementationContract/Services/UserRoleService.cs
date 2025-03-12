namespace Infrastructure.ImplementationContract.Services;

public sealed class UserRoleService : IUserRoleService
{
    public async Task<Result<PagedResponse<IEnumerable<UserRole>>>> GetUserRolesAsync(UserRoleFilter filter,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<UserRole>> GetUserRoleDetailAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Guid>> CreateUserRoleAsync(UserRole request, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Guid>> UpdateUserRoleAsync(Guid id, UserRole request, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResult> DeleteUserRoleAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}