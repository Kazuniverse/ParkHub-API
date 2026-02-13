using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkHub_API.DTOs;
using ParkHub_API.Models;
using System.Security.Claims;

namespace ParkHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JenisKendaraanController(ParkHubContext _db) : ControllerBase
    {
        [HttpGet("ShowAll")]
        public IActionResult getAll()
        {
            var jenis = _db.JenisKendaraans.ToList();

            return Ok(jenis);
        }

        [HttpGet("ShowByID/{id}")]
        public IActionResult get(int id)
        {
            var jenis = _db.JenisKendaraans.FirstOrDefault(p => p.Id == id);

            return Ok(jenis);
        }

        [Authorize(Roles = "1")]
        [HttpPost("AddJenisKendaraan")]
        public IActionResult addJenisKendaraan(JenisKendaraanDTO dto)
        {
            var jenis = new JenisKendaraan
            {
                Jenis = dto.Jenis
            };

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Menambah Jenis Kendaraan",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.JenisKendaraans.Add(jenis);
            _db.SaveChanges();
            return Ok(new
            {
                message = "Jenis Kendaraan Baru Berhasil Ditambahkan!",
                jenis = jenis
            });
        }

        [Authorize(Roles = "1")]
        [HttpPut("UpdateJenisKendaraan/{id}")]
        public IActionResult updateAreaKendaraan(int id, JenisKendaraan dto)
        {
            var jenis = _db.JenisKendaraans.FirstOrDefault(p => p.Id == id);
            if (jenis == null) return NotFound("Jenis Kendaraan Tidak Ditemukan");

            jenis.Jenis = dto.Jenis;

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Memperbarui Jenis Kendaraan",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.SaveChanges();

            return Ok(jenis);
        }

        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public IActionResult deleteJenisKendaraan(int id)
        {
            var jenis = _db.JenisKendaraans.FirstOrDefault(p => p.Id == id);
            if (jenis == null) return NotFound("Jenis Kendaraan Tidak Ditemukan");

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Menghapus Jenis Kendaraan",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);
            _db.JenisKendaraans.Remove(jenis);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
