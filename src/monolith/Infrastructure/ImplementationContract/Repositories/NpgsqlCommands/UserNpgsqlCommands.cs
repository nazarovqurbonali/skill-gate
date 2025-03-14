namespace Infrastructure.ImplementationContract.Repositories.NpgsqlCommands;

public static class UserNpgsqlCommands
{
    public const string InsertUser = @"
        INSERT INTO users (
            id, first_name, last_name, email, phone_number, user_name, 
            password_hash, created_at, created_by, created_by_ip
        )
        VALUES (
            @Id, @FirstName, @LastName, @Email, @PhoneNumber, @UserName,
            @PasswordHash,  @CreatedAt, @CreatedBy, @CreatedByIp
        )";

    public const string UpdateUser = @"
        UPDATE users
        SET 
            first_name = @FirstName,
            last_name = @LastName,
            email = @Email,
            phone_number = @PhoneNumber,
            user_name = @UserName,
            password_hash = @PasswordHash,
            status = @Status,
            updated_at = @UpdatedAt,
            updated_by = @UpdatedBy,
            updated_by_ip = @UpdatedByIp
        WHERE id = @Id";

    public const string DeleteUserById = @"
        DELETE FROM users 
        WHERE id = @Id";


    public const string GetUserById = @"
        SELECT * FROM users 
        WHERE id = @Id";

    public const string GetAllUsers = @"
        SELECT * FROM users";

    public const string CheckExistingUser = @"SELECT EXISTS (
            SELECT 1
            FROM users
            WHERE user_name = @Username
               OR phone_number = @Phone
               OR email = @Email
        )";

    public const string CheckToLoin = @"SELECT EXISTS (
    SELECT 1
    FROM users
    WHERE (user_name = @Login
           OR phone_number = @Login
           OR email = @Login) 
          AND password_hash = @Password)";
    public const string GetCountUsers = @"
        SELECT COUNT(*) FROM users";

    
    public const string GetUserWithRolesByCredentials = @"
    SELECT 
        u.id, u.first_name, u.last_name, u.email, u.phone_number, u.user_name, 
        r.name AS role_name
    FROM users u
    LEFT JOIN user_roles ur ON u.id = ur.user_id
    LEFT JOIN roles r ON ur.role_id = r.id
    WHERE (u.user_name = @Login OR u.phone_number = @Login OR u.email = @Login)
      AND u.password_hash = @Password";

    public const string CheckToUniqueUser=@"
        SELECT EXISTS (
            SELECT 1 FROM users 
            WHERE (user_name = @UserName OR email = @Email OR phone_number = @PhoneNumber)
            AND id <> @UserId
        )";
}