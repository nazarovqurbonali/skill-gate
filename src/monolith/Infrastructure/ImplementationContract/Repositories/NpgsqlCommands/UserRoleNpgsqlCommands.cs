namespace Infrastructure.ImplementationContract.Repositories.NpgsqlCommands;

public static class UserRoleNpgsqlCommands
{
    public const string InsertUserRole = @"
        INSERT INTO user_roles (
            id, user_id, role_id, status, created_at, created_by, created_by_ip
        )
        VALUES (
            @Id, @UserId, @RoleId, @Status, @CreatedAt, @CreatedBy, @CreatedByIp
        )";

    public const string GetCountUserRoles = @"
        SELECT COUNT(*) FROM user_roles";


    public const string UpdateUserRole = @"
        UPDATE user_roles
        SET 
            user_id = @UserId,
            role_id = @RoleId,
            status = @Status,
            updated_at = @UpdatedAt,
            updated_by = @UpdatedBy,
            updated_by_ip = @UpdatedByIp
        WHERE id = @Id";

    public const string DeleteUserRoleById = @"
        DELETE FROM user_roles 
        WHERE id = @Id";

    public const string GetUserRoleById = @"
        SELECT * FROM user_roles 
        WHERE id = @Id";

    public const string GetAllUserRoles = @"
        SELECT * FROM user_roles";
}