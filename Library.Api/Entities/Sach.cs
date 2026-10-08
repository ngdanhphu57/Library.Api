using System;
using System.Collections.Generic;

namespace Library.Api.Entities;

public partial class Sach
{
    public int MaSach { get; set; }

    public string TenSach { get; set; } = null!;

    public string? TacGia { get; set; }

    public string? NhaXuatBans { get; set; }

    public int? NamXuatBan { get; set; }

    public decimal? GiaBia { get; set; }

    public int SoLuongTong { get; set; }

    public int SoLuongConLai { get; set; }

    public int? MaTheLoai { get; set; }

    public virtual ICollection<Chitietphieumuon> Chitietphieumuons { get; set; } = new List<Chitietphieumuon>();

    public virtual Theloai? MaTheLoaiNavigation { get; set; }
}
