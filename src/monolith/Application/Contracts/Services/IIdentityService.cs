namespace Application.Contracts.Services;

public interface IIdentityService
{
    Task<Result<RegisterResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken token = default);

    Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken token = default);

    Task<BaseResult> LogoutAsync();
}