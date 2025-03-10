namespace Infrastructure.Extensions;

public static class DbExtensions
{
    public static async Task<NpgsqlConnection> CreateConnectionAsync(string connectionString)
    {
        NpgsqlConnection connection = new(connectionString);
        await connection.OpenAsync();
        return connection;
    }


    public static object ToDbValue(this object value)
    {
        return value ?? DBNull.Value;
    }
}