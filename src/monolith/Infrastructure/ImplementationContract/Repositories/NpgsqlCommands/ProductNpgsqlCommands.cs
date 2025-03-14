namespace Infrastructure.ImplementationContract.Repositories.NpgsqlCommands;

public static class ProductNpgsqlCommands
{
    public const string InsertProduct = @"
        INSERT INTO products (
            id, name, description, price, stock_quantity, image_url, 
            status, created_at, created_by, created_by_ip
        )
        VALUES (
            @Id, @Name, @Description, @Price, @StockQuantity, @ImageUrl, 
            @Status, @CreatedAt, @CreatedBy, @CreatedByIp
        )";
    public const string GetCountProducts = @"
        SELECT COUNT(*) FROM products";


    public const string UpdateProduct = @"
        UPDATE products
        SET 
            name = @Name,
            description = @Description,
            price = @Price,
            stock_quantity = @StockQuantity,
            image_url = @ImageUrl,
            status = @Status,
            updated_at = @UpdatedAt,
            updated_by = @UpdatedBy,
            updated_by_ip = @UpdatedByIp
        WHERE id = @Id";

    public const string DeleteProductById = @"
        DELETE FROM products 
        WHERE id = @Id";

    public const string GetProductById = @"
        SELECT * FROM products 
        WHERE id = @Id";

    public const string GetAllProducts = @"
        SELECT * FROM products";
}