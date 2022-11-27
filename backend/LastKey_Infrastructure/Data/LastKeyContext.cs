﻿using LastKey_Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LastKey_Infrastructure.Data;

public class LastKeyContext : DbContext
{
    public LastKeyContext(DbContextOptions<LastKeyContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName).IsUnique();
        base.OnModelCreating(modelBuilder);
    }
}