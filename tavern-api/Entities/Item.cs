using tavern_api.Commons;

namespace tavern_api.Entities;

public sealed class Item : BaseEntity
{
    public string ItemName { get; set; } = null!;
    public string ItemUrl { get; set; } = null!;
    public string Extension { get; set; } = null!;
    public string? Note { get; set; } = null;
    public string FolderId { get; set; } = null!; 
    public Folder Folder { get; set; } = null!;

    private Item() { }

    
}
