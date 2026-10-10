using System.ComponentModel.DataAnnotations;

namespace Library.Api.Dtos;

public class TheLoaiRequest
{
    [Required]
    [StringLength(100)]
    public string TenTheLoai { get; set; } = string.Empty;
}
