using Persistence.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.DatabaseContext;

public class ApplicationDBContext : IdentityDbContext<User>
{
    public DbSet<Todos> Todos { get; set; }
    public DbSet<TodoDetails1> TodoDetails { get; set; }
    public DbSet<UserToken> UserToken { get; set; }

    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Todos>(entity =>
        {
            entity.ToTable("Todo");
            entity.HasKey(e => e.TodoId);
            entity.Property(e => e.TodoId).HasDefaultValueSql("UUID()");
            entity.Property(e => e.day).IsRequired();
            entity.Property(e => e.todayDate).IsRequired();
            entity.Property(e => e.note).IsRequired();
            entity.Property(e => e.detailCount).IsRequired();
        });

        modelBuilder.Entity<TodoDetails1>(entity =>
        {
            entity.ToTable("TodoDetail");
            entity.HasKey(e => e.TodoDetailId);
            entity.Property(e => e.TodoDetailId).HasDefaultValueSql("UUID()");
            entity.Property(e => e.Activity).IsRequired();
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.DetailNote).IsRequired();
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.ToTable("UserToken");
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.Expiry).IsRequired();
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableDetailedErrors();
    }
}
