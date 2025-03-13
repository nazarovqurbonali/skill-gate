namespace Infrastructure.DataAccess.Seed;

public class Seeder(IConfiguration config, ILogger<Seeder> logger)
{
    private readonly string _connectionString = config.GetConnectionString(ConfigNames.Npgsql)
                                                ?? throw new ArgumentNullException(
                                                    $"Connection string '{ConfigNames.Npgsql}' is missing in configuration.");

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using NpgsqlConnection
            connection = await DbExtensions.CreateConnectionAsync(_connectionString);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await InitRolesAsync(connection, cancellationToken);
            await InitUsersAsync(connection, cancellationToken);
            await InitUserRolesAsync(connection, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(e, "Error during seeding.");
            throw;
        }
    }

    private async Task<bool> ExistsAsync(NpgsqlConnection connection, string query, Guid id,
        CancellationToken cancellationToken)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);
        return Convert.ToBoolean(await command.ExecuteScalarAsync(cancellationToken));
    }

    private async Task InitRolesAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        foreach (Role role in SeedData.ListRoles)
        {
            try
            {
                logger.LogInformation("Checking if role exists: {RoleId}", role.Id);
                if (await ExistsAsync(connection, SqlCommands.CheckExistingRole, role.Id, cancellationToken))
                {
                    logger.LogInformation("Role '{RoleName}' (ID: {RoleId}) already exists.", role.Name, role.Id);
                    continue;
                }

                bool inserted = await InsertEntityAsync(connection, SqlCommands.InsertRole, cancellationToken,
                    new("@id", role.Id),
                    new("@name", role.Name),
                    new("@role_key", role.RoleKey),
                    new("@description", NpgsqlDbType.Text)
                    {
                        Value = role.Description.ToDbValue()
                    },
                    new("@created_by_ip", NpgsqlDbType.Text)
                    {
                        Value = role.CreatedByIp.ToDbValue()
                    },
                    new("@created_by", NpgsqlDbType.Uuid)
                    {
                        Value = role.CreatedBy.ToDbValue()
                    }
                );

                if (inserted)
                {
                    logger.LogInformation("Role '{RoleName}' (ID: {RoleId}) inserted successfully.", role.Name,
                        role.Id);
                }
                else
                {
                    logger.LogError("Failed to insert role '{RoleName}' (ID: {RoleId}).", role.Name, role.Id);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error inserting role '{RoleName}' (ID: {RoleId}).", role.Name, role.Id);
            }
        }
    }

    private async Task InitUsersAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        foreach (User user in SeedData.ListUsers)
        {
            try
            {
                logger.LogInformation("Checking if user exists: {UserId}", user.Id);
                if (await ExistsAsync(connection, SqlCommands.CheckExistingUser, user.Id, cancellationToken))
                {
                    logger.LogInformation("User '{UserName}' (ID: {UserId}) already exists.", user.UserName, user.Id);
                    continue;
                }

                bool inserted = await InsertEntityAsync(connection, SqlCommands.InsertUser, cancellationToken,
                    new NpgsqlParameter("@id", user.Id),
                    new NpgsqlParameter("@email", user.Email),
                    new NpgsqlParameter("@phone_number", user.PhoneNumber),
                    new NpgsqlParameter("@user_name", user.UserName),
                    new NpgsqlParameter("@password_hash", user.PasswordHash),
                    new NpgsqlParameter("@created_by_ip", user.CreatedByIp?.ToDbValue()),
                    new NpgsqlParameter("@created_by", user.CreatedBy?.ToDbValue())
                );

                if (inserted)
                {
                    logger.LogInformation("User '{UserName}' (ID: {UserId}) inserted successfully.", user.UserName,
                        user.Id);
                }
                else
                {
                    logger.LogError("Failed to insert user '{UserName}' (ID: {UserId}).", user.UserName, user.Id);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error inserting user '{UserName}' (ID: {UserId}).", user.UserName, user.Id);
            }
        }
    }

    private async Task InitUserRolesAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        foreach (UserRole userRole in SeedData.ListUserRoles)
        {
            try
            {
                logger.LogInformation("Checking if user role exists: {UserRoleId}", userRole.Id);
                if (await ExistsAsync(connection, SqlCommands.CheckExistingUserRole, userRole.Id, cancellationToken))
                {
                    logger.LogInformation("User role '{UserRoleId}' already exists.", userRole.Id);
                    continue;
                }

                bool inserted = await InsertEntityAsync(connection, SqlCommands.InsertUserRole, cancellationToken,
                    new NpgsqlParameter("@id", userRole.Id),
                    new NpgsqlParameter("@user_id", userRole.UserId),
                    new NpgsqlParameter("@role_id", userRole.RoleId),
                    new NpgsqlParameter("@created_by_ip", userRole.CreatedByIp?.ToDbValue()),
                    new NpgsqlParameter("@created_by", userRole.CreatedBy?.ToDbValue())
                );

                if (inserted)
                {
                    logger.LogInformation("User role '{UserRoleId}' inserted successfully.", userRole.Id);
                }
                else
                {
                    logger.LogError("Failed to insert user role '{UserRoleId}'.", userRole.Id);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error inserting user role '{UserRoleId}'.", userRole.Id);
            }
        }
    }

    private async Task<bool> InsertEntityAsync(NpgsqlConnection connection, string insertCommand,
        CancellationToken cancellationToken, params NpgsqlParameter[] parameters)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = insertCommand;
        command.Parameters.AddRange(parameters);

        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }
}

file static class SeedData
{
    private static readonly Guid SystemId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid AdminId = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid UserId = new("33333333-3333-3333-3333-333333333333");

    private static readonly Guid AdminRoleId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid UserRoleId = new("22222222-2222-2222-2222-222222222222");

    private static readonly Guid UserRoleId1 = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid UserRoleId2 = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid UserRoleId3 = new("33333333-3333-3333-3333-333333333333");
    private static readonly Guid UserRoleId4 = new("44444444-4444-4444-4444-444444444444");
    private static readonly Guid UserRoleId5 = new("55555555-5555-5555-5555-555555555555");

    public static readonly List<Role> ListRoles =
    [
        new()
        {
            Id = AdminRoleId,
            Name = Roles.Admin,
            RoleKey = Roles.Admin,
            CreatedBy = SystemId,
            CreatedByIp = "localhost"
        },
        new()
        {
            Id = UserRoleId,
            Name = Roles.User,
            RoleKey = Roles.User,
            CreatedBy = SystemId,
            CreatedByIp = "localhost"
        },
    ];

    public static readonly List<User> ListUsers =
    [
        new()
        {
            Id = SystemId,
            Email = "system@gmail.com",
            PhoneNumber = "+99200000000",
            UserName = "System",
            PasswordHash = HashingUtility.ComputeSha256Hash("11111111"),
            CreatedBy = SystemId,
            CreatedByIp = "localhost"
        },
        new()
        {
            Id = AdminId,
            Email = "admin@gmail.com",
            PhoneNumber = "+992001001001",
            UserName = "Admin",
            CreatedBy = SystemId,
            PasswordHash = HashingUtility.ComputeSha256Hash("12345678"),
            CreatedByIp = "localhost"
        },
        new()
        {
            Id = UserId,
            Email = "user@gmail.com",
            PhoneNumber = "+992002002002",
            UserName = "User",
            CreatedBy = SystemId,
            PasswordHash = HashingUtility.ComputeSha256Hash("43211234"),
            CreatedByIp = "localhost"
        },
    ];

    public static readonly List<UserRole> ListUserRoles =
    [
        new()
        {
            Id = UserRoleId1,
            UserId = AdminId,
            RoleId = AdminRoleId,
            CreatedBy = SystemId,
            CreatedByIp = "localhost"
        },
        new()
        {
            Id = UserRoleId2,
            UserId = AdminId,
            RoleId = UserRoleId,
            CreatedBy = SystemId,
            CreatedByIp = "localhost"
        },
        new()
        {
            Id = UserRoleId3,
            UserId = UserId,
            RoleId = UserRoleId,
            CreatedBy = SystemId,
            CreatedByIp = "localhost"
        },
        new()
        {
            Id = UserRoleId4,
            UserId = SystemId,
            RoleId = AdminRoleId,
            CreatedBy = SystemId,
            CreatedByIp = "localhost"
        },
        new()
        {
            Id = UserRoleId5,
            UserId = SystemId,
            RoleId = UserRoleId,
            CreatedBy = SystemId,
            CreatedByIp = "localhost"
        },
    ];
}

file static class SqlCommands
{
    public const string CheckExistingRole = "SELECT EXISTS (SELECT 1 FROM roles WHERE id = @id);";
    public const string CheckExistingUser = "SELECT EXISTS (SELECT 1 FROM users WHERE id = @id);";
    public const string CheckExistingUserRole = "SELECT EXISTS (SELECT 1 FROM user_roles WHERE id = @id);";

    public const string InsertRole = @"INSERT INTO roles(id, name, role_key, description, created_by_ip, created_by)
                                        VALUES(@id, @name, @role_key, @description, @created_by_ip, @created_by)";

    public const string InsertUser =
        @"INSERT INTO users(id, email, phone_number, user_name, password_hash, created_by_ip, created_by)
                                        VALUES(@id, @email, @phone_number, @user_name, @password_hash, @created_by_ip, @created_by)";

    public const string InsertUserRole = @"INSERT INTO user_roles(id, user_id, role_id, created_by_ip, created_by)
                                             VALUES(@id, @user_id, @role_id, @created_by_ip, @created_by)";
}