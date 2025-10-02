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
    {
        //=> optionsBuilder.UseNpgsql("Host=hopper.proxy.rlwy.net;Port=17710;Database=railway;Username=postgres;Password=lkxgcHxVHYPbGRRGTIbAYbrixXjriKpc;SSL Mode=Require;Trust Server Certificate=true");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produit>(entity =>
        {
            entity.ToTable("t_e_produit_prd");
            entity.HasKey(e => e.IdProduit);

            entity.Property(e => e.IdProduit)
                  .ValueGeneratedOnAdd()
                  .UseIdentityColumn();

            entity.HasOne(p => p.MarqueProduitNavigation)
                  .WithMany(m => m.Produits)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_t_e_produit_prd_t_e_marque_mrq");

            entity.HasOne(p => p.TypeProduitNavigation)
                  .WithMany(t => t.Produits)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_t_e_produit_prd_t_e_typeproduit_typ");
        });

        modelBuilder.Entity<TypeProduit>(entity =>
        {
            entity.ToTable("t_e_typeproduit_typ");
            entity.HasKey(e => e.IdTypeProduit);

            entity.Property(e => e.IdTypeProduit)
                  .ValueGeneratedOnAdd()
                  .UseIdentityColumn();
        });

        modelBuilder.Entity<Marque>(entity =>
        {
            entity.ToTable("t_e_marque_mrq");
            entity.HasKey(e => e.IdMarque);

            entity.Property(e => e.IdMarque)
                  .ValueGeneratedOnAdd()
                  .UseIdentityColumn();
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("t_e_image_img");
            entity.HasKey(e => e.IdImage);

            entity.Property(e => e.IdImage)
                  .ValueGeneratedOnAdd()
                  .UseIdentityColumn();

            entity.HasOne(i => i.ProduitNavigation)
                  .WithMany(p => p.Images)
                  .HasForeignKey(i => i.IdProduit)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_t_e_image_img_t_e_produit_prd");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}