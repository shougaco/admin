using Application.Shared.Models.Org;

namespace Application.Shared.Models;


public class TransLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public string CompanyId { get; set; }
    public Company Company { get; set; }
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public EntryType EntryType { get; set; }
    public string Table { get; set; }
    public string Column { get; set; }
    public string ObjectId { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }

}