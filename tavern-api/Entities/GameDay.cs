using System;
using tavern_api.Commons;
using tavern_api.Commons.Exceptions;

namespace tavern_api.Entities;

public sealed class GameDay : BaseEntity
{
    public DateTime ScheduledAt { get; private set; }
    public string? Notes { get; private set; }

    public string TavernId { get; private set; } = null!;
    public Tavern Tavern { get; private set; } = null!;
    public bool IsConcluded { get; private set; } = false;

    private GameDay() { }

    private GameDay(string tavernId, DateTime scheduledAt, string? notes)
    {
        TavernId = tavernId;
        ScheduledAt = scheduledAt;
        Notes = notes;
    }

    public static GameDay Create(string tavernId, DateTime scheduledAt, string? notes)
    {
        VerifyTavernId(tavernId);
        VerifyScheduledAt(scheduledAt);
        VerifyNotes(notes);

        return new GameDay(tavernId, scheduledAt, notes);
    }

    private static void VerifyTavernId(string tavernId)
    {
        if (string.IsNullOrWhiteSpace(tavernId))
            throw new DomainException("A data de jogo precisa de uma taverna.");
    }

    private static void VerifyScheduledAt(DateTime scheduledAt)
    {
        if (scheduledAt < DateTime.UtcNow.AddMinutes(-5))
            throw new DomainException("Data do jogo não pode ser no passado.");
    }

    private static void VerifyNotes(string? notes)
    {
        if (notes != null && notes.Length > 1000)
            throw new DomainException("Notas não podem exceder 1000 caracteres.");
    }

    public void ConcludeGameDay()
    {
        this.IsConcluded = true;
    }

    public void UpdateNotes(string? notes)
    {
        VerifyNotes(notes);
        Notes = notes;
    }

    public void Reschedule(DateTime newDate)
    {
        VerifyScheduledAt(newDate);
        ScheduledAt = newDate;
    }
}