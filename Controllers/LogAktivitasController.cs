using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkHub_API.Models;

namespace ParkHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogAktivitasController(ParkHubContext _db) : ControllerBase
    {
        [Authorize(Roles = "1")]
        [HttpGet("ShowAll")]
        public IActionResult GetAll(DateTime? date, int index = 1)
        {
            var query = _db.LogAktivitas.AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(p => p.WaktuAktivitas.Date == date.Value.Date);
            }

            var history = query
                .OrderByDescending(p => p.WaktuAktivitas)
                .Skip((index - 1) * 25)
                .Take(25)
                .Select(p => new
                {
                    id = p.Id,
                    aktivitas = p.Aktivitas,
                    waktuAktivitas = p.WaktuAktivitas,
                    userId = p.UserId
                })
                .ToList();

            return Ok(new
            {
                data = history,
                page = index
            });
        }



        [Authorize(Roles = "1")]
        [HttpGet("ShowById/{id}")]
        public IActionResult get(int id)
        {
            var history = _db.LogAktivitas.Select(p => new
            {
                id = p.Id,
                aktivitas = p.Aktivitas,
                waktuAktivitas = p.WaktuAktivitas,
                userId = p.UserId,
            }).FirstOrDefault(p => p.id == id);

            return Ok(history);
        }
    }
}
