namespace Infrastructure.ImplementationContract.Repositories.NpgsqlMappers;

public static class UserMapper
{
    public static User MapUser(this NpgsqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            FirstName = reader.IsDBNull(reader.GetOrdinal("first_name"))
                ? null
                : reader.GetString(reader.GetOrdinal("first_name")),
            LastName = reader.IsDBNull(reader.GetOrdinal("last_name"))
                ? null
                : reader.GetString(reader.GetOrdinal("last_name")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
            UserName = reader.GetString(reader.GetOrdinal("user_name")),
            Dob = reader.IsDBNull(reader.GetOrdinal("dob")) ? null : reader.GetDateTime(reader.GetOrdinal("dob")),

            EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("email_confirmed")),
            PhoneNumberConfirmed = reader.GetBoolean(reader.GetOrdinal("phone_number_confirmed")),

            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
            LastPasswordChangeAt = reader.IsDBNull(reader.GetOrdinal("last_password_change_at"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("last_password_change_at")),

            LastLoginAt = reader.IsDBNull(reader.GetOrdinal("last_login_at"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("last_login_at")),
            IsLockedOut = reader.GetBoolean(reader.GetOrdinal("is_locked_out")),
            LockoutEnd = reader.IsDBNull(reader.GetOrdinal("lockout_end"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("lockout_end")),
            TotalLogins = reader.GetInt64(reader.GetOrdinal("total_logins")),

            TwoFactorSecret = reader.IsDBNull(reader.GetOrdinal("two_factor_secret"))
                ? null
                : reader.GetString(reader.GetOrdinal("two_factor_secret")),
            TwoFactorEnabled = reader.GetBoolean(reader.GetOrdinal("two_factor_enabled")),

            Status = (EntityStatus)reader.GetInt32(reader.GetOrdinal("status")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("updated_at")),
            DeletedAt = reader.IsDBNull(reader.GetOrdinal("deleted_at"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("deleted_at")),
            Version = reader.GetInt64(reader.GetOrdinal("version")),

            CreatedBy = reader.IsDBNull(reader.GetOrdinal("created_by"))
                ? null
                : reader.GetGuid(reader.GetOrdinal("created_by")),
            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("updated_by"))
                ? null
                : reader.GetGuid(reader.GetOrdinal("updated_by")),
            DeletedBy = reader.IsDBNull(reader.GetOrdinal("deleted_by"))
                ? null
                : reader.GetGuid(reader.GetOrdinal("deleted_by")),

            CreatedByIp = reader.IsDBNull(reader.GetOrdinal("created_by_ip"))
                ? null
                : reader.GetString(reader.GetOrdinal("created_by_ip")),
            UpdatedByIp = reader.IsDBNull(reader.GetOrdinal("updated_by_ip"))
                ? null
                : reader.GetString(reader.GetOrdinal("updated_by_ip")),
            DeletedByIp = reader.IsDBNull(reader.GetOrdinal("deleted_by_ip"))
                ? null
                : reader.GetString(reader.GetOrdinal("deleted_by_ip")),
        };
    }
}