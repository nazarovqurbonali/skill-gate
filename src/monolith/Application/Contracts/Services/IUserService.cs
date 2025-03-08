namespace Application.Contracts.Services;

public interface IUserService
{
    Task<Result<PagedResponse<IEnumerable<User>>>> GetUsersAsync(
        UserFilter filter,
        CancellationToken token = default);

    Task<Result<User>> GetByIdForUser(
        Guid userId,
        CancellationToken token = default);

    Task<Result<User>> GetByIdForSelf(
        CancellationToken token = default);

    Task<Result<Guid>> UpdateProfileAsync(
        User request,
        CancellationToken token = default);
}