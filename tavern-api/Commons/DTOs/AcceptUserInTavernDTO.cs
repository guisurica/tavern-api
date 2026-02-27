namespace tavern_api.Commons.DTOs;


public record AcceptUserInTavernDTO
{
    public string UserIdWhoWantsEnterTavern { get; init; }
    public string TavernId { get; init; }
    public string UserWhoWillAcceptsNewUserTavern { get; init; }
}
