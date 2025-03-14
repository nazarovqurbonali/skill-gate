namespace Infrastructure.ImplementationContract.Services;

public sealed class IdentityService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    ILogger<IdentityService> logger,
    IHttpContextAccessor accessor) : IIdentityService
{
    private HttpContext Context => accessor.HttpContext!;

    public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request,
        CancellationToken token = default)
    {
        try
        {
            Result<bool> checkExistingUser = await userRepository.CheckExistingUserAsync(request, token);
            if (!checkExistingUser.IsSuccess)
                return Result<RegisterResponse>.Failure(checkExistingUser.Error);
            if (checkExistingUser is { IsSuccess: true, Value: true })
                return Result<RegisterResponse>.Failure(
                    ResultPatternError.AlreadyExist(
                        "User already exists with this username or phone number or email address"));

            User newUser = new()
            {
                Email = request.EmailAddress,
                PhoneNumber = request.Phone,
                UserName = request.UserName,
                CreatedBy = accessor.GetId(),
                CreatedByIp = accessor.GetRemoteIpAddress(),
                PasswordHash = HashingUtility.ComputeSha256Hash(request.Password)
            };
            Result<int> result = await userRepository.AddAsync(newUser, token);
            if (!result.IsSuccess)
                return Result<RegisterResponse>.Failure(result.Error);

            Result<Role?> existingRole = await roleRepository.GetRoleByNameAsync(Roles.User, token);
            if (!existingRole.IsSuccess)
                return Result<RegisterResponse>.Failure(result.Error);
            UserRole newUserRole = new()
            {
                CreatedBy = accessor.GetId(),
                CreatedByIp = accessor.GetRemoteIpAddress(),
                RoleId = existingRole.Value?.Id
                         ?? throw new ArgumentException(nameof(existingRole.Value.Id)),
                UserId = newUser.Id
            };
            Result<int> resultInsertUserRole = await userRoleRepository.AddAsync(newUserRole, token);
            if (!resultInsertUserRole.IsSuccess)
                return Result<RegisterResponse>.Failure(resultInsertUserRole.Error);

            return Result<RegisterResponse>.Success(new(newUser.Id));
        }
        catch (Exception e)
        {
            return Result<RegisterResponse>.Failure(ResultPatternError.InternalServerError(e.Message));
        }
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken token = default)
    {
        try
        {
            Result<bool> result = await userRepository.CheckToLoginAsync(request, token);
            if (!result.IsSuccess)
                return Result<LoginResponse>.Failure(result.Error);
            Result<ClaimsPrincipal> resultOfClaims = await userRepository.GetUserByCredentialsAsync(request, token);
            if (!resultOfClaims.IsSuccess)
                return Result<LoginResponse>.Failure(resultOfClaims.Error);

            await Context.SignInAsync(AuthenticationSchemeDefaults.Cookies, resultOfClaims.Value);

            return Result<LoginResponse>.Success(
                new(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMinutes(30)));
        }
        catch (Exception e)
        {
            return Result<LoginResponse>.Failure(ResultPatternError.InternalServerError(e.Message));
        }
    }

    public async Task<BaseResult> LogoutAsync()
    {
        try
        {
            await Context.SignOutAsync();
            return BaseResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return BaseResult.Failure(ResultPatternError.InternalServerError(e.Message));
        }
    }
}