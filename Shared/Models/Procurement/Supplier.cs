using Application.Shared.Models.Org;

namespace Application.Shared.Models.Procurement;


public class Supplier : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public string? CompanyId { get; set; }
    public Company? Company { get; set; }

    public string Code { get; set; }
    public string Name { get; set; }
}