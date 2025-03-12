namespace Infrastructure.ImplementationContract.Services;

public sealed class UserService(
    IUserRepository userRepository,
    IHttpContextAccessor accessor,
    ILogger<UserService> logger) : IUserService
{
    public async Task<Result<PagedResponse<IEnumerable<User>>>> GetUsersAsync(UserFilter filter,
        CancellationToken token = default)
    {
        try
        {
            Result<IEnumerable<User>> users = await userRepository.GetAllAsync(filter, token);
            if (!users.IsSuccess)
                return Result<PagedResponse<IEnumerable<User>>>.Failure(users.Error);

            PagedResponse<IEnumerable<User>> response =
                PagedResponse<IEnumerable<User>>
                    .Create(filter.PageSize, filter.PageNumber, users.Value!.Count(), users.Value);
            return Result<PagedResponse<IEnumerable<User>>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute query.");
            return Result<PagedResponse<IEnumerable<User>>>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<User>> GetByIdForUser(Guid userId, CancellationToken token = default)
    {
        try
        {
            Result<User?> result = await userRepository.GetByIdAsync(userId, token);
            if (!result.IsSuccess)
                return Result<User>.Failure(result.Error);

            return Result<User>.Success(result.Value);
        }
        catch (Exception ex)
        {
            return Result<User>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<User>> GetByIdForSelf(CancellationToken token = default)
    {
        try
        {
            Result<User?> result = await userRepository.GetByIdAsync((Guid)accessor.GetId()!, token);
            if (!result.IsSuccess)
                return Result<User>.Failure(result.Error);

            return Result<User>.Success(result.Value);
        }
        catch (Exception ex)
        {
            return Result<User>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<Guid>> UpdateProfileAsync(User request, CancellationToken token = default)
    {
        try
        {
            Result<User?> result = await userRepository.GetByIdAsync(request.Id, token);
            if (!result.IsSuccess)
                return Result<Guid>.Failure(result.Error);
            Result<bool> resultOfChecking = await userRepository.CheckToUniqueUserAsync(request, token);
            if (!resultOfChecking.IsSuccess)
                return Result<Guid>.Failure(resultOfChecking.Error);
            Result<int> resultUpdate = await userRepository.UpdateAsync(request, token);
            return result.IsSuccess
                ? Result<Guid>.Success(request.Id)
                : Result<Guid>.Failure(resultUpdate.Error);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }
}