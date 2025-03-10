namespace Infrastructure.ImplementationContract.Repositories;

public sealed class UserRepository(
    IConfiguration config,
    ILogger<UserRepository> logger) : IUserRepository
{
    private readonly string _connectionString =
        config.GetConnectionString(ConfigNames.Npgsql)
        ?? throw new ArgumentNullException($"Connection string '{ConfigNames.Npgsql}' is missing in configuration.");

    private readonly ILogger<UserRepository> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

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
            _logger.LogError(ex, "Failed to add users.");
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

    public async Task<Result<IEnumerable<User>>> GetAllAsync(string query, CancellationToken token = default)
        => await ExecuteQueryListAsync(query, _ => { }, token);

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
            _logger.LogError(ex, "Failed to execute query.");
            return Result<IEnumerable<User>>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    private async Task<Result<User?>> ExecuteQuerySingleAsync(string query, Action<NpgsqlCommand> configureCommand,
        CancellationToken token)
    {
        Result<IEnumerable<User>> result = await ExecuteQueryListAsync(query, configureCommand, token);
        if (result.IsSuccess)
            return Result<User?>.Success(result.Value?.FirstOrDefault());
        return Result<User?>.Failure(result.Error);
    }
}