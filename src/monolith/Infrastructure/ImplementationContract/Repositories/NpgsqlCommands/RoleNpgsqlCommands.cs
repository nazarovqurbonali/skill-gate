namespace Infrastructure.ImplementationContract.Repositories.NpgsqlCommands;

public static class RoleNpgsqlCommands
{
    public const string InsertRole = @"
        INSERT INTO roles (
            id, name, role_key, description, status, created_at, 
            created_by, created_by_ip
        )
        VALUES (
            @Id, @Name, @RoleKey, @Description, @Status, @CreatedAt, 
            @CreatedBy, @CreatedByIp
        )";

    public const string UpdateRole = @"
        UPDATE roles
        SET 
            name = @Name,
            role_key = @RoleKey,
            description = @Description,
            status = @Status,
            updated_at = @UpdatedAt,
            updated_by = @UpdatedBy,
            updated_by_ip = @UpdatedByIp
        WHERE id = @Id";

    public const string DeleteRoleById = @"
        DELETE FROM roles 
        WHERE id = @Id";

    public const string GetRoleById = @"
        SELECT * FROM roles 
        WHERE id = @Id";

    public const string GetAllRoles = @"
        SELECT * FROM roles";

    public const string GetRoleByName = @"
            SELECT * 
            FROM roles 
            WHERE name = @RoleName";

    public const string CheckExistingRole = @"
            SELECT EXISTS (
                SELECT 1
                FROM roles
                WHERE name = @RoleName
            )";
}