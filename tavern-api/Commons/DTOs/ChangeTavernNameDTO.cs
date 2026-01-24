namespace tavern_api.Commons.DTOs;

public record ChangeTavernNameDTO
{
    public string TavernId { get; init; }
    public string Name { get; init; }
}