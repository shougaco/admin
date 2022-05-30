using System.Text.Json.Serialization;

namespace Application.Shared.Models;

public class Country : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code2 { get; set; }
    public string Code3 { get; set; }
    public string Currency { get; set; }
    public string Language { get; set; }
    public string TimeZone { get; set; }
    public string PhoneCode { get; set; }
    
}