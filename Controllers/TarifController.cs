using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParkHub_API.DTOs;
using ParkHub_API.Models;
using System.Security.Claims;

namespace ParkHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarifController(ParkHubContext _db) : ControllerBase
    {
        [HttpGet("ShowAll")]
        public IActionResult getAll()
        {
            var tarifs = _db.Tarifs.Select(p => new
            {
                id = p.Id,
                jenisKendaraan = p.JenisKendaraan.Jenis,
                tarifPerJam = p.TarifPerJam
            }).ToList();

            return Ok(tarifs);
        }

        [HttpGet("ShowByID/{id}")]
        public IActionResult get(int id)
        {
            var tarif = _db.Tarifs.Select(p => new
            {
                id = p.Id,
                jenisKendaraan = p.JenisKendaraan.Jenis,
                tarifPerJam = p.TarifPerJam
            }).FirstOrDefault(p => p.id == id);

            return Ok(tarif);
        }

        [Authorize(Roles = "1")]
        [HttpPost("AddTarif")]
        public IActionResult addTarif(TarifDTO dto)
        {
            var exist = _db.Tarifs.FirstOrDefault(p => p.JenisKendaraan.Jenis == dto.JenisKendaraan);
            if (exist != null) return UnprocessableEntity("Jenis Kendaraan Ini Sudah Digunakan Pada Tarif Lain");

            var jk = new JenisKendaraan
            {
                Jenis = dto.JenisKendaraan
            };

            _db.JenisKendaraans.Add(jk);
            _db.SaveChanges();

            var tarif = new Tarif
            {
                TarifPerJam = dto.TarifPerJam,
                JenisKendaraanId = jk.Id
            };

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Menambah Tarif",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.Tarifs.Add(tarif);
            _db.SaveChanges();

            return Ok(new
            {
                message = "Tarif Beru Berhasil Ditambahkan",
                data = new
                {
                    tarif
                }
            });
        }

        [Authorize(Roles = "1")]    
        [HttpPut("UpdateTarif/{id}")]
        public IActionResult updateTarif(int id, TarifDTO dto)
        {
            var tarif = _db.Tarifs.FirstOrDefault(p => p.Id == id);
            if (tarif == null) return NotFound("Data Tarif Tidak Ditemukan");

            var jk = _db.JenisKendaraans.FirstOrDefault(p => p.Id == tarif.JenisKendaraanId);

            var exist = _db.Tarifs.FirstOrDefault(p => p.Id != id && p.JenisKendaraan.Jenis == dto.JenisKendaraan);
            if (exist != null) return UnprocessableEntity("Jenis Kendaraan Ini Sudah Digunakan Pada Tarif Lain");

            jk.Jenis = dto.JenisKendaraan;
            tarif.TarifPerJam = dto.TarifPerJam;

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Memperbarui Tarif",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.SaveChanges();

            return NoContent();
        }

        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public IActionResult deleteTarif(int id)
        {
            var tarif = _db.Tarifs.FirstOrDefault(p => p.Id == id);
            if (tarif == null) return NotFound("Data Tarif Tidak Ditemukan!");

            var jk = _db.JenisKendaraans.FirstOrDefault(p => p.Id == tarif.JenisKendaraanId);

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Menghapus Tarif",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.Tarifs.Remove(tarif);
            _db.JenisKendaraans.Remove(jk);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
