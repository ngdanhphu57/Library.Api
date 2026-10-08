using System;
using System.Collections.Generic;

namespace Library.Api.Entities;

public partial class Theloai
{
    public int MaTheLoai { get; set; }

    public string TenTheLoai { get; set; } = null!;

    public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
}
