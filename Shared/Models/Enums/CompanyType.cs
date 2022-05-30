using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Shared.Models.Enums;

public enum CompanyType
{
    [Description("customer")]
    Customer,

    [Description("supplier")]
    Supplier
    
}