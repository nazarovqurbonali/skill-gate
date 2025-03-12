namespace Application.Contracts.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<Result<bool>> CheckExistingUserAsync(RegisterRequest request, CancellationToken token = default);
    Task<Result<bool>> CheckToLoginAsync(LoginRequest request, CancellationToken token = default);
    Task<Result<ClaimsPrincipal>> GetUserByCredentialsAsync(LoginRequest request, CancellationToken token = default);
}