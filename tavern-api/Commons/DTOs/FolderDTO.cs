namespace tavern_api.Commons.DTOs;

public record FolderDTO
{
    public string FolderName { get; set; }
    public string Id { get; set; }
    public string MembershipId { get; set; }
    public string AssignedUsername { get; set; }
    public string AssignedUserId { get; set; }
    public int TotalItems { get; set; }
}
