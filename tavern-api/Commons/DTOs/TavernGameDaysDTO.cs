namespace tavern_api.Commons.DTOs;

public record TavernGameDaysDTO
{
    public string Id { get; init; }
    public string TavernId { get; init; }
    public DateTime ScheduleAt { get; init; }
    public string? Notes { get; init; }
    public bool IsConcluded { get; set; }
}
