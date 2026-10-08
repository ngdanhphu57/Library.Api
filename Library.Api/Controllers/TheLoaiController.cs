using Library.Api.Data;
using Library.Api.Entities;
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
}