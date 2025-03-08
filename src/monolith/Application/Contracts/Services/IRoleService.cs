namespace Application.Contracts.Services;

public interface IRoleService
{
    Task<Result<PagedResponse<IEnumerable<Role>>>> GetRolesAsync(
        RoleFilter filter,
        CancellationToken token = default);

    Task<Result<Role>> GetRoleDetailAsync(
        Guid roleId,
        CancellationToken token = default);

    Task<Result<Guid>> CreateRoleAsync(
        Role request,
        CancellationToken token = default);

    Task<Result<Guid>> UpdateRoleAsync(
        Guid roleId,
        Role request,
        CancellationToken token = default);

    Task<BaseResult> DeleteRoleAsync(
        Guid roleId,
        CancellationToken token = default);
}