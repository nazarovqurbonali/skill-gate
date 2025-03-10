namespace Infrastructure.ImplementationContract.Repositories.NpgsqlParameters;

public static class ProductParameter
{
    public static void AddProductParameters(this NpgsqlCommand command, Product entity)
    {
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@Name", entity.Name);
        command.Parameters.AddWithValue("@Description", entity.Description.ToDbValue());
        command.Parameters.AddWithValue("@Price", entity.Price);
        command.Parameters.AddWithValue("@StockQuantity", entity.StockQuantity);
        command.Parameters.AddWithValue("@ImageUrl", entity.ImageUrl.ToDbValue());
        command.Parameters.AddWithValue("@Status", entity.Status);
        command.Parameters.AddWithValue("@CreatedAt", entity.CreatedAt);
        command.Parameters.AddWithValue("@CreatedBy", entity.CreatedBy.ToDbValue());
        command.Parameters.AddWithValue("@CreatedByIp", entity.CreatedByIp.ToDbValue());
    }

    public static void AddUpdateProductParameters(this NpgsqlCommand command, Product entity)
    {
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@Name", entity.Name);
        command.Parameters.AddWithValue("@Description", entity.Description.ToDbValue());
        command.Parameters.AddWithValue("@Price", entity.Price);
        command.Parameters.AddWithValue("@StockQuantity", entity.StockQuantity);
        command.Parameters.AddWithValue("@ImageUrl", entity.ImageUrl.ToDbValue());
        command.Parameters.AddWithValue("@Status", entity.Status);
        command.Parameters.AddWithValue("@UpdatedAt", entity.UpdatedAt.ToDbValue());
        command.Parameters.AddWithValue("@UpdatedBy", entity.UpdatedBy.ToDbValue());
        command.Parameters.AddWithValue("@UpdatedByIp", entity.UpdatedByIp.ToDbValue());
    }

    public static void AddDeleteProductParameters(this NpgsqlCommand command, Guid productId)
    {
        command.Parameters.AddWithValue("@Id", productId);
    }
}
