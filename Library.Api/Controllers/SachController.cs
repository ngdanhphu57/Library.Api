using Library.Api.Data;
using Library.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Api.Dtos;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/sach")]
public class SachController : ControllerBase
{
    private readonly LibraryDbContext _db;

    public SachController(LibraryDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _db.Set<Sach>()
            .AsNoTracking()
            .OrderBy(s => s.MaSach)
            .Select(s => new
            {
                s.MaSach,
                s.TenSach,
                s.TacGia,
                s.MaTheLoai,
                s.SoLuongTong,
                s.SoLuongConLai
            })
            .ToListAsync();

        return Ok(data);
    }
    [HttpPost]
    public async Task<IActionResult> Create(SachRequest request)
    {
        if (request.MaTheLoai.HasValue)
        {
            var exists = await _db.Set<Theloai>()
                .AnyAsync(t => t.MaTheLoai == request.MaTheLoai.Value);

            if (!exists)
            {
                return BadRequest(new
                {
                    message = "Thể loại không tồn tại."
                });
            }
        }

        var entity = new Sach
        {
            TenSach = request.TenSach.Trim(),
            TacGia = request.TacGia?.Trim(),
            NhaXuatBans = request.NhaXuatBans?.Trim(),
            NamXuatBan = request.NamXuatBan,
            GiaBia = request.GiaBia,
            SoLuongTong = request.SoLuongTong,
            SoLuongConLai = request.SoLuongTong,
            MaTheLoai = request.MaTheLoai
        };

        _db.Set<Sach>().Add(entity);
        await _db.SaveChangesAsync();

        return StatusCode(201, new
        {
            entity.MaSach,
            entity.TenSach,
            entity.SoLuongTong,
            entity.SoLuongConLai,
            entity.MaTheLoai
        });
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SachRequest request)
    {
        var entity = await _db.Set<Sach>().FindAsync(id);

        if (entity == null)
        {
            return NotFound(new { message = "Không tìm thấy sách." });
        }

        if (request.MaTheLoai.HasValue)
        {
            var exists = await _db.Set<Theloai>()
                .AnyAsync(t => t.MaTheLoai == request.MaTheLoai.Value);

            if (!exists)
            {
                return BadRequest(new { message = "Thể loại không tồn tại." });
            }
        }

        var borrowed = entity.SoLuongTong - entity.SoLuongConLai;

        if (request.SoLuongTong < borrowed)
        {
            return BadRequest(new
            {
                message = "Tổng số lượng không được nhỏ hơn số sách đang cho mượn."
            });
        }

        entity.TenSach = request.TenSach.Trim();
        entity.TacGia = request.TacGia?.Trim();
        entity.NhaXuatBans = request.NhaXuatBans?.Trim();
        entity.NamXuatBan = request.NamXuatBan;
        entity.GiaBia = request.GiaBia;
        entity.MaTheLoai = request.MaTheLoai;
        entity.SoLuongTong = request.SoLuongTong;
        entity.SoLuongConLai = request.SoLuongTong - borrowed;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            entity.MaSach,
            entity.TenSach,
            entity.SoLuongTong,
            entity.SoLuongConLai
        });
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Set<Sach>().FindAsync(id);

        if (entity == null)
        {
            return NotFound(new { message = "Không tìm thấy sách." });
        }

        var hasHistory = await _db.Set<Chitietphieumuon>()
            .AnyAsync(ct => ct.MaSach == id);

        if (hasHistory)
        {
            return Problem(
                statusCode: 409,
                title: "Không thể xóa sách đã có lịch sử mượn.");
        }

        _db.Set<Sach>().Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
