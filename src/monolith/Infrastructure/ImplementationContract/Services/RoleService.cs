namespace Infrastructure.ImplementationContract.Services;

public sealed class RoleService(
    IRoleRepository roleRepository,
    ILogger<RoleService> logger) : IRoleService
{
    public async Task<Result<PagedResponse<IEnumerable<Role>>>> GetRolesAsync(RoleFilter filter,
        CancellationToken token = default)
    {
        try
        {
            Result<IEnumerable<Role>> roles = await roleRepository.GetAllAsync(filter, token);
            if (!roles.IsSuccess)
                return Result<PagedResponse<IEnumerable<Role>>>.Failure(roles.Error);

            PagedResponse<IEnumerable<Role>> response =
                PagedResponse<IEnumerable<Role>>
                    .Create(filter.PageSize, filter.PageNumber, roles.Value!.Count(), roles.Value);
            return Result<PagedResponse<IEnumerable<Role>>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute query.");
            return Result<PagedResponse<IEnumerable<Role>>>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<Role>> GetRoleDetailAsync(Guid roleId, CancellationToken token = default)
    {
        try
        {
            Result<Role?> result = await roleRepository.GetByIdAsync(roleId, token);
            if (!result.IsSuccess)
                return Result<Role>.Failure(result.Error);

            return Result<Role>.Success(result.Value);
        }
        catch (Exception ex)
        {
            return Result<Role>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<Guid>> CreateRoleAsync(Role request, CancellationToken token = default)
    {
        try
        {
            Result<bool> checkExisting = await roleRepository.CheckExistingRoleAsync(request.Name, token);
            if (checkExisting is { IsSuccess: true, Value: true })
                return Result<Guid>.Failure(ResultPatternError.BadRequest("Role already exists."));

            Result<int> result = await roleRepository.AddAsync(request, token);
            return result.IsSuccess ? Result<Guid>.Success(request.Id) : Result<Guid>.Failure(result.Error);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<Guid>> UpdateRoleAsync(Guid roleId, Role request, CancellationToken token = default)
    {
        try
        {
            Result<Role?> role = await roleRepository.GetByIdAsync(roleId, token);
            if (!role.IsSuccess)
                return Result<Guid>.Failure(role.Error);

            if (role.Value!.Name != request.Name)
            {
                Result<bool> checkExisting = await roleRepository.CheckExistingRoleAsync(request.Name, token);
                if (checkExisting is { IsSuccess: true, Value: true })
                    return Result<Guid>.Failure(ResultPatternError.Conflict("Role name already exists."));
            }

            request.Id = roleId;
            Result<int> result = await roleRepository.UpdateAsync(request, token);
            return result.IsSuccess ? Result<Guid>.Success(roleId) : Result<Guid>.Failure(result.Error);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<BaseResult> DeleteRoleAsync(Guid roleId, CancellationToken token = default)
    {
        try
        {
            Result<int> result = await roleRepository.DeleteAsync(roleId, token);
            return result.IsSuccess ? BaseResult.Success() : BaseResult.Failure(result.Error);
        }
        catch (Exception ex)
        {
            return BaseResult.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }
}