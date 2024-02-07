using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ExcursionsDomain.Model;

//namespace ExcursionsDomain.Model;
namespace ExcursionsInfrastructure;

public partial class ExcursionsDbContext : DbContext
{
    public ExcursionsDbContext()
    {
    }

    public ExcursionsDbContext(DbContextOptions<ExcursionsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Excursion> Excursions { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<PlacesExcursion> PlacesExcursions { get; set; }

    public virtual DbSet<Visitor> Visitors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-QNPQDTG\\SQLEXPRESS; Database=ExcursionsDB; Trusted_Connection=True; TrustServerCertificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(d => d.Excursions).WithMany(p => p.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoriesExcursion",
                    r => r.HasOne<Excursion>().WithMany()
                        .HasForeignKey("ExcursionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CategoriesExcursions_Excursions"),
                    l => l.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CategoriesExcursions_Categories"),
                    j =>
                    {
                        j.HasKey("CategoryId", "ExcursionId");
                        j.ToTable("CategoriesExcursions");
                    });
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cities_Countries");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Excursion>(entity =>
        {
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.Property(e => e.CoordinateX).HasMaxLength(50);
            entity.Property(e => e.CoordinateY).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.City).WithMany(p => p.Places)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Places_Cities");
        });

        modelBuilder.Entity<PlacesExcursion>(entity =>
        {
            entity.HasKey(e => new { e.PlaceId, e.ExcursionId });

            entity.HasOne(d => d.Excursion).WithMany(p => p.PlacesExcursions)
                .HasForeignKey(d => d.ExcursionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlacesExcursions_Excursions");

            entity.HasOne(d => d.Place).WithMany(p => p.PlacesExcursions)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlacesExcursions_Places");
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);

            entity.HasMany(d => d.Excursions).WithMany(p => p.Visitors)
                .UsingEntity<Dictionary<string, object>>(
                    "VisitorsExcursion",
                    r => r.HasOne<Excursion>().WithMany()
                        .HasForeignKey("ExcursionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_VisitorsExcursions_Excursions"),
                    l => l.HasOne<Visitor>().WithMany()
                        .HasForeignKey("VisitorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_VisitorsExcursions_Visitors"),
                    j =>
                    {
                        j.HasKey("VisitorId", "ExcursionId");
                        j.ToTable("VisitorsExcursions");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
