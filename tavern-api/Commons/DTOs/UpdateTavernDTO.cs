namespace tavern_api.Commons.DTOs;

public record UpdateTavernDTO
{
    public string TavernId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public int? Capacity { get; init; }
}