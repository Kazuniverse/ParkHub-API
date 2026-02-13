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
    public class KendaraanController(ParkHubContext _db) : ControllerBase
    {
        [HttpGet("Show")]
        public IActionResult Show()
        {
            var areas = _db.Kendaraans
                .Select(p => new
                {
                    id = p.Id,
                    platNomor = p.PlatNomor,
                    warna = p.Warna,
                    pemilik = p.Pemilik,
                    jenisKendaraan = p.JenisKendaraan.Jenis,
                    imagePath = p.ImagePath
                })
                .ToList();

            return Ok(areas);
        }

        [HttpGet("ShowAll")]
        public IActionResult GetAll(string search = "", int index = 1, string jenis = "")
        {
            var query = _db.Kendaraans.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.PlatNomor.Contains(search) ||
                    p.Pemilik.Contains(search) ||
                    p.Warna.Contains(search));
            }

            if (!string.IsNullOrEmpty(jenis))
            {
                query = query.Where(p =>
                    p.JenisKendaraan.Jenis == jenis);
            }

            var areas = query
                .OrderBy(p => p.Id)
                .Skip((index - 1) * 25)
                .Take(25)
                .Select(p => new
                {
                    id = p.Id,
                    platNomor = p.PlatNomor,
                    warna = p.Warna,
                    pemilik = p.Pemilik,
                    jenisKendaraan = p.JenisKendaraan.Jenis,
                    imagePath = p.ImagePath
                })
                .ToList();

            return Ok(new
            {
                data = areas,
                page = index
            });
        }


        [HttpGet("ShowByID/{id}")]
        public IActionResult get(int id)
        {
            var area = _db.Kendaraans.Select(p => new
            {
                id = p.Id,
                platNomor = p.PlatNomor,
                warna = p.Warna,
                pemilik = p.Pemilik,
                jenisKendaraan = p.JenisKendaraan.Jenis,
                imagePath = p.ImagePath
            }).FirstOrDefault(p => p.id == id);

            return Ok(area);
        }

        [Authorize(Roles = "1")]
        [HttpPost("AddKendaraan")]
        public IActionResult addKendaraan(KendaraanDTO dto)
        {
            var jk = _db.JenisKendaraans.FirstOrDefault(p => p.Jenis == dto.JenisKendaraan);
            if (jk == null) return NotFound("Jenis Kendaraan Tidak Ditemukan!");

            var kendaraan = new Kendaraan
            {
                PlatNomor = dto.PlatNomor.ToUpper(),
                Warna = dto.Warna,
                Pemilik = dto.Pemilik,
                JenisKendaraanId = jk.Id,
                ImagePath = dto.ImagePath
            };

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Menambah Kendaraan",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.Kendaraans.Add(kendaraan);
            _db.SaveChanges();

            return Ok(new
            {
                kendaraan.PlatNomor,
                kendaraan.Pemilik,
                kendaraan.Warna,
                kendaraan.JenisKendaraan.Jenis
            });
        }

        [Authorize(Roles = "1")]
        [HttpPut("UpdateKendaraan/{id}")]
        public IActionResult updateKendaraan(int id, UpdateKendaraanDTO dto)
        {
            var kendaraan = _db.Kendaraans.FirstOrDefault(p => p.Id == id);
            if (kendaraan == null) return NotFound("Kendaraan Tidak Ditemukan!");

            var jk = _db.JenisKendaraans.FirstOrDefault(p => p.Jenis == dto.JenisKendaraan);
            if (jk == null) return NotFound("Jenis Kendaraan Tidak Ditemukan!");

            kendaraan.Warna = dto.Warna;
            kendaraan.Pemilik = dto.Pemilik;
            kendaraan.JenisKendaraanId = jk.Id;
            if (!string.IsNullOrEmpty(dto.ImagePath))
            {
                kendaraan.ImagePath = dto.ImagePath;
            }

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Memperbarui Kendaraan",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.SaveChanges();

            return NoContent();
        }

        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public IActionResult deleteKendaraan(int id)
        {
            var kendaraan = _db.Kendaraans.FirstOrDefault(p => p.Id == id);
            if (kendaraan == null) return NotFound("Kendaraan Tidak Ditemukan!");

            var cannot = _db.Transaksis.FirstOrDefault(p => p.KendaraanId == id && p.Status == "Masuk");
            if (cannot != null) return UnprocessableEntity("Terdapat Kendaraan Yang Masih Terparkir");

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Menghapus Kendaraan",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.Kendaraans.Remove(kendaraan);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
