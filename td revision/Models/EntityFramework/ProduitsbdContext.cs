using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using td_revision.Models;

namespace td_revision.Models.EntityFramework;

public partial class ProduitsbdContext : DbContext
{
    public DbSet<Produit> Produits { get; set; }
    public DbSet<Marque> Marques { get; set; }
    public DbSet<TypeProduit> TypeProduits { get; set; }
    public DbSet<Image> Images { get; set; }

    public ProduitsbdContext()
    {
    }

    public ProduitsbdContext(DbContextOptions<ProduitsbdContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=hopper.proxy.rlwy.net;Port=17710;Database=railway;Username=postgres;Password=lkxgcHxVHYPbGRRGTIbAYbrixXjriKpc;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produit>(entity =>
        {
            entity.HasKey(e => e.IdProduit);

            entity.HasOne(p => p.MarqueProduitNavigation)
            .WithMany(m => m.Produits)
            .OnDelete(deleteBehavior: DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_produits_marque");


        });

        modelBuilder.Entity<TypeProduit>(entity =>
        {
            entity.HasKey(e => e.IdTypeProduit);

            entity.HasMany(p => p.Produits)
            .WithOne(m => m.TypeProduitNavigation)
            .OnDelete(deleteBehavior: DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_produits_type_produit");


        });


        modelBuilder.Entity<Marque>(entity =>
        {
            entity.HasKey(e => e.IdMarque);

            entity.HasMany(p => p.Produits)
            .WithOne(m => m.MarqueProduitNavigation)
            .OnDelete(deleteBehavior: DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_produits_marque");


        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.IdImage);

            entity.HasOne(i => i.ProduitNavigation)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.IdProduit)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_images_produit");
        });

        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}
