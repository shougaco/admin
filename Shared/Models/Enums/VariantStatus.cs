using System.ComponentModel;

namespace Application.Shared.Models.Enums;

public enum VariantStatus
{

    [Description("live")]
    Live,

    [Description("deleted")]
    Deleted,

    [Description("discontinued")]
    Discontinued,

    [Description("out of stock")]
    OutOfStock
}