namespace tavern_api.Commons.DTOs;

public record ChangeTavernCapacityDTO
{
    public string TavernId { get; init; }
    public int Capacity { get; init; }
}