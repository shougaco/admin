namespace Application.Shared.Models.ProductManagement;

public class ProductCollection : Detail
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    public string? ProductId { get; set; }
    public Product? Product { get; set; }

    public string? CollectionId { get; set; }
    public Collection? Collection { get; set; }
}