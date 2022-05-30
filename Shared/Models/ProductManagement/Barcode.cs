namespace Application.Shared.Models.ProductManagement;

public class Barcode : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public string No { get; set; }
    public string? ProductId { get; set; }
    public Product? Product { get; set; }
    public string UnitOMeasure { get; set; }

}
