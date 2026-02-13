using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ParkHub_API.Models;

public partial class ParkHubContext : DbContext
{
    public ParkHubContext()
    {
    }

    public ParkHubContext(DbContextOptions<ParkHubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AreaParkir> AreaParkirs { get; set; }

    public virtual DbSet<JenisKendaraan> JenisKendaraans { get; set; }

    public virtual DbSet<Kendaraan> Kendaraans { get; set; }

    public virtual DbSet<LogAktivita> LogAktivitas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tarif> Tarifs { get; set; }

    public virtual DbSet<Transaksi> Transaksis { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=ONDEMANDE\\SQLEXPRESS;Initial Catalog=ParkHub;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AreaParkir>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AreaPark__3213E83FD83AEF9C");

            entity.ToTable("AreaParkir");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Kapasitas).HasColumnName("kapasitas");
            entity.Property(e => e.NamaArea)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nama_area");
        });

        modelBuilder.Entity<JenisKendaraan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JenisKen__3213E83F486F9D55");

            entity.ToTable("JenisKendaraan");

            entity.HasIndex(e => e.Jenis, "UQ__JenisKen__2E880327CD76E804").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Jenis)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("jenis");
        });

        modelBuilder.Entity<Kendaraan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Kendaraa__3213E83F35E27C89");

            entity.ToTable("Kendaraan");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ImagePath).HasColumnName("imagePath");
            entity.Property(e => e.JenisKendaraanId).HasColumnName("jenis_kendaraan_id");
            entity.Property(e => e.Pemilik)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pemilik");
            entity.Property(e => e.PlatNomor)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("plat_nomor");
            entity.Property(e => e.Warna)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("warna");

            entity.HasOne(d => d.JenisKendaraan).WithMany(p => p.Kendaraans)
                .HasForeignKey(d => d.JenisKendaraanId)
                .HasConstraintName("FK_Kendaraan_JenisKendaraan");
        });

        modelBuilder.Entity<LogAktivita>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LogAktiv__3213E83F88D49570");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aktivitas)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("aktivitas");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WaktuAktivitas)
                .HasColumnType("datetime")
                .HasColumnName("waktu_aktivitas");

            entity.HasOne(d => d.User).WithMany(p => p.LogAktivita)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LogAktivi__user___628FA481");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3213E83FD4979484");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__783254B1712F4689").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Tarif>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tarif__3213E83F006C5952");

            entity.ToTable("Tarif");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JenisKendaraanId).HasColumnName("jenis_kendaraan_id");
            entity.Property(e => e.TarifPerJam)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("tarif_per_jam");

            entity.HasOne(d => d.JenisKendaraan).WithMany(p => p.Tarifs)
                .HasForeignKey(d => d.JenisKendaraanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarif_JenisKendaraan");
        });

        modelBuilder.Entity<Transaksi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transaks__3213E83FAD63BF88");

            entity.ToTable("Transaksi");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AreaId).HasColumnName("area_id");
            entity.Property(e => e.BiayaTotal)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("biaya_total");
            entity.Property(e => e.Durasi).HasColumnName("durasi");
            entity.Property(e => e.KendaraanId).HasColumnName("kendaraan_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TarifId).HasColumnName("tarif_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WaktuKeluar)
                .HasColumnType("datetime")
                .HasColumnName("waktu_keluar");
            entity.Property(e => e.WaktuMasuk)
                .HasColumnType("datetime")
                .HasColumnName("waktu_masuk");

            entity.HasOne(d => d.Area).WithMany(p => p.Transaksis)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Transaksi_AreaParkir");

            entity.HasOne(d => d.Kendaraan).WithMany(p => p.Transaksis)
                .HasForeignKey(d => d.KendaraanId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Transaksi_Kendaraan");

            entity.HasOne(d => d.Tarif).WithMany(p => p.Transaksis)
                .HasForeignKey(d => d.TarifId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Transaksi_Tarif");

            entity.HasOne(d => d.User).WithMany(p => p.Transaksis)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Transaksi_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83FED54E9B4");

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC5725301CB8C").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NamaLengkap)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nama_lengkap");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__role_id__4D94879B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
