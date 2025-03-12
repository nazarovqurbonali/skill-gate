namespace Infrastructure.ImplementationContract.Repositories;

public sealed class RoleRepository(
    IConfiguration config,
    ILogger<RoleRepository> logger) : IRoleRepository
{
    private readonly string _connectionString =
        config.GetConnectionString(ConfigNames.Npgsql)
        ?? throw new ArgumentNullException($"Connection string '{ConfigNames.Npgsql}' is missing in configuration.");

    private readonly ILogger<RoleRepository> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Result<int>> AddAsync(Role entity, CancellationToken token = default) =>
        await ExecuteNonQueryTransactionAsync(RoleNpgsqlCommands.InsertRole, cmd
            => cmd.AddRoleParameters(entity), token);

    public async Task<Result<int>> AddRangeAsync(ICollection<Role> entities, CancellationToken token = default)
    {
        if (!entities.Any())
            return Result<int>.Failure(ResultPatternError.BadRequest("No roles to add."));

        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(token);

        try
        {
            await using NpgsqlCommand cmd = new(RoleNpgsqlCommands.InsertRole, connection, transaction);
            foreach (Role entity in entities)
            {
                cmd.Parameters.Clear();
                cmd.AddRoleParameters(entity);
                await cmd.ExecuteNonQueryAsync(token);
            }

            await transaction.CommitAsync(token);
            return Result<int>.Success(entities.Count);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);
            _logger.LogError(ex, "Failed to add roles.");
            return Result<int>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<int>> UpdateAsync(Role value, CancellationToken token = default) =>
        await ExecuteNonQueryTransactionAsync(RoleNpgsqlCommands.UpdateRole,
            cmd => cmd.AddUpdateRoleParameters(value), token);

    public async Task<Result<int>> UpdateAsync(Guid id, Role value, CancellationToken token = default)
    {
        value.Id = id;
        return await UpdateAsync(value, token);
    }

    public async Task<Result<int>> DeleteAsync(Guid id, CancellationToken token = default)
        => await ExecuteNonQueryTransactionAsync(RoleNpgsqlCommands.DeleteRoleById,
            cmd => cmd.Parameters.AddWithValue("@Id", id), token);

    public async Task<Result<int>> DeleteAsync(Role value, CancellationToken token = default)
        => await DeleteAsync(value.Id, token);

    public async Task<Result<Role?>> GetByIdAsync(Guid id, CancellationToken token = default)
        => await ExecuteQuerySingleAsync(RoleNpgsqlCommands.GetRoleById, cmd => cmd.Parameters.AddWithValue("@Id", id),
            token);

    public async Task<Result<IEnumerable<Role>>> GetAllAsync(CancellationToken token = default)
        => await ExecuteQueryListAsync(RoleNpgsqlCommands.GetAllRoles, _ => { }, token);
    
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
            _logger.LogError(ex, "Database operation failed.");
            return Result<int>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    private async Task<Result<IEnumerable<Role>>> ExecuteQueryListAsync(string query,
        Action<NpgsqlCommand> configureCommand, CancellationToken token)
    {
        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlCommand command = new(query, connection);
        configureCommand(command);

        try
        {
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(token);
            List<Role> roles = [];
            while (await reader.ReadAsync(token))
            {
                roles.Add(reader.MapRole());
            }

            return Result<IEnumerable<Role>>.Success(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute query.");
            return Result<IEnumerable<Role>>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    private async Task<Result<Role?>> ExecuteQuerySingleAsync(string query, Action<NpgsqlCommand> configureCommand,
        CancellationToken token)
    {
        Result<IEnumerable<Role>> result = await ExecuteQueryListAsync(query, configureCommand, token);
        if (result.IsSuccess)
            return Result<Role?>.Success(result.Value?.FirstOrDefault());
        return Result<Role?>.Failure(result.Error);
    }

    public async Task<Result<IEnumerable<Role>>> GetAllAsync(RoleFilter filter, CancellationToken token = default)
    {
        string query = RoleNpgsqlCommands.GetAllRoles;
        List<string> conditions = [];
        List<NpgsqlParameter> parameters = [];

        void AddCondition(string column, string paramName, string? value, bool useLike = true)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            if (useLike)
            {
                conditions.Add($"{column} ILIKE @{paramName}");
                parameters.Add(new (paramName, $"%{value}%"));
            }
            else
            {
                conditions.Add($"{column} = @{paramName}");
                parameters.Add(new(paramName, value));
            }
        }

        AddCondition("role_name", "RoleName", filter.Name);
        AddCondition("description", "Description", filter.Description);
        AddCondition("role_key", "RoleKey", filter.Keyword);

        if (conditions.Any())
        {
            query += " WHERE " + string.Join(" AND ", conditions);
        }

        return await ExecuteQueryListAsync(query,
            (cmd) => { cmd.Parameters.AddRange(parameters.ToArray()); }, token);
    }

    public async Task<Result<Role?>> GetRoleByNameAsync(string roleName, CancellationToken token = default)
        => await ExecuteQuerySingleAsync(RoleNpgsqlCommands.GetRoleByName,
            cmd => cmd.Parameters.AddWithValue("@RoleName", roleName),
            token);

    public async Task<Result<bool>> CheckExistingRoleAsync(string roleName, CancellationToken token = default)
    {
        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlCommand command = new(RoleNpgsqlCommands.CheckExistingRole, connection);
        try
        {
            command.Parameters.AddWithValue("@RoleName", roleName);
            bool result = Convert.ToBoolean(await command.ExecuteScalarAsync(token));
            return result
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(ResultPatternError.InternalServerError());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute query.");
            return Result<bool>.Failure(ResultPatternError.InternalServerError(e.Message));
        }
    }
}