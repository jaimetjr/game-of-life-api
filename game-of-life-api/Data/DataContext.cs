using game_of_life_api.Models;
using Microsoft.EntityFrameworkCore;

namespace game_of_life_api.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Board> Boards => Set<Board>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var b = modelBuilder.Entity<Board>();
        b.ToTable("Boards");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired(false);

        b.Property(x => x.Rows).IsRequired();
        b.Property(x => x.Cols).IsRequired();
        b.Property(x => x.CellsJson).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();

        b.HasIndex(x => x.CreatedAt);
    }

}
