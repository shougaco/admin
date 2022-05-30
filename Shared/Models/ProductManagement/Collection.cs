namespace Application.Shared.Models.ProductManagement;

public class Collection : Detail
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id  { get; set;}
    public string CompanyId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public string Handle { get; set; }
}