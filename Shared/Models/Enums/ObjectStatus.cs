using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Shared.Models.Enums;

public enum ObjectStatus
{
    [Display(Name = "Active")]
    [Description("active")]
    Active,

    [Display(Name = "Blocked")]
    [Description("blocked")]
    Blocked,

    [Display(Name = "Deleted")]
    [Description("deleted")]
    Deleted,

    
}