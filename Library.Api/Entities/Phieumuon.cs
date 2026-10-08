using System;
using System.Collections.Generic;

namespace Library.Api.Entities;

public partial class Phieumuon
{
    public int MaPhieu { get; set; }

    public int MaDocGia { get; set; }

    public int MaNv { get; set; }

    public DateTime? NgayMuon { get; set; }

    public DateTime NgayHenTra { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<Chitietphieumuon> Chitietphieumuons { get; set; } = new List<Chitietphieumuon>();

    public virtual Docgium MaDocGiaNavigation { get; set; } = null!;

    public virtual Nhanvien MaNvNavigation { get; set; } = null!;
}
