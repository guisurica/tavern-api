namespace tavern_api.Commons.DTOs;

public record RemoveUserTavernDTO
{
    public string UserId { get; set; }  
    public string TavernId { get; set; }
}
