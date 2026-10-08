using System;
using System.Collections.Generic;
using Library.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Data;

public partial class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chitietphieumuon> Chitietphieumuons { get; set; }

    public virtual DbSet<Docgium> Docgia { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<Phieumuon> Phieumuons { get; set; }

    public virtual DbSet<Sach> Saches { get; set; }

    public virtual DbSet<Theloai> Theloais { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chitietphieumuon>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieu, e.MaSach }).HasName("PRIMARY");

            entity.ToTable("chitietphieumuon");

            entity.HasIndex(e => e.MaSach, "MaSach");

            entity.Property(e => e.NgayTraThucTe).HasColumnType("datetime");
            entity.Property(e => e.SoLuong).HasDefaultValueSql("'1'");
            entity.Property(e => e.TienPhat).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.TinhTrangKhiMuon).HasMaxLength(100);
            entity.Property(e => e.TinhTrangKhiTra).HasMaxLength(100);

            entity.HasOne(d => d.MaPhieuNavigation).WithMany(p => p.Chitietphieumuons)
                .HasForeignKey(d => d.MaPhieu)
                .HasConstraintName("chitietphieumuon_ibfk_1");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.Chitietphieumuons)
                .HasForeignKey(d => d.MaSach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chitietphieumuon_ibfk_2");
        });

        modelBuilder.Entity<Docgium>(entity =>
        {
            entity.HasKey(e => e.MaDocGia).HasName("PRIMARY");

            entity.ToTable("docgia");

            entity.Property(e => e.DienThoai).HasMaxLength(15);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.Lop).HasMaxLength(50);
            entity.Property(e => e.NgayHetHan).HasColumnType("date");
            entity.Property(e => e.NgayLapThe)
                .HasDefaultValueSql("'curdate()'")
                .HasColumnType("date");
            entity.Property(e => e.NgaySinh).HasColumnType("date");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PRIMARY");

            entity.ToTable("nhanvien");

            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.DienThoai).HasMaxLength(15);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(100);
        });

        modelBuilder.Entity<Phieumuon>(entity =>
        {
            entity.HasKey(e => e.MaPhieu).HasName("PRIMARY");

            entity.ToTable("phieumuon");

            entity.HasIndex(e => e.MaDocGia, "MaDocGia");

            entity.HasIndex(e => e.MaNv, "MaNV");

            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.NgayHenTra).HasColumnType("datetime");
            entity.Property(e => e.NgayMuon)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Dang muon'");

            entity.HasOne(d => d.MaDocGiaNavigation).WithMany(p => p.Phieumuons)
                .HasForeignKey(d => d.MaDocGia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("phieumuon_ibfk_1");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Phieumuons)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("phieumuon_ibfk_2");
        });

        modelBuilder.Entity<Sach>(entity =>
        {
            entity.HasKey(e => e.MaSach).HasName("PRIMARY");

            entity.ToTable("sach");

            entity.HasIndex(e => e.MaTheLoai, "MaTheLoai");

            entity.Property(e => e.NhaXuatBans).HasMaxLength(100);
            entity.Property(e => e.TacGia).HasMaxLength(100);
            entity.Property(e => e.TenSach).HasMaxLength(200);

            entity.HasOne(d => d.MaTheLoaiNavigation).WithMany(p => p.Saches)
                .HasForeignKey(d => d.MaTheLoai)
                .HasConstraintName("sach_ibfk_1");
        });

        modelBuilder.Entity<Theloai>(entity =>
        {
            entity.HasKey(e => e.MaTheLoai).HasName("PRIMARY");

            entity.ToTable("theloai");

            entity.Property(e => e.TenTheLoai).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
