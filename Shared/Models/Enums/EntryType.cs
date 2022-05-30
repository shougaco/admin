using System.ComponentModel;

namespace Application.Shared.Models.Enums;


public enum EntryType {

    [Description("create")]
    Create,

    [Description("update")]
    Update,

    [Description("delete")]
    Delete

}