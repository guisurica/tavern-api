using tavern_api.Commons;
using tavern_api.Commons.Exceptions;

namespace tavern_api.Entities;

public sealed class Item : BaseEntity
{
    public string ItemName { get; set; } = null!;
    public string Extension { get; set; } = null!;
    public string? Note { get; set; } = null;
    public string FolderId { get; set; } = null!;
    public long ItemSize { get; set; } = 0;
    public Folder Folder { get; set; } = null!;

    private Item() { }

    private Item(string itemName, string itemExtension, string folderId, long fileSize)
    {
        ItemName = itemName;
        Extension =  itemExtension;
        FolderId = folderId;
        ItemSize = fileSize;
    }

    public static Item Create(string itemName, string itemExtension, string folderId, long fileSize)
    {
        VerifyFolderId(folderId);

        return new Item(itemName, itemExtension, folderId, fileSize);
    }


    private static void VerifyFolderId(string folderId)
    {
        if (string.IsNullOrEmpty(folderId))
            throw new DomainException("Um arquivo deve estar vinculado com uma pasta");
    }

    public void ChangeName(string newFileName)
    {
        this.ItemName = newFileName;
    }

    public void Delete()
    {
        this.IsDeleted = true;
    }

}
