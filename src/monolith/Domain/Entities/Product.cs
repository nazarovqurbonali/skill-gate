namespace Domain.Entities;

public sealed class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty; 
    public string? Description { get; set; } 
    public decimal Price { get; set; } 
    public int StockQuantity { get; set; } 
    public string? ImageUrl { get; set; } 
}