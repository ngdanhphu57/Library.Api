using System.ComponentModel.DataAnnotations;

namespace Library.Api.Dtos;

public class SachRequest
{
    [Required]
    [StringLength(200)]
    public string TenSach { get; set; } = string.Empty;

    [StringLength(100)]
    public string? TacGia { get; set; }

    [StringLength(100)]
    public string? NhaXuatBans { get; set; }

    [Range(1, 9999)]
    public int? NamXuatBan { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999.99",
    ParseLimitsInInvariantCulture = true)]
    public decimal? GiaBia { get; set; }

    [Range(1, int.MaxValue)]
    public int SoLuongTong { get; set; }

    [Range(1, int.MaxValue)]
    public int? MaTheLoai { get; set; }
}