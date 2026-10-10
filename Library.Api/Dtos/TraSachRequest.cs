using System.ComponentModel.DataAnnotations;

namespace Library.Api.Dtos;

public class TraSachRequest
{
    [Required]
    [StringLength(100)]
    public string TinhTrangKhiTra { get; set; } = "Bình thường";
}