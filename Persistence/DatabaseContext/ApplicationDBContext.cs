using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.Models;

namespace Persistence.DatabaseContext;

public class ApplicationDBContext : IdentityDbContext<User>
{
    public const string AdminRoleId = "03a8da38-b8ff-4ea8-a2eb-f5204f06c72e";
    public const string UsersRoleId = "0415194d-59ef-4707-8749-0a807bb28ab0";

    public DbSet<Todos> Todos => Set<Todos>();
    public DbSet<TodoDetails1> TodoDetails => Set<TodoDetails1>();

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
            entity.Property(e => e.UserId).HasMaxLength(255);
            entity.Property(e => e.day).IsRequired();
            entity.Property(e => e.todayDate).IsRequired();
            entity.Property(e => e.note).IsRequired();
            entity.Property(e => e.Version).IsConcurrencyToken();
            entity.HasIndex(e => new { e.UserId, e.todayDate });

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TodoDetails1>(entity =>
        {
            entity.ToTable("TodoDetail");
            entity.HasKey(e => e.TodoDetailId);
            entity.Property(e => e.Activity).IsRequired();
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.DetailNote).IsRequired();

            entity.HasOne(d => d.Todo)
                  .WithMany(p => p.TodoDetails)
                  .HasForeignKey(d => d.TodoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = AdminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = UsersRoleId, Name = "Users", NormalizedName = "USERS" });
    }
}
