using System.ComponentModel.DataAnnotations;

namespace Library.Api.Dtos;

public class PhieuMuonRequest
{
    [Range(1, int.MaxValue)]
    public int MaDocGia { get; set; }

    [Range(1, int.MaxValue)]
    public int MaNv { get; set; }

    [Required]
    public DateTime? NgayHenTra { get; set; }

    [Required]
    [MinLength(1)]
    public List<int> MaSach { get; set; } = new();
}