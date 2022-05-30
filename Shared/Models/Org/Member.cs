namespace Application.Shared.Models.Org;

public class Member : Detail 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    public string? CompanyId { get; set; }

    public Company? Company { get; set; }

    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
}