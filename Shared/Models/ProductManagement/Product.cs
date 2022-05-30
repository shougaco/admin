using Application.Shared.Models.Org;

namespace Application.Shared.Models.ProductManagement;


public class Product : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    public string? CompanyId { get; set; }
    public Company? Company { get; set; }
    public string Code { get; set; }
    public string Brand { get; set; }
    public string Description { get; set; }

    public ProductStatus Status { get; set; }


    public ICollection<ProductTag>? ProductTags { get; set; }


}