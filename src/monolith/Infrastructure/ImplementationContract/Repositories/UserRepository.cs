namespace Infrastructure.ImplementationContract.Repositories;

public sealed class UserRepository(
    IConfiguration config,
    ILogger<UserRepository> logger) : IUserRepository
{
    private readonly string _connectionString =
        config.GetConnectionString(ConfigNames.Npgsql)
        ?? throw new ArgumentNullException($"Connection string '{ConfigNames.Npgsql}' is missing in configuration.");

    public async Task<Result<int>> AddAsync(User entity, CancellationToken token = default) =>
        await ExecuteNonQueryTransactionAsync(UserNpgsqlCommands.InsertUser, cmd
            => cmd.AddUserParameters(entity), token);

    public async Task<Result<int>> AddRangeAsync(ICollection<User> entities, CancellationToken token = default)
    {
        if (!entities.Any())
            return Result<int>.Failure(ResultPatternError.BadRequest("No users to add."));

        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(token);

        await using NpgsqlCommand cmd = new(UserNpgsqlCommands.InsertUser, connection, transaction);
        try
        {
            foreach (User entity in entities)
            {
                cmd.Parameters.Clear();
                cmd.AddUserParameters(entity);
                await cmd.ExecuteNonQueryAsync(token);
            }

            await transaction.CommitAsync(token);
            return Result<int>.Success(entities.Count);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);
            logger.LogError(ex, "Failed to add users.");
            return Result<int>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<int>> UpdateAsync(User value, CancellationToken token = default) =>
        await ExecuteNonQueryTransactionAsync(UserNpgsqlCommands.UpdateUser,
            cmd => cmd.AddUpdateUserParameters(value), token);

    public async Task<Result<int>> UpdateAsync(Guid id, User value, CancellationToken token = default)
    {
        value.Id = id;
        return await UpdateAsync(value, token);
    }

    public async Task<Result<int>> DeleteAsync(Guid id, CancellationToken token = default)
        => await ExecuteNonQueryTransactionAsync(UserNpgsqlCommands.DeleteUserById,
            cmd => cmd.Parameters.AddWithValue("@Id", id), token);

    public async Task<Result<int>> DeleteAsync(User value, CancellationToken token = default)
        => await DeleteAsync(value.Id, token);

    public async Task<Result<User?>> GetByIdAsync(Guid id, CancellationToken token = default)
        => await ExecuteQuerySingleAsync(UserNpgsqlCommands.GetUserById, cmd => cmd.Parameters.AddWithValue("@Id", id),
            token);

    public async Task<Result<IEnumerable<User>>> GetAllAsync(CancellationToken token = default)
        => await ExecuteQueryListAsync(UserNpgsqlCommands.GetAllUsers, _ => { }, token);

    public async Task<Result<IEnumerable<User>>> GetAllAsync(UserFilter filter, CancellationToken token = default)
    {
        string query = UserNpgsqlCommands.GetAllUsers;
        List<string> conditions = new List<string>();
        List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

        void AddCondition(string column, string paramName, string? value, bool useLike = true)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            if (useLike)
            {
                conditions.Add($"{column} ILIKE @{paramName}");
                parameters.Add(new NpgsqlParameter(paramName, $"%{value}%"));
            }
            else
            {
                conditions.Add($"{column} = @{paramName}");
                parameters.Add(new NpgsqlParameter(paramName, value));
            }
        }

        AddCondition("first_name", "FirstName", filter.FirstName);
        AddCondition("last_name", "LastName", filter.LastName);
        AddCondition("email", "Email", filter.Email, useLike: false);
        AddCondition("phone_number", "PhoneNumber", filter.PhoneNumber, useLike: false);
        AddCondition("user_name", "UserName", filter.UserName, useLike: false);

        if (conditions.Any())
            query += " WHERE " + string.Join(" AND ", conditions);

        return await ExecuteQueryListAsync(query,
            (cmd) => { cmd.Parameters.AddRange(parameters.ToArray()); }, token);
    }


    public async Task<Result<bool>> CheckExistingUserAsync(RegisterRequest request, CancellationToken token = default)
    {
        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlCommand command = new(UserNpgsqlCommands.CheckExistingUser, connection);
        try
        {
            command.Parameters.AddWithValue("@Username", request.UserName);
            command.Parameters.AddWithValue("@Phone", request.EmailAddress);
            command.Parameters.AddWithValue("@Email", request.EmailAddress);

            bool result = Convert.ToBoolean(await command.ExecuteScalarAsync(token));
            return Result<bool>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute query.");
            return Result<bool>.Failure(ResultPatternError.InternalServerError(e.Message));
        }
    }

    public async Task<Result<bool>> CheckToLoginAsync(LoginRequest request, CancellationToken token = default)
    {
        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlCommand command = new(UserNpgsqlCommands.CheckToLoin, connection);
        try
        {
            command.Parameters.AddWithValue("@Login", request.Login);
            command.Parameters.AddWithValue("@Password", HashingUtility.ComputeSha256Hash(request.Password));

            bool result = Convert.ToBoolean(await command.ExecuteScalarAsync(token));
            return result
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(ResultPatternError.NotFound("User with Login and Password not found"));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute query.");
            return Result<bool>.Failure(ResultPatternError.InternalServerError(e.Message));
        }
    }


    private async Task<Result<int>> ExecuteNonQueryTransactionAsync(string query,
        Action<NpgsqlCommand> configureCommand, CancellationToken token)
    {
        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(token);

        try
        {
            await using NpgsqlCommand command = new(query, connection, transaction);
            configureCommand(command);

            int rowsAffected = await command.ExecuteNonQueryAsync(token);
            if (rowsAffected == 0)
            {
                await transaction.RollbackAsync(token);
                return Result<int>.Failure(ResultPatternError.InternalServerError("Operation failed."));
            }

            await transaction.CommitAsync(token);
            return Result<int>.Success(rowsAffected);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);
            logger.LogError(ex, "Database operation failed.");
            return Result<int>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    private async Task<Result<IEnumerable<User>>> ExecuteQueryListAsync(string query,
        Action<NpgsqlCommand> configureCommand, CancellationToken token)
    {
        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlCommand command = new(query, connection);
        configureCommand(command);

        try
        {
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(token);
            List<User> users = [];
            while (await reader.ReadAsync(token))
            {
                users.Add(reader.MapUser());
            }

            return Result<IEnumerable<User>>.Success(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute query.");
            return Result<IEnumerable<User>>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    private async Task<Result<User?>> ExecuteQuerySingleAsync(string query, Action<NpgsqlCommand> configureCommand,
        CancellationToken token = default)
    {
        Result<IEnumerable<User>> result = await ExecuteQueryListAsync(query, configureCommand, token);
        
        if (result.IsSuccess)
            return Result<User?>.Success(result.Value?.FirstOrDefault());
        if (result.Value != null && !result.Value.Any())
            return Result<User?>.Failure(ResultPatternError.NotFound());
        
        return Result<User?>.Failure(result.Error);
    }


    public async Task<Result<ClaimsPrincipal>> GetUserByCredentialsAsync(LoginRequest request,
        CancellationToken token = default)
    {
        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlCommand command = new(UserNpgsqlCommands.GetUserWithRolesByCredentials, connection);

        try
        {
            command.Parameters.AddWithValue("@Login", request.Login);
            command.Parameters.AddWithValue("@Password", HashingUtility.ComputeSha256Hash(request.Password));

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(token);

            User? user = null;
            List<string> roles = [];

            while (await reader.ReadAsync(token))
            {
                user ??= new User
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    FirstName = reader.IsDBNull(reader.GetOrdinal("first_name"))
                        ? string.Empty 
                        : reader.GetString(reader.GetOrdinal("first_name")),
                    LastName = reader.IsDBNull(reader.GetOrdinal("last_name"))
                        ? string.Empty 
                        : reader.GetString(reader.GetOrdinal("last_name")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email"))
                        ? string.Empty 
                        : reader.GetString(reader.GetOrdinal("email")),
                    PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number"))
                        ? string.Empty 
                        : reader.GetString(reader.GetOrdinal("phone_number")),
                    UserName = reader.IsDBNull(reader.GetOrdinal("user_name"))
                        ? string.Empty 
                        : reader.GetString(reader.GetOrdinal("user_name"))
                };


                if (!reader.IsDBNull(reader.GetOrdinal("role_name")))
                    roles.Add(reader.GetString(reader.GetOrdinal("role_name")));
            }

            if (user is null)
                return Result<ClaimsPrincipal>.Failure(
                    ResultPatternError.NotFound("User with this Login and Password not found ."));


            List<Claim> claims =
            [
                new(CustomClaimTypes.Id, user.ToString() ?? ""),
                new(CustomClaimTypes.UserName, user.UserName),
                new(CustomClaimTypes.Email, user.Email),
                new(CustomClaimTypes.Phone, user.PhoneNumber),
                new(CustomClaimTypes.FirstName, user.FirstName ?? ""),
                new(CustomClaimTypes.LastName, user.LastName ?? "")
            ];

            claims.AddRange(roles.Select(role => new Claim(CustomClaimTypes.Role, role)));

            ClaimsIdentity identity = new(claims, AuthenticationSchemeDefaults.Cookies);
            return Result<ClaimsPrincipal>.Success(new(identity));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute query.");
            return Result<ClaimsPrincipal>.Failure(ResultPatternError.InternalServerError(e.Message));
        }
    }

    public async Task<Result<bool>> CheckToUniqueUserAsync(User request, CancellationToken token = default)
    {

        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlCommand command = new(UserNpgsqlCommands.CheckToUniqueUser, connection);

        try
        {
            command.Parameters.AddWithValue("@UserName", request.UserName);
            command.Parameters.AddWithValue("@Email", request.Email);
            command.Parameters.AddWithValue("@PhoneNumber", request.PhoneNumber);
            command.Parameters.AddWithValue("@UserId", request.Id);

            bool exists = Convert.ToBoolean(await command.ExecuteScalarAsync(token));
            return exists
                ? Result<bool>.Failure(ResultPatternError.Conflict("User with the same credentials already exists."))
                : Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute uniqueness check.");
            return Result<bool>.Failure(ResultPatternError.InternalServerError(e.Message));
        }
    }

    public async Task<Result<int>> GetCountAsync(UserFilter filter, CancellationToken token = default)
    {
        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlCommand command = connection.CreateCommand();
        try
        {
            string query = UserNpgsqlCommands.GetCountUsers;
            List<string> conditions = [];
            List<NpgsqlParameter> parameters = [];

            void AddCondition(string column, string paramName, string? value, bool useLike = true)
            {
                if (string.IsNullOrWhiteSpace(value)) return;

                if (useLike)
                {
                    conditions.Add($"{column} ILIKE @{paramName}");
                    parameters.Add(new(paramName, $"%{value}%"));
                }
                else
                {
                    conditions.Add($"{column} = @{paramName}");
                    parameters.Add(new(paramName, value));
                }
            }

            AddCondition("first_name", "FirstName", filter.FirstName);
            AddCondition("last_name", "LastName", filter.LastName);
            AddCondition("user_name", "UserName", filter.UserName);
            AddCondition("email", "Email", filter.Email);
            AddCondition("phone_number", "PhoneNumber", filter.PhoneNumber);
           
            if (conditions.Any())
            {
                query += " WHERE " + string.Join(" AND ", conditions);
            }

            command.Parameters.AddRange(parameters.ToArray());
            command.CommandText = query;
            int result = Convert.ToInt32(await command.ExecuteScalarAsync(token));

            return Result<int>.Success(result);
        }
        catch (Exception e)
        {
            return Result<int>.Failure(ResultPatternError.InternalServerError(e.Message));
        }
    }
}