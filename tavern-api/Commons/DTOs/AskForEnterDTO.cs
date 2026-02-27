namespace tavern_api.Commons.DTOs;

public record AskForEnterDTO
{
    public string ReceriverEmail { get; init; }
    public string UserId { get; init; }
    public string TavernId { get; init; }
    public string InviteMessage { get; init; }
}
