using System;

namespace tavern_api.Commons.DTOs;

public record GameDayDTO
{
    public string Id { get; init; }
    public string TavernId { get; init; }
    public DateTime ScheduledAt { get; init; }
    public string? Notes { get; init; }
    public bool IsConcluded { get; init; }
}