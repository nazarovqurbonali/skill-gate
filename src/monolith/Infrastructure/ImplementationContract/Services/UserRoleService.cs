namespace Infrastructure.ImplementationContract.Services;

public sealed class UserRoleService(
    IUserRoleRepository userRoleRepository,
    ILogger<UserRoleService> logger) : IUserRoleService
{
    public async Task<Result<PagedResponse<IEnumerable<UserRole>>>> GetUserRolesAsync(UserRoleFilter filter,
        CancellationToken token = default)
    {
        try
        {
            Result<IEnumerable<UserRole>> userRoles = await userRoleRepository.GetAllAsync(filter, token);
            if (!userRoles.IsSuccess)
                return Result<PagedResponse<IEnumerable<UserRole>>>.Failure(userRoles.Error);

            PagedResponse<IEnumerable<UserRole>> response =
                PagedResponse<IEnumerable<UserRole>>
                    .Create(filter.PageSize, filter.PageNumber, userRoles.Value!.Count(), userRoles.Value);
            return Result<PagedResponse<IEnumerable<UserRole>>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute query.");
            return Result<PagedResponse<IEnumerable<UserRole>>>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<UserRole>> GetUserRoleDetailAsync(Guid id, CancellationToken token = default)
    {
        try
        {
            Result<UserRole?> result = await userRoleRepository.GetByIdAsync(id, token);
            if (!result.IsSuccess)
                return Result<UserRole>.Failure(result.Error);

            return Result<UserRole>.Success(result.Value);
        }
        catch (Exception ex)
        {
            return Result<UserRole>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<Guid>> CreateUserRoleAsync(UserRole request, CancellationToken token = default)
    {
        try
        {
            Result<int> result = await userRoleRepository.AddAsync(request, token);
            return result.IsSuccess ? Result<Guid>.Success(request.Id) : Result<Guid>.Failure(result.Error);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<Guid>> UpdateUserRoleAsync(Guid id, UserRole request, CancellationToken token = default)
    {
        try
        {
            request.Id = id;
            Result<int> result = await userRoleRepository.UpdateAsync(request, token);
            return result.IsSuccess ? Result<Guid>.Success(id) : Result<Guid>.Failure(result.Error);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<BaseResult> DeleteUserRoleAsync(Guid id, CancellationToken token = default)
    {
        try
        {
            Result<int> result = await userRoleRepository.DeleteAsync(id, token);
            return result.IsSuccess ? BaseResult.Success() : BaseResult.Failure(result.Error);
        }
        catch (Exception ex)
        {
            return BaseResult.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }
}