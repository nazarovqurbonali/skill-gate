namespace Infrastructure.Extensions;

public static class Npgsql
{
    public static async Task<NpgsqlConnection> CreateConnection(
        IConfiguration configuration,
        string connectionStringName)
    {
        NpgsqlConnection connection = new(configuration.GetConnectionString(connectionStringName));
        await connection.OpenAsync();
        return connection;
    }
}