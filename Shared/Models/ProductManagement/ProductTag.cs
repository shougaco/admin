using Application.Shared.Models.Org;

namespace Application.Shared.Models.ProductManagement;


public class ProductTag : Detail
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public string? ProductId { get; set; }
    public Product? Product { get; set; }
    public string Name { get; set; }
}