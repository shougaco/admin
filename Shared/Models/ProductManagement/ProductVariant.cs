namespace Application.Shared.Models.ProductManagement;


public class ProductVariant : Detail
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string? ProductId { get; set; }
    public Product Product { get; set; }

    [MaxLength(4)]
    public string Code { get; set; }

    public string Name { get; set; }

}