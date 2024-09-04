using Persistence.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.DatabaseContext;

public class ApplicationDBContext : IdentityDbContext<User>
{
    public DbSet<Todo1> Todos { get; set; }
    public DbSet<TodoDetail1> TodoDetails { get; set; }
    public DbSet<UserToken> UserToken { get; set; }

    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Todo1>(entity =>
        {
            entity.ToTable("Todo");
            entity.HasKey(e => e.TodoId);
            entity.Property(e => e.TodoId).HasDefaultValueSql("UUID()");
            entity.Property(e => e.Day).IsRequired();
            entity.Property(e => e.Note).IsRequired();
            entity.Property(e => e.DetailCount).IsRequired();
        });

        modelBuilder.Entity<TodoDetail1>(entity =>
        {
            entity.ToTable("TodoDetail");
            entity.HasKey(e => e.TodoDetailId);
            entity.Property(e => e.TodoDetailId).HasDefaultValueSql("UUID()");
            entity.Property(e => e.Activity).IsRequired();
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.DetailNote).IsRequired();

            entity.HasOne(d => d.Todo)
                  .WithMany(p => p.TodoDetails)
                  .HasForeignKey(d => d.TodoId);
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.ToTable("UserToken");
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.Expiry).IsRequired();
        });
    }
}
