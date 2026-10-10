using Library.Api.Data;
using Library.Api.Dtos;
using Library.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/phieumuon")]
public class PhieuMuonController : ControllerBase
{
    private readonly LibraryDbContext _db;

    public PhieuMuonController(LibraryDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create(PhieuMuonRequest request)
    {
        var now = DateTime.Now;
        var dueDate = request.NgayHenTra!.Value.Date;

        if (dueDate <= now.Date)
        {
            return BadRequest(new
            {
                message = "Ngày hẹn trả phải sau ngày hôm nay."
            });
        }

        if (request.MaSach.Any(id => id <= 0) ||
            request.MaSach.Distinct().Count() != request.MaSach.Count)
        {
            return BadRequest(new
            {
                message = "Mã sách phải lớn hơn 0 và không được trùng."
            });
        }

        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        var reader = await _db.Set<Docgium>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.MaDocGia == request.MaDocGia);

        if (reader == null)
        {
            return NotFound(new { message = "Không tìm thấy độc giả." });
        }

        if (!reader.NgayHetHan.HasValue ||
            reader.NgayHetHan.Value.Date < now.Date)
        {
            return BadRequest(new
            {
                message = "Thẻ độc giả chưa có hạn sử dụng hoặc đã hết hạn."
            });
        }

        if (reader.NgayLapThe.HasValue &&
            reader.NgayLapThe.Value.Date > now.Date)
        {
            return BadRequest(new
            {
                message = "Thẻ độc giả chưa đến ngày sử dụng."
            });
        }

        var staffExists = await _db.Set<Nhanvien>()
            .AnyAsync(n => n.MaNv == request.MaNv);

        if (!staffExists)
        {
            return NotFound(new { message = "Không tìm thấy nhân viên." });
        }

        foreach (var bookId in request.MaSach.OrderBy(id => id))
        {
            var affected = await _db.Set<Sach>()
                .Where(s => s.MaSach == bookId && s.SoLuongConLai > 0)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        s => s.SoLuongConLai,
                        s => s.SoLuongConLai - 1));

            if (affected == 0)
            {
                return Problem(
                    statusCode: 409,
                    title: $"Sách mã {bookId} không tồn tại hoặc đã hết.");
            }
        }

        var entity = new Phieumuon
        {
            MaDocGia = request.MaDocGia,
            MaNv = request.MaNv,
            NgayMuon = now,
            NgayHenTra = dueDate,
            TrangThai = "Đang mượn"
        };

        foreach (var bookId in request.MaSach)
        {
            entity.Chitietphieumuons.Add(new Chitietphieumuon
            {
                MaSach = bookId,
                SoLuong = 1,
                TinhTrangKhiMuon = "Bình thường",
                TienPhat = 0
            });
        }

        _db.Set<Phieumuon>().Add(entity);
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return StatusCode(201, new
        {
            entity.MaPhieu,
            entity.MaDocGia,
            entity.MaNv,
            entity.NgayMuon,
            entity.NgayHenTra,
            entity.TrangThai,
            MaSach = request.MaSach
        });
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _db.Set<Phieumuon>()
            .AsNoTracking()
            .Include(p => p.Chitietphieumuons)
                .ThenInclude(ct => ct.MaSachNavigation)
            .FirstOrDefaultAsync(p => p.MaPhieu == id);

        if (entity == null)
        {
            return NotFound(new { message = "Không tìm thấy phiếu mượn." });
        }

        return Ok(new
        {
            entity.MaPhieu,
            entity.MaDocGia,
            entity.MaNv,
            entity.NgayMuon,
            entity.NgayHenTra,
            entity.TrangThai,
            Sach = entity.Chitietphieumuons
                .OrderBy(ct => ct.MaSach)
                .Select(ct => new
                {
                    ct.MaSach,
                    TenSach = ct.MaSachNavigation.TenSach,
                    ct.SoLuong,
                    ct.NgayTraThucTe,
                    ct.TinhTrangKhiMuon,
                    ct.TinhTrangKhiTra,
                    ct.TienPhat
                })
                .ToList()
        });
    }
    [HttpPost("{id:int}/tra/{maSach:int}")]
    public async Task<IActionResult> ReturnBook(
    int id, int maSach, TraSachRequest request)
    {
        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        var receipts = await _db.Set<Phieumuon>()
            .FromSqlInterpolated(
                $"SELECT * FROM phieumuon WHERE MaPhieu = {id} FOR UPDATE")
            .ToListAsync();

        var receipt = receipts.SingleOrDefault();

        if (receipt == null)
        {
            return NotFound(new { message = "Không tìm thấy phiếu mượn." });
        }

        var details = await _db.Set<Chitietphieumuon>()
            .Where(ct => ct.MaPhieu == id)
            .ToListAsync();

        var detail = details.SingleOrDefault(ct => ct.MaSach == maSach);

        if (detail == null)
        {
            return NotFound(new
            {
                message = "Sách không nằm trong phiếu mượn này."
            });
        }

        if (detail.NgayTraThucTe.HasValue)
        {
            return Problem(
                statusCode: 409,
                title: "Sách này đã được trả.");
        }

        var quantity = detail.SoLuong.GetValueOrDefault();

        if (quantity <= 0)
        {
            return Problem(
                statusCode: 409,
                title: "Số lượng trong chi tiết phiếu không hợp lệ.");
        }

        var affected = await _db.Set<Sach>()
            .Where(s => s.MaSach == maSach &&
                        s.SoLuongConLai <= s.SoLuongTong - quantity)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    s => s.SoLuongConLai,
                    s => s.SoLuongConLai + quantity));

        if (affected == 0)
        {
            return Problem(
                statusCode: 409,
                title: "Không thể cộng số lượng: dữ liệu tồn kho không khớp.");
        }

        detail.NgayTraThucTe = DateTime.Now;
        detail.TinhTrangKhiTra = request.TinhTrangKhiTra.Trim();

        receipt.TrangThai = details.All(ct => ct.NgayTraThucTe.HasValue)
            ? "Đã trả"
            : "Đang mượn";

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(new
        {
            receipt.MaPhieu,
            detail.MaSach,
            detail.NgayTraThucTe,
            detail.TinhTrangKhiTra,
            receipt.TrangThai
        });
    }
}