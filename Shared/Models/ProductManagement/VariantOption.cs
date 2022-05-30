namespace Application.Shared.Models.ProductManagement;


public class VariantOption : Detail
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string? ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; }
    public string Value { get; set; }
    
}