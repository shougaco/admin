using Application.Shared.Models.Procurement;

namespace Application.Shared.Models.Org;


public class SupplierUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    public string? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
}