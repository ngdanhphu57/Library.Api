using System;
using System.Collections.Generic;

namespace Library.Api.Entities;

public partial class Docgium
{
    public int MaDocGia { get; set; }

    public string HoTen { get; set; } = null!;

    public DateTime? NgaySinh { get; set; }

    public string? Lop { get; set; }

    public string? DienThoai { get; set; }

    public string? Email { get; set; }

    public DateTime? NgayLapThe { get; set; }

    public DateTime? NgayHetHan { get; set; }

    public virtual ICollection<Phieumuon> Phieumuons { get; set; } = new List<Phieumuon>();
}
