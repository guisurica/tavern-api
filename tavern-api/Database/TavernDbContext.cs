using Microsoft.EntityFrameworkCore;
using tavern_api.Entities;

namespace tavern_api.Database;

public class TavernDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Tavern> Taverns { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<GameDay> GameDays { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }

    public TavernDbContext(DbContextOptions<TavernDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<User>()
            .HasIndex(u => new { u.Username, u.Discriminator })
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Tavern>()
            .HasMany(t => t.GameDays)
            .WithOne(g => g.Tavern)
            .HasForeignKey(g => g.TavernId);
    }
}
