namespace tavern_api.Commons.DTOs;

public record CreateFileDTO
{
    public IFormFile FormFile { get; set; }
    public string TavernId { get; set; }
    public string FolderId { get; set; }
}
