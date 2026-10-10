using Library.Api.Data;
using Library.Api.Dtos;
using Library.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/nhanvien")]
public class NhanVienController : ControllerBase
{
    private readonly LibraryDbContext _db;
    private readonly PasswordHasher<Nhanvien> _hasher = new();

    public NhanVienController(LibraryDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _db.Set<Nhanvien>()
            .AsNoTracking()
            .OrderBy(n => n.MaNv)
            .Select(n => new
            {
                n.MaNv,
                n.HoTen,
                n.DienThoai,
                n.Email,
                n.ChucVu
            })
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _db.Set<Nhanvien>()
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.MaNv == id);

        if (entity == null)
        {
            return NotFound(new { message = "Không tìm thấy nhân viên." });
        }

        return Ok(ToResponse(entity));
    }

    [HttpPost]
    public async Task<IActionResult> Create(NhanVienRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MatKhau))
        {
            return BadRequest(new
            {
                message = "Mật khẩu bắt buộc khi thêm nhân viên."
            });
        }

        var entity = new Nhanvien();
        ApplyRequest(entity, request);

        entity.MatKhau = _hasher.HashPassword(
            entity, request.MatKhau);

        _db.Set<Nhanvien>().Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = entity.MaNv },
            ToResponse(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, NhanVienRequest request)
    {
        var entity = await _db.Set<Nhanvien>().FindAsync(id);

        if (entity == null)
        {
            return NotFound(new { message = "Không tìm thấy nhân viên." });
        }

        if (request.MatKhau != null &&
            string.IsNullOrWhiteSpace(request.MatKhau))
        {
            return BadRequest(new
            {
                message = "Mật khẩu mới không được chỉ chứa khoảng trắng."
            });
        }

        ApplyRequest(entity, request);

        if (request.MatKhau != null)
        {
            entity.MatKhau = _hasher.HashPassword(
                entity, request.MatKhau);
        }

        await _db.SaveChangesAsync();

        return Ok(ToResponse(entity));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Set<Nhanvien>().FindAsync(id);

        if (entity == null)
        {
            return NotFound(new { message = "Không tìm thấy nhân viên." });
        }

        var hasHistory = await _db.Set<Phieumuon>()
            .AnyAsync(p => p.MaNv == id);

        if (hasHistory)
        {
            return Problem(
                statusCode: 409,
                title: "Không thể xóa nhân viên đã lập phiếu mượn.");
        }

        _db.Set<Nhanvien>().Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static void ApplyRequest(
        Nhanvien entity, NhanVienRequest request)
    {
        entity.HoTen = request.HoTen.Trim();
        entity.DienThoai = request.DienThoai?.Trim();
        entity.Email = request.Email?.Trim();
        entity.ChucVu = request.ChucVu?.Trim();
    }

    private static object ToResponse(Nhanvien entity)
    {
        return new
        {
            entity.MaNv,
            entity.HoTen,
            entity.DienThoai,
            entity.Email,
            entity.ChucVu
        };
    }
}