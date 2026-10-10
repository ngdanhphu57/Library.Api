using System.ComponentModel.DataAnnotations;

namespace Library.Api.Dtos;

public class NhanVienRequest
{
    [Required]
    [StringLength(100)]
    public string HoTen { get; set; } = string.Empty;

    [StringLength(15)]
    public string? DienThoai { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? ChucVu { get; set; }

    [StringLength(128, MinimumLength = 8)]
    public string? MatKhau { get; set; }
}