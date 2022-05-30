namespace Application.Shared.Models.Org;



public class Company : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public CompanyType Type { get; set; }
    // Max data anotation
    [MaxLength(30)]
    [Required]
    public string Slug { get; set; }

    [Required]
    [MaxLength(10, ErrorMessage = "Name cannot be longer than 10 characters.")]
    public string Name { get; set; }

    public string? Description { get; set; }
    public int? CountryId { get; set; }
    public Country? Country { get; set; }

    public bool IsDefault { get; set; }


}