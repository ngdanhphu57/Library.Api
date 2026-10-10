using System.ComponentModel.DataAnnotations;

namespace Library.Api.Dtos;

public class DocGiaRequest : IValidatableObject
{
    [Required]
    [StringLength(100)]
    public string HoTen { get; set; } = string.Empty;

    public DateTime? NgaySinh { get; set; }

    [StringLength(50)]
    public string? Lop { get; set; }

    [StringLength(15)]
    public string? DienThoai { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required]
    public DateTime? NgayLapThe { get; set; }

    public DateTime? NgayHetHan { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (NgaySinh.HasValue &&
            NgaySinh.Value.Date > DateTime.Today)
        {
            yield return new ValidationResult(
                "Ngày sinh không được ở tương lai.",
                new[] { nameof(NgaySinh) });
        }

        if (NgayLapThe.HasValue && NgayHetHan.HasValue &&
            NgayHetHan.Value.Date < NgayLapThe.Value.Date)
        {
            yield return new ValidationResult(
                "Ngày hết hạn không được trước ngày lập thẻ.",
                new[] { nameof(NgayHetHan) });
        }
    }
}
