namespace Application.Contracts.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<Result<IEnumerable<User>>> GetAllAsync(UserFilter filter, CancellationToken token = default);

    Task<Result<bool>> CheckExistingUserAsync(RegisterRequest request, CancellationToken token = default);
    Task<Result<bool>> CheckToLoginAsync(LoginRequest request, CancellationToken token = default);
    Task<Result<ClaimsPrincipal>> GetUserByCredentialsAsync(LoginRequest request, CancellationToken token = default);
    Task<Result<bool>> CheckToUniqueUserAsync(User request, CancellationToken token=default);
}