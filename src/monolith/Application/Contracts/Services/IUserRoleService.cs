namespace Application.Contracts.Services;

public interface IUserRoleService
{
    Task<Result<PagedResponse<IEnumerable<UserRole>>>> GetUserRolesAsync(
        UserRoleFilter filter,
        CancellationToken token = default);

    Task<Result<UserRole>> GetUserRoleDetailAsync(
        Guid id,
        CancellationToken token = default);

    Task<Result<Guid>> CreateUserRoleAsync(
        UserRole request,
        CancellationToken token = default);

    Task<Result<Guid>> UpdateUserRoleAsync(
        Guid id,
        UserRole request,
        CancellationToken token = default);

    Task<BaseResult> DeleteUserRoleAsync(
        Guid id,
        CancellationToken token = default);
}