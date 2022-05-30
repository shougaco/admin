namespace Application.Shared.Models.ProductManagement;


public class ProductPrice : Detail
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string? VariantOptionId { get; set; }
    public VariantOption VariantOption { get; set; }


    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
}