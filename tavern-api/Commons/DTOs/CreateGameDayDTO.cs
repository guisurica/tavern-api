using System;

namespace tavern_api.Commons.DTOs;

public record CreateGameDayDTO
{
    public string TavernId { get; init; }
    public DateTime ScheduledAt { get; init; }
    public string? Notes { get; init; }
}