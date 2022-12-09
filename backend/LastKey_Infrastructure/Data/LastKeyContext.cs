using LastKey_Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LastKey_Infrastructure.Data;

public class LastKeyContext : DbContext
{
    public LastKeyContext(DbContextOptions<LastKeyContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Lock> Locks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName).IsUnique();
        
        modelBuilder.Entity<Lock>()
            .HasIndex(l => new { l.UserId, l.LockName }).IsUnique();
        
        base.OnModelCreating(modelBuilder);
    }
}