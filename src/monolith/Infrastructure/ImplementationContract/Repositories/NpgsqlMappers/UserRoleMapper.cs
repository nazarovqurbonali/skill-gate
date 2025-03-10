namespace Infrastructure.ImplementationContract.Repositories.NpgsqlMappers;

public static class UserRoleMapper
{
    public static UserRole MapUserRole(this NpgsqlDataReader reader)
    {
        return new UserRole
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
            RoleId = reader.GetGuid(reader.GetOrdinal("role_id")),
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