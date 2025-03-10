namespace Infrastructure.ImplementationContract.Repositories.NpgsqlMappers;

public static class RoleMapper
{
    public static Role MapRole(this NpgsqlDataReader reader)
    {
        return new Role
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            RoleKey = reader.GetString(reader.GetOrdinal("role_key")),
            Description = reader.IsDBNull(reader.GetOrdinal("description"))
                ? null
                : reader.GetString(reader.GetOrdinal("description")),

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