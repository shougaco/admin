using Application.Shared.Models.Enums;

namespace Application.Shared.Models.Org;

public class Store
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string CompanyId { get; set; }
    public Company Company { get; set; }

    [MaxLength(30)]
    public string Slug { get; set; }

    [MaxLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
    public string Name { get; set; }

    [MaxLength(250, ErrorMessage = "Address cannot be longer than 250 characters.")]
    public string Description { get; set; }
    
    
}