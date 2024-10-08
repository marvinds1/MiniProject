﻿using Persistence.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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
            entity.Property(e => e.TodoId);
            entity.Property(e => e.day).IsRequired();
            entity.Property(e => e.todayDate).IsRequired();
            entity.Property(e => e.note).IsRequired();
            entity.Property(e => e.detailCount).IsRequired();
        });

        modelBuilder.Entity<TodoDetails1>(entity =>
        {
            entity.ToTable("TodoDetail");
            entity.HasKey(e => e.TodoDetailId);
            entity.Property(e => e.TodoDetailId);
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

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var adminRoleId = Guid.NewGuid().ToString();
        var usersRoleId = Guid.NewGuid().ToString();
        var adminUserId = Guid.NewGuid().ToString();

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            }, new IdentityRole
            {
                Id = usersRoleId,
                Name = "Users",
                NormalizedName = "USERS"
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Admin123."),
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = "Administrator"
            }
        );

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            }
        );
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableDetailedErrors();
    }
}