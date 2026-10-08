using System;
using System.Collections.Generic;

namespace Library.Api.Entities;

public partial class Chitietphieumuon
{
    public int MaPhieu { get; set; }

    public int MaSach { get; set; }

    public int? SoLuong { get; set; }

    public DateTime? NgayTraThucTe { get; set; }

    public string? TinhTrangKhiMuon { get; set; }

    public string? TinhTrangKhiTra { get; set; }

    public decimal? TienPhat { get; set; }

    public virtual Phieumuon MaPhieuNavigation { get; set; } = null!;

    public virtual Sach MaSachNavigation { get; set; } = null!;
}
