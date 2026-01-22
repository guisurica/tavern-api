namespace tavern_api.Commons.DTOs;

public record ResheduleGameDayDTO
{
    public DateTime NewDate { get; set; }
    public string Id { get; set; }
    public string TavernId { get; set; }
    public string? Notes { get; set; }
}
