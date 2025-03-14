namespace Infrastructure.ImplementationContract.Repositories;

public sealed class ProductRepository(
    IConfiguration config,
    ILogger<ProductRepository> logger) : IProductRepository
{
    private readonly string _connectionString =
        config.GetConnectionString(ConfigNames.Npgsql)
        ?? throw new ArgumentNullException(
            $"Connection string '{ConfigNames.Npgsql}' is missing in configuration.");

    private readonly ILogger<ProductRepository> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Result<int>> AddAsync(Product entity, CancellationToken token = default) =>
        await ExecuteNonQueryTransactionAsync(ProductNpgsqlCommands.InsertProduct,
            cmd => cmd.AddProductParameters(entity), token);

    public async Task<Result<int>> AddRangeAsync(ICollection<Product> entities, CancellationToken token = default)
    {
        if (!entities.Any())
            return Result<int>.Failure(ResultPatternError.BadRequest("No products to add."));

        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(token);

        try
        {
            await using NpgsqlCommand cmd = new(ProductNpgsqlCommands.InsertProduct, connection, transaction);
            foreach (Product entity in entities)
            {
                cmd.Parameters.Clear();
                cmd.AddProductParameters(entity);
                await cmd.ExecuteNonQueryAsync(token);
            }

            await transaction.CommitAsync(token);
            return Result<int>.Success(entities.Count);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);
            _logger.LogError(ex, "Failed to add products.");
            return Result<int>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<int>> UpdateAsync(Product value, CancellationToken token = default) =>
        await ExecuteNonQueryTransactionAsync(ProductNpgsqlCommands.UpdateProduct,
            cmd => cmd.AddUpdateProductParameters(value), token);

    public async Task<Result<int>> UpdateAsync(Guid id, Product value, CancellationToken token = default)
    {
        value.Id = id;
        return await UpdateAsync(value, token);
    }

    public async Task<Result<int>> DeleteAsync(Guid id, CancellationToken token = default) =>
        await ExecuteNonQueryTransactionAsync(ProductNpgsqlCommands.DeleteProductById,
            cmd => cmd.Parameters.AddWithValue("@Id", id), token);

    public async Task<Result<int>> DeleteAsync(Product value, CancellationToken token = default) =>
        await DeleteAsync(value.Id, token);

    public async Task<Result<Product?>> GetByIdAsync(Guid id, CancellationToken token = default) =>
        await ExecuteQuerySingleAsync(ProductNpgsqlCommands.GetProductById,
            cmd => cmd.Parameters.AddWithValue("@Id", id), token);

    public async Task<Result<IEnumerable<Product>>> GetAllAsync(CancellationToken token = default) =>
        await ExecuteQueryListAsync(ProductNpgsqlCommands.GetAllProducts, _ => { }, token);

    public async Task<Result<IEnumerable<Product>>> GetAllAsync(ProductFilter filter, CancellationToken token = default)
    {
        string query = ProductNpgsqlCommands.GetAllProducts;
        List<string> conditions = [];
        List<NpgsqlParameter> parameters = [];
        

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

        AddCondition("name", "Name", filter.Name);
        AddCondition("description", "Description", filter.Description);

        if (conditions.Any())
            query += " WHERE " + string.Join(" AND ", conditions);

        int offset = (filter.PageNumber - 1) * filter.PageSize;
        query += $" LIMIT {filter.PageSize} OFFSET {offset}";
        
        return await ExecuteQueryListAsync(query,
            (cmd) => { cmd.Parameters.AddRange(parameters.ToArray()); }, token);
    }

    public async Task<Result<int>> GetCountAsync(ProductFilter filter, CancellationToken token = default)
    {
        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlCommand command = connection.CreateCommand();
        try
        {
            string query = ProductNpgsqlCommands.GetCountProducts;
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

            AddCondition("name", "RoleName", filter.Name);
            AddCondition("description", "Description", filter.Description);
          
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

    private async Task<Result<IEnumerable<Product>>> ExecuteQueryListAsync(string query,
        Action<NpgsqlCommand> configureCommand, CancellationToken token)
    {
        await using NpgsqlConnection connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlCommand command = new(query, connection);
        configureCommand(command);

        try
        {
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(token);
            List<Product> products = new List<Product>();
            while (await reader.ReadAsync(token))
            {
                products.Add(reader.MapProduct());
            }

            return Result<IEnumerable<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute query.");
            return Result<IEnumerable<Product>>.Failure(ResultPatternError.InternalServerError(ex.Message));
        }
    }

    private async Task<Result<Product?>> ExecuteQuerySingleAsync(string query, Action<NpgsqlCommand> configureCommand,
        CancellationToken token)
    {
        Result<IEnumerable<Product>> result = await ExecuteQueryListAsync(query, configureCommand, token);
        if (result.IsSuccess)
            return Result<Product?>.Success(result.Value?.FirstOrDefault());
        return Result<Product?>.Failure(result.Error);
    }
}