using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SkripsiIvan.Models
{
    public partial class SkripsiIvanContext : DbContext
    {
        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<Gejala> Gejala { get; set; }
        public virtual DbSet<Kategori> Kategori { get; set; }
        public virtual DbSet<Penyakit> Penyakit { get; set; }
        public virtual DbSet<RekamMedik> RekamMedik { get; set; }
        public virtual DbSet<Rule> Rule { get; set; }

        public SkripsiIvanContext(DbContextOptions<SkripsiIvanContext> options): base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-S4JTTFJ;Initial Catalog=SkripsiIvan;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.IdAdmin)
                    .HasName("PK_Admin");

                entity.Property(e => e.IdAdmin).HasColumnName("ID_Admin");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.PasswordAdmin)
                    .IsRequired()
                    .HasColumnName("Password_Admin")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.UsernameAdmin)
                    .IsRequired()
                    .HasColumnName("Username_Admin")
                    .HasColumnType("varchar(30)");
            });

            modelBuilder.Entity<Gejala>(entity =>
            {
                entity.HasKey(e => e.IdGejala)
                    .HasName("PK_Gejala");

                entity.Property(e => e.IdGejala).HasColumnName("ID_Gejala");

                entity.Property(e => e.KategoriId)
                    .HasColumnName("Kategori_ID")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.GambarGejala)
                    .HasColumnName("GambarGejala")
                    .HasDefaultValueSql("notfound.jpg");

                entity.Property(e => e.NamaGejala)
                    .IsRequired()
                    .HasColumnName("Nama_Gejala")
                    .HasColumnType("varchar(125)");

                entity.Property(e => e.ParentId).HasColumnName("Parent_ID");

                entity.HasOne(d => d.Kategori)
                    .WithMany(p => p.Gejala)
                    .HasForeignKey(d => d.KategoriId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Gejala_Kategori");
            });

            modelBuilder.Entity<Kategori>(entity =>
            {
                entity.HasKey(e => e.KategoriPenyakit)
                    .HasName("PK_Kategori");

                entity.Property(e => e.KategoriPenyakit).HasColumnName("Kategori_Penyakit");

                entity.Property(e => e.NamaKategori)
                    .IsRequired()
                    .HasColumnName("Nama_Kategori")
                    .HasColumnType("varchar(125)");
            });

            modelBuilder.Entity<Penyakit>(entity =>
            {
                entity.HasKey(e => e.IdPenyakit)
                    .HasName("PK_Penyakit");

                entity.Property(e => e.IdPenyakit).HasColumnName("ID_Penyakit");

                entity.Property(e => e.KategoriId)
                    .HasColumnName("Kategori_ID")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.NamaPenyakit)
                    .IsRequired()
                    .HasColumnName("Nama_Penyakit")
                    .HasColumnType("varchar(125)");

                entity.Property(e => e.Solusi)
                    .IsRequired()
                    .HasColumnType("varchar(max)");

                entity.HasOne(d => d.Kategori)
                    .WithMany(p => p.Penyakit)
                    .HasForeignKey(d => d.KategoriId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Penyakit_Kategori");
            });

            modelBuilder.Entity<RekamMedik>(entity =>
            {
                entity.HasKey(e => e.IdRekammedik)
                    .HasName("PK_RekamMedik");

                entity.Property(e => e.IdRekammedik).HasColumnName("ID_Rekammedik");

                entity.Property(e => e.IdGejala).HasColumnName("ID_Gejala");
         
                entity.Property(e => e.IdPenyakit).HasColumnName("ID_Penyakit");

                entity.Property(e => e.KategoriId)
                    .HasColumnName("Kategori_ID")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Kode)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.HasOne(d => d.IdGejalaNavigation)
                    .WithMany(p => p.RekamMedik)
                    .HasForeignKey(d => d.IdGejala)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RekamMedik_Gejala");

                entity.HasOne(d => d.IdPenyakitNavigation)
                    .WithMany(p => p.RekamMedik)
                    .HasForeignKey(d => d.IdPenyakit)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RekamMedik_Penyakit");

                entity.HasOne(d => d.Kategori)
                    .WithMany(p => p.RekamMedik)
                    .HasForeignKey(d => d.KategoriId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RekamMedik_Kategori");
            });

            modelBuilder.Entity<Rule>(entity =>
            {
                entity.HasKey(e => e.IdRule)
                    .HasName("PK_Rule");

                entity.Property(e => e.IdRule).HasColumnName("ID_Rule");

                entity.Property(e => e.IdGejala).HasColumnName("ID_Gejala");

                entity.Property(e => e.IdPenyakit).HasColumnName("ID_Penyakit");

                entity.Property(e => e.KategoriId)
                    .HasColumnName("Kategori_ID")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.IdGejalaNavigation)
                    .WithMany(p => p.Rule)
                    .HasForeignKey(d => d.IdGejala)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Rule_Gejala");

                entity.HasOne(d => d.IdPenyakitNavigation)
                    .WithMany(p => p.Rule)
                    .HasForeignKey(d => d.IdPenyakit)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Rule_Penyakit");

                entity.HasOne(d => d.Kategori)
                    .WithMany(p => p.Rule)
                    .HasForeignKey(d => d.KategoriId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Rule_Kategori");
            });
        }
    }
}