namespace tavern_api.Commons.DTOs;
public record ItemDTO
{
    public string Id { get; set; }
    public string ItemName { get; set; }
    public string FolderId { get; set; }
    public string FolderName { get; set; }
    public string TavernId { get; set; }
    public string TavernName { get; set; }
    public string UserSignedInId { get; set; }
    public string UserSignedInUsername { get; set; }
    public long Size { get; set; }
    public string Extension { get; set; }
    public DateTime CreatedAt { get; set; }
}
