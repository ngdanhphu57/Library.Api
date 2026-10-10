using Library.Api.Data;
using Library.Api.Dtos;
using Library.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/docgia")]
public class DocGiaController : ControllerBase
{
    private readonly LibraryDbContext _db;

    public DocGiaController(LibraryDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _db.Set<Docgium>()
            .AsNoTracking()
            .OrderBy(d => d.MaDocGia)
            .Select(d => new
            {
                d.MaDocGia,
                d.HoTen,
                d.NgaySinh,
                d.Lop,
                d.DienThoai,
                d.Email,
                d.NgayLapThe,
                d.NgayHetHan
            })
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _db.Set<Docgium>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.MaDocGia == id);

        if (entity == null)
        {
            return NotFound(new { message = "Không tìm thấy độc giả." });
        }

        return Ok(ToResponse(entity));
    }

    [HttpPost]
    public async Task<IActionResult> Create(DocGiaRequest request)
    {
        var entity = new Docgium();
        ApplyRequest(entity, request);

        _db.Set<Docgium>().Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = entity.MaDocGia },
            ToResponse(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, DocGiaRequest request)
    {
        var entity = await _db.Set<Docgium>().FindAsync(id);

        if (entity == null)
        {
            return NotFound(new { message = "Không tìm thấy độc giả." });
        }

        ApplyRequest(entity, request);
        await _db.SaveChangesAsync();

        return Ok(ToResponse(entity));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Set<Docgium>().FindAsync(id);

        if (entity == null)
        {
            return NotFound(new { message = "Không tìm thấy độc giả." });
        }

        var hasHistory = await _db.Set<Phieumuon>()
            .AnyAsync(p => p.MaDocGia == id);

        if (hasHistory)
        {
            return Problem(
                statusCode: 409,
                title: "Không thể xóa độc giả đã có phiếu mượn.");
        }

        _db.Set<Docgium>().Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static void ApplyRequest(
        Docgium entity, DocGiaRequest request)
    {
        entity.HoTen = request.HoTen.Trim();
        entity.NgaySinh = request.NgaySinh?.Date;
        entity.Lop = request.Lop?.Trim();
        entity.DienThoai = request.DienThoai?.Trim();
        entity.Email = request.Email?.Trim();
        entity.NgayLapThe = request.NgayLapThe?.Date;
        entity.NgayHetHan = request.NgayHetHan?.Date;
    }

    private static object ToResponse(Docgium entity)
    {
        return new
        {
            entity.MaDocGia,
            entity.HoTen,
            entity.NgaySinh,
            entity.Lop,
            entity.DienThoai,
            entity.Email,
            entity.NgayLapThe,
            entity.NgayHetHan
        };
    }
}