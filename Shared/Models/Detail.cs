using Application.Shared.Models.Enums;

namespace Application.Shared.Models;

public class Detail
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime UpdatedOn { get; set; } = DateTime.Now;
    
    public ObjectStatus ObjectStatus { get; set; }
}