using System.Xml.Linq;
using tavern_api.Commons;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Exceptions;

namespace tavern_api.Entities;

public sealed class Tavern : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; } = null;
    public int Capacity { get; private set; }
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; } = 0;
    public int CurrentExperience { get; private set; } = 0;
    public int LevelExperienceLimit { get; private set; } = 0;
    public List<GameDay> GameDays { get; private set; } = new();

    private static readonly int TavernLevelLimit = 20;

    private Tavern() { }

    private Tavern(string name, string description, int capacity)
    {
        Name = name;
        Description = description;
        Capacity = capacity;
        LevelExperienceLimit = Level * 100;
    }

    public Membership DeleteMembership(Membership membership)
    {
        membership.IsDeleted = true;
        return membership;
    }

    public void CanAddUserInTavern(int usersAlreadyInTavern)
    {
        if (usersAlreadyInTavern + 1 > this.Capacity)
            throw new DomainException("Capacidade da taverna não permite a adição de mais um usuário");
    }

    public void UserHasPermissionToPerformAction(Membership membership)
    {
        if (!membership.IsDm || membership.Status != Commons.Enums.MembershipStatus.ADMMIN)
            throw new DomainException("Usuário não tem permissão dentro da taverna para fazer isso.");
    }


    public void ChangeName(string name)
    {
        VerifyName(name);
        this.Name = name;
    }

    public void ChangeDescription(string? description)
    {
        VerifyDescription(description);
        this.Description = description;
    }

    public void ChangeCapacity(int capacity)
    {
        VerifyCapacity(capacity);
        this.Capacity = capacity;
    }

    public void LevelUp()
    {
        CheckIfCanLevelUp(this.Level);
        this.Level += 1;
        this.CurrentExperience = 0;
    }

    public void ExperienceGain(int gainedExperience)
    {
        this.CurrentExperience += gainedExperience;

        if (this.CurrentExperience >= LevelExperienceLimit)
        {
            LevelUp();
        }
    }

    #region Static Methods
    
    public static Tavern Create(string name, string description, int capacity)
    {
        VerifyName(name);
        VerifyDescription(description);
        VerifyCapacity(capacity);

        return new Tavern(name, description, capacity);
    }
    private static void VerifyName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da taverna não pode ser vazio");
        if (name.Length < 3)
            throw new DomainException("Nome da taverna deve ter no mínimo 3 caracteres");
        if (name.Length > 100)
            throw new DomainException("Nome da taverna deve ter no máximo 100 caracteres");
    }
    
    private static void VerifyDescription(string? description)
    {
        if (!string.IsNullOrEmpty(description))
        {
            if (description.Length < 3)
                throw new DomainException("Nome da taverna deve ter no mínimo 3 caracteres");
            if (description.Length > 255)
                throw new DomainException("Descrição da taverna deve ter no máximo 255 caracteres");
        }
    }
    
    private static void VerifyCapacity(int capacity)
    {
        if (capacity <= 0)
            throw new DomainException("Capacidade da taverna deve ser maior que zero");
    }

    private static void CheckIfCanLevelUp(int level)
    {
        if (level + 1 > TavernLevelLimit) 
            throw new DomainException("Nível máximo da taverna atingido");
    }

    #endregion
}
