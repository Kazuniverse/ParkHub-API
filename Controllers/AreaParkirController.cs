using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkHub_API.DTOs;
using ParkHub_API.Models;
using System.Security.Claims;
using System.Transactions;

namespace ParkHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaParkirController(ParkHubContext _db) : ControllerBase
    {
        [HttpGet("ShowAll")]
        public IActionResult getAll()
        {
            var areas = _db.AreaParkirs
                .Select(p => new
                {
                    id = p.Id,
                    namaArea = p.NamaArea,
                    kapasitas = p.Kapasitas,
                    terisi = p.Transaksis.Count(p => p.WaktuKeluar == null)
                })
                .ToList();

            return Ok(areas);
        }

        [HttpGet("ShowByID/{id}")]
        public IActionResult get(int id)
        {
            var area = _db.AreaParkirs.Select(p => new
            {
                id = p.Id,
                namaArea = p.NamaArea,
                kapasitas = p.Kapasitas,
                terisi = p.Transaksis.Count(p => p.WaktuKeluar == null)
            }).FirstOrDefault(p => p.id == id);

            return Ok(area);
        }

        [Authorize(Roles = "1")]
        [HttpPost("AddArea")]
        public IActionResult addArea(AreaDTO dto)
        {
            var exist = _db.AreaParkirs.FirstOrDefault(p => p.NamaArea == dto.NamaArea);
            if (exist != null) return UnprocessableEntity("Nama Area Telah Digunakan");

            int terisi = 0;

            var area = new AreaParkir
            {
                NamaArea = dto.NamaArea.ToUpper(),
                Kapasitas = dto.Kapasitas,
            };

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Menambah Area Parkir",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.AreaParkirs.Add(area);
            _db.SaveChanges();
            return Ok(new
            {
                message = "Area Parkir Baru Berhasil Ditambahkan!",
                data = new
                {
                    area
                }
            });
        }

        [Authorize(Roles = "1")]
        [HttpPut("UpdateArea/{id}")]
        public IActionResult updateArea(int id, AreaDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.NamaArea))
                return BadRequest("Nama Area tidak boleh kosong");

            if (dto.Kapasitas <= 0)
                return BadRequest("Kapasitas harus lebih dari 0");

            var area = _db.AreaParkirs.FirstOrDefault(p => p.Id == id);
            if (area == null)
                return NotFound("Area Parkir Tidak Ditemukan!");

            var exist = _db.AreaParkirs
                .FirstOrDefault(p => p.Id != id
                                  && p.NamaArea.ToUpper() == dto.NamaArea.ToUpper());

            if (exist != null)
                return UnprocessableEntity("Nama Area Telah Digunakan");

            area.NamaArea = dto.NamaArea.ToUpper();
            area.Kapasitas = dto.Kapasitas;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out int uid);

            _db.LogAktivitas.Add(new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Memperbarui Area Parkir",
                WaktuAktivitas = DateTime.Now
            });

            _db.SaveChanges();

            return NoContent();
        }


        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public IActionResult deleteArea(int id)
        {
            var area = _db.AreaParkirs.FirstOrDefault(p => p.Id == id);
            if (area == null) return NotFound("Area Parkir Tidak Ditemukan!");

            var cannot = _db.Transaksis.FirstOrDefault(p => p.AreaId == id && p.Status == "Masuk");
            if (cannot != null) return UnprocessableEntity("Terdapat Kendaraan Yang Masih Terparkir");

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Menghapus Area Parkir",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.AreaParkirs.Remove(area);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
