using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Enums;
using RestaurantApp.Infrastructure.Identity;

namespace RestaurantApp.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Employe> Employes => Set<Employe>();
    public DbSet<Serveur> Serveurs => Set<Serveur>();
    public DbSet<Chef> Chefs => Set<Chef>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<TableRestaurant> Tables => Set<TableRestaurant>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<Plat> Plats => Set<Plat>();
    public DbSet<Categorie> Categories => Set<Categorie>();
    public DbSet<Avis> Avis => Set<Avis>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 

        // ── RESTAURANT ──────────────────────────────────────────
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Nom)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(r => r.Adresse)
                  .IsRequired()
                  .HasMaxLength(200);
            entity.Property(r => r.Type)
                  .HasConversion<string>()  
                  .IsRequired();
            entity.Property(r => r.Telephone)
                  .HasMaxLength(20)
                  .IsRequired(false);
            entity.Property(r => r.NoteGlobale)
                  .HasDefaultValue(0.0);
            entity.ToTable("Restaurants");
        });

        // ── EMPLOYE ──────────────────────────────
        modelBuilder.Entity<Employe>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cin).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(60);
            entity.Property(e => e.Prenom).IsRequired().HasMaxLength(60);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.NumTel).HasMaxLength(20).IsRequired(false);
            entity.Property(e => e.Salaire)
                  .IsRequired()
                  .HasColumnType("decimal(10,2)");
            entity.HasDiscriminator<string>("TypeEmploye")
                  .HasValue<Serveur>("Serveur")
                  .HasValue<Chef>("Chef");
            entity.HasOne(e => e.Restaurant)
                  .WithMany(r => r.Employes)
                  .HasForeignKey(e => e.RestaurantId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.ToTable("Employes");
        });

        modelBuilder.Entity<Serveur>(entity =>
        {
            entity.Property(s => s.Note).HasDefaultValue(0);
            entity.Property(s => s.Zone).HasMaxLength(50).IsRequired(false);
        });

        modelBuilder.Entity<Chef>(entity =>
        {
            entity.Property(c => c.Specialite).IsRequired().HasMaxLength(100);
            entity.Property(c => c.AnneeExp).HasDefaultValue(0);
        });

        // ── CLIENT ──────────────────────────────────────────────
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(c => c.ClientId);
            entity.Property(c => c.Nom).IsRequired().HasMaxLength(60);
            entity.Property(c => c.Prenom).IsRequired().HasMaxLength(60);
            entity.Property(c => c.Adresse).HasMaxLength(200).IsRequired(false);
            entity.Property(c => c.PointFidelite).HasDefaultValue(0);
            entity.Property(c => c.Email).HasMaxLength(150).IsRequired(false);
            entity.Property(c => c.UserId).HasMaxLength(450).IsRequired(false);
            entity.ToTable("Clients");
        });

        // ── TABLE RESTAURANT ────────────────────────────────────
        modelBuilder.Entity<TableRestaurant>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Numero).IsRequired();
            entity.Property(t => t.Capacite).IsRequired();
            entity.Property(t => t.Disponible).HasDefaultValue(true);
            entity.HasOne(t => t.Restaurant)
                  .WithMany(r => r.Tables)
                  .HasForeignKey(t => t.RestaurantId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.ToTable("TablesRestaurant");

            // Contrainte unique : numéro de table unique par restaurant
            entity.HasIndex(t => new { t.RestaurantId, t.Numero }).IsUnique();
        });

        // ── RESERVATION ─────────────────────────────────────────
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Date).IsRequired();
            entity.Property(r => r.NbPersonnes).IsRequired();
            entity.Property(r => r.StatutReservation).HasDefaultValue(false);
            entity.Property(r => r.Notes).HasMaxLength(500).IsRequired(false);
            entity.Property(r => r.DateCreation).HasDefaultValueSql("GETDATE()");
            entity.HasOne(r => r.Restaurant)
                  .WithMany(r => r.Reservations)
                  .HasForeignKey(r => r.RestaurantId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(r => r.Client)
                  .WithMany(c => c.Reservations)
                  .HasForeignKey(r => r.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(r => r.TableRestaurant)
                  .WithMany(t => t.Reservations)
                  .HasForeignKey(r => r.TableRestaurantId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.ToTable("Reservations");
        });

        // ── MENU ────────────────────────────────────────────────
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Libelle).IsRequired().HasMaxLength(100);
            entity.Property(m => m.Description).HasMaxLength(500).IsRequired(false);
            entity.Property(m => m.PrixTotal)
                  .IsRequired()
                  .HasColumnType("decimal(8,2)");
            entity.Property(m => m.DisponibleDu).IsRequired(false);
            entity.HasOne(m => m.Restaurant)
                  .WithMany(r => r.Menus)
                  .HasForeignKey(m => m.RestaurantId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.ToTable("Menus");
        });

        // ── CATEGORIE ───────────────────────────────────────────
        modelBuilder.Entity<Categorie>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Libelle).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Description).HasMaxLength(200).IsRequired(false);
            entity.ToTable("Categories");
        });

        // ── PLAT ────────────────────────────────────────────────
        modelBuilder.Entity<Plat>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nom).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Description).HasMaxLength(500).IsRequired(false);
            entity.Property(p => p.Prix)
                  .IsRequired()
                  .HasColumnType("decimal(6,2)");
            entity.Property(p => p.Disponible).HasDefaultValue(true);
            entity.HasOne(p => p.Categorie)
                  .WithMany(c => c.Plats)
                  .HasForeignKey(p => p.CategorieId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(p => p.Menu)
                  .WithMany(m => m.Plats)
                  .HasForeignKey(p => p.MenuId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.ToTable("Plats");
        });

        // ── AVIS ────────────────────────────────────────────────
        modelBuilder.Entity<Avis>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Note).IsRequired();
            entity.Property(a => a.Commentaire).HasMaxLength(1000).IsRequired(false);
            entity.Property(a => a.DateAvis).HasDefaultValueSql("GETDATE()");
            entity.HasOne(a => a.Restaurant)
                  .WithMany(r => r.Avis)
                  .HasForeignKey(a => a.RestaurantId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(a => a.Client)
                  .WithMany(c => c.Avis)
                  .HasForeignKey(a => a.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.ToTable("Avis");
        });
    }
}