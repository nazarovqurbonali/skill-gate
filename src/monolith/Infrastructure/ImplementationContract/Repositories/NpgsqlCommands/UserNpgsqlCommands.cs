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
}