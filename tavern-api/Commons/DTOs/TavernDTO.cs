namespace tavern_api.Commons.DTOs;

public record TavernDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public int Level { get; set; }
    public int CurrentExperience { get; set; }
    public int LevelExperienceLimit { get; set; }
    public List<UserTavernDTO> Users { get; set; }
    public List<TavernGameDaysDTO> GameDays { get; set; }
}
