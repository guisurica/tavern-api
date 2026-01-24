namespace tavern_api.Commons.DTOs;

public record ChangeTavernDescriptionDTO
{
    public string TavernId { get; init; }
    public string? Description { get; init; }
}