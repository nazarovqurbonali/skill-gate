namespace Infrastructure.ImplementationContract.Repositories;

public sealed class UserRoleRepository(
    IConfiguration config,
    ILogger<UserRoleRepository> logger) : IUserRoleRepository
{
    private readonly string _connectionString =
        config.GetConnectionString(ConfigNames.Npgsql)
        ?? throw new ArgumentNullException($"Connection string '{ConfigNames.Npgsql}' is missing in configuration.");

    private readonly ILogger<UserRoleRepository> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Result<int>> AddAsync(UserRole entity, CancellationToken token = default)
        => await ExecuteNonQueryTransactionAsync(UserRoleNpgsqlCommands.InsertUserRole,
            cmd => cmd.AddUserRoleParameters(entity), token);


    public async Task<Result<int>> AddRangeAsync(ICollection<UserRole> entities, CancellationToken token = default)
    {
        if (!entities.Any())
            return Result<int>.Failure(ResultPatternError.BadRequest("No user roles to add."));

        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(token);

        try
        {
            await using NpgsqlCommand cmd = new(UserRoleNpgsqlCommands.InsertUserRole, connection, transaction);
            foreach (UserRole entity in entities)
            {
                cmd.Parameters.Clear();
                cmd.AddUserRoleParameters(entity);
                await cmd.ExecuteNonQueryAsync(token);
            }

            await transaction.CommitAsync(token);
            return Result<int>.Success(entities.Count);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);
            _logger.LogError(ex, "Failed to add user roles.");
            return Result<int>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<int>> UpdateAsync(UserRole value, CancellationToken token = default)
    {
        return await ExecuteNonQueryTransactionAsync(UserRoleNpgsqlCommands.UpdateUserRole,
            cmd => cmd.AddUpdateUserRoleParameters(value), token);
    }

    public async Task<Result<int>> UpdateAsync(Guid id, UserRole value, CancellationToken token = default)
    {
        value.Id = id;
        return await UpdateAsync(value, token);
    }

    public async Task<Result<UserRole?>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await ExecuteQuerySingleAsync(UserRoleNpgsqlCommands.GetUserRoleById,
            cmd => cmd.Parameters.AddWithValue("@Id", id), token);
    }

    public async Task<Result<IEnumerable<UserRole>>> GetAllAsync(CancellationToken token = default)
    {
        return await ExecuteQueryListAsync(UserRoleNpgsqlCommands.GetAllUserRoles, _ => { }, token);
    }

    public async Task<Result<IEnumerable<UserRole>>> GetAllAsync(string query, CancellationToken token = default)
    {
        return await ExecuteQueryListAsync(query, _ => { }, token);
    }

    public async Task<Result<int>> DeleteAsync(Guid id, CancellationToken token = default)
    {
        return await ExecuteNonQueryTransactionAsync(UserRoleNpgsqlCommands.DeleteUserRoleById,
            cmd => cmd.Parameters.AddWithValue("@Id", id), token);
    }

    public async Task<Result<int>> DeleteAsync(UserRole value, CancellationToken token = default)
    {
        return await DeleteAsync(value.Id, token);
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
            _logger.LogError(ex, "Database operation failed.");
            return Result<int>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    private async Task<Result<IEnumerable<UserRole>>> ExecuteQueryListAsync(string query,
        Action<NpgsqlCommand> configureCommand, CancellationToken token)
    {
        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlCommand command = new(query, connection);
        configureCommand(command);

        try
        {
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(token);
            List<UserRole> userRoles = new();
            while (await reader.ReadAsync(token))
            {
                userRoles.Add(reader.MapUserRole());
            }

            return Result<IEnumerable<UserRole>>.Success(userRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute query.");
            return Result<IEnumerable<UserRole>>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    private async Task<Result<UserRole?>> ExecuteQuerySingleAsync(string query, Action<NpgsqlCommand> configureCommand,
        CancellationToken token)
    {
        Result<IEnumerable<UserRole>> result = await ExecuteQueryListAsync(query, configureCommand, token);
        if (result.IsSuccess)
            return Result<UserRole?>.Success(result.Value?.FirstOrDefault());
        return Result<UserRole?>.Failure(result.Error);
    }
}