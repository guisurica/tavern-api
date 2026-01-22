namespace tavern_api.Commons.DTOs;

public record CreateTavernDTO
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Capacity { get; set; }
}
