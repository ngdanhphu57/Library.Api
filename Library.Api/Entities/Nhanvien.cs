using System;
using System.Collections.Generic;

namespace Library.Api.Entities;

public partial class Nhanvien
{
    public int MaNv { get; set; }

    public string HoTen { get; set; } = null!;

    public string? DienThoai { get; set; }

    public string? Email { get; set; }

    public string MatKhau { get; set; } = null!;

    public string? ChucVu { get; set; }

    public virtual ICollection<Phieumuon> Phieumuons { get; set; } = new List<Phieumuon>();
}
