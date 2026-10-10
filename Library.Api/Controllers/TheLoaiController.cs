using Library.Api.Data;
using Library.Api.Entities;
using Library.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/theloai")]
public class TheLoaiController : ControllerBase
{
    private readonly LibraryDbContext _db;

    public TheLoaiController(LibraryDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _db.Set<Theloai>()
            .AsNoTracking()
            .OrderBy(t => t.MaTheLoai)
            .Select(t => new { t.MaTheLoai, t.TenTheLoai })
            .ToListAsync();

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TheLoaiRequest request)
    {
        var name = request.TenTheLoai.Trim();

        if (await _db.Set<Theloai>()
            .AnyAsync(t => t.TenTheLoai == name))
        {
            return Problem(
                statusCode: 409,
                title: "Tên thể loại đã tồn tại.");
        }

        var entity = new Theloai
        {
            TenTheLoai = name
        };

        _db.Set<Theloai>().Add(entity);
        await _db.SaveChangesAsync();

        return StatusCode(201, new
        {
            entity.MaTheLoai,
            entity.TenTheLoai
        });
    }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, TheLoaiRequest request)
        {
            var entity = await _db.Set<Theloai>().FindAsync(id);

            if (entity == null)
            {
                return NotFound(new { message = "Không tìm thấy thể loại." });
            }

            var name = request.TenTheLoai.Trim();

            var exists = await _db.Set<Theloai>()
                .AnyAsync(t => t.TenTheLoai == name && t.MaTheLoai != id);

            if (exists)
            {
                return Problem(
                    statusCode: 409,
                    title: "Tên thể loại đã tồn tại.");
            }

            entity.TenTheLoai = name;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                entity.MaTheLoai,
                entity.TenTheLoai
            });
        
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Set<Theloai>().FindAsync(id);

        if (entity == null)
        {
            return NotFound(new { message = "Không tìm thấy thể loại." });
        }

        var hasBooks = await _db.Set<Sach>()
            .AnyAsync(s => s.MaTheLoai == id);

        if (hasBooks)
        {
            return Problem(
                statusCode: 409,
                title: "Không thể xóa thể loại đang có sách.");
        }

        _db.Set<Theloai>().Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}