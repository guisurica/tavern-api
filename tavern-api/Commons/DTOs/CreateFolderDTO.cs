namespace tavern_api.Commons.DTOs;

public record CreateFolderDTO
{
    public string UserId { get; set; }
    public string FolderName { get; set; }
    public string TavernId { get; set; }
}
