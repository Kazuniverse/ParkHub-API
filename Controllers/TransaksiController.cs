using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using ParkHub_API.DTOs;
using ParkHub_API.Models;
using System.Security.Claims;

namespace ParkHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaksiController(ParkHubContext _db) : ControllerBase
    {
        [HttpGet("Show")]
        public IActionResult Show()
        {
            var transaksi = _db.Transaksis
                .OrderByDescending(p => p.WaktuMasuk)
                .Select(p => new
                {
                    id = p.Id,
                    userId = p.UserId,
                    waktuMasuk = p.WaktuMasuk,
                    waktuKeluar = p.WaktuKeluar,
                    durasi = p.WaktuKeluar == null
                        ? (int)(DateTime.Now - p.WaktuMasuk).TotalHours
                        : (int)(p.WaktuKeluar.Value - p.WaktuMasuk).TotalHours,
                    biayaTotal = p.WaktuKeluar == null
                        ? 0
                        : p.BiayaTotal,
                    status = p.Status,
                    tarif = p.Tarif == null ? 0 : p.Tarif.TarifPerJam,
                    platNomor = p.Kendaraan == null ? "----" : p.Kendaraan.PlatNomor,
                    areaId = p.Area == null ? "-" : p.Area.NamaArea
                })
                .ToList();

            var total = transaksi.Sum(p => p.biayaTotal);

            return Ok(new
            {
                data = transaksi,
                totalPendapatan = total
            });
        }

        [Authorize(Roles = "2,3")]
        [HttpGet("ShowAll")]
        public IActionResult GetAll(string search = "", int index = 1, string jenis = "", string area = "")
        {
            var query = _db.Transaksis.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.Kendaraan.PlatNomor.Contains(search) ||
                    p.Kendaraan.Warna.Contains(search));
            }

            if (!string.IsNullOrEmpty(area))
            {
                query = query.Where(p =>
                    p.Area.NamaArea == area);
            }

            if (!string.IsNullOrEmpty(jenis))
            {
                query = query.Where(p =>
                    p.Kendaraan.JenisKendaraan.Jenis == jenis);
            }

            var transaksi = query
                .OrderByDescending(p => p.WaktuMasuk)
                .Skip((index - 1) * 25)
                .Take(25)
                .Select(p => new
                {
                    id = p.Id,
                    userId = p.UserId,
                    waktuMasuk = p.WaktuMasuk,
                    waktuKeluar = p.WaktuKeluar,
                    durasi = p.WaktuKeluar == null
                        ? (int)(DateTime.Now - p.WaktuMasuk).TotalHours
                        : (int)(p.WaktuKeluar.Value - p.WaktuMasuk).TotalHours,
                    biayaTotal = p.WaktuKeluar == null
                        ? 0
                        : p.BiayaTotal,
                    status = p.Status,
                    tarif = p.Tarif == null ? 0 : p.Tarif.TarifPerJam,
                    platNomor = p.Kendaraan == null ? "----" : p.Kendaraan.PlatNomor,
                    areaId = p.Area == null ? "-" : p.Area.NamaArea
                })
                .ToList();

            return Ok(new
            {
                data = transaksi,
                page = index
            });
        }


        [Authorize(Roles = "2,3")]
        [HttpGet("ShowByID/{id}")]
        public IActionResult get(int id)
        {
            var transaksi = _db.Transaksis
                .Select(p => new
                {
                    id = p.Id,
                    userId = p.UserId,
                    waktuMasuk = p.WaktuMasuk,
                    waktuKeluar = p.WaktuKeluar,
                    durasi = p.WaktuKeluar == null
                        ? (int)(DateTime.Now - p.WaktuMasuk).TotalHours
                        : (int)(p.WaktuKeluar.Value - p.WaktuMasuk).TotalHours,
                    biayaTotal = p.WaktuKeluar == null
                        ? 0
                        : p.BiayaTotal,
                    status = p.Status,
                    tarif = p.Tarif == null ? 0 : p.Tarif.TarifPerJam,
                    platNomor = p.Kendaraan == null ? "----" : p.Kendaraan.PlatNomor,
                    areaId = p.Area == null ? "-" : p.Area.NamaArea
                })
                .FirstOrDefault(p => p.id == id);
            if (transaksi == null) return NotFound("Data Transaksi Tidak Dapat Ditemukan!");


            return Ok(transaksi);
        }

        [Authorize(Roles = "2")]
        [HttpPost("AddTransaksi")]
        public IActionResult addTransaksi(TransaksiDTO dto)
        {
            var kendaraan = _db.Kendaraans.FirstOrDefault(p => p.PlatNomor == dto.PlatNomor);
            if (kendaraan == null) return BadRequest();

            var tarif = _db.Tarifs.FirstOrDefault(p => p.JenisKendaraanId == kendaraan.JenisKendaraanId);
            if (tarif == null) return NotFound("Jenis Kendaraan Tidak Ditemukan!");

            var area = _db.AreaParkirs.FirstOrDefault(p => p.NamaArea == dto.NamaArea);
            if (area == null) return NotFound("Area Parkir Tidak Ditemukan!");

            var terisi = _db.Transaksis.Count(p => p.WaktuKeluar == null && p.AreaId == area.Id);
            if (terisi >= area.Kapasitas) return UnprocessableEntity("Area Parkir Penuh");

            var aktif = _db.Transaksis.FirstOrDefault(p => p.Kendaraan.PlatNomor == dto.PlatNomor && p.WaktuKeluar == null);
            if (aktif != null) return UnprocessableEntity("Kendaraan Masih Terparkir!");

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var transaksi = new Transaksi
            {
                WaktuMasuk = DateTime.Now,
                BiayaTotal = 0,
                Status = "Masuk",
                Durasi = 0,
                KendaraanId = kendaraan.Id,
                TarifId = tarif.Id,
                UserId = uid,
                AreaId = area.Id
            };

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Menambah Transaksi",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.Transaksis.Add(transaksi);
            _db.SaveChanges();

            return Ok(new
            {
                message = "Transaksi Berhasil Ditambahkan!",
                data = new
                {
                    id = transaksi.Id,
                    userId = transaksi.UserId,
                    waktuMasuk = transaksi.WaktuMasuk,
                    waktuKeluar = transaksi.WaktuKeluar,
                    durasi = transaksi.Durasi,
                    biayaTotal = transaksi.BiayaTotal,
                    status = transaksi.Status,
                    tarif = transaksi.Tarif.TarifPerJam,
                    platNomor = transaksi.Kendaraan.PlatNomor,
                    areaId = area.NamaArea
                }
            });
        }

        [Authorize(Roles = "2")]
        [HttpPut("UpdateTransaksi/{id}")]
        public IActionResult updateTransaksi(int id)
        {
            var transaksi = _db.Transaksis.FirstOrDefault(p => p.Id == id);
            if (transaksi == null) return NotFound("Data Transaksi Tidak Dapat Ditemukan");

            var tarif = _db.Tarifs.FirstOrDefault(p => p.Id == transaksi.TarifId);
            if (tarif == null) return NotFound("Tarif Tidak Diketahui!");

            transaksi.WaktuKeluar = DateTime.Now;
            transaksi.Status = "Keluar";
            transaksi.Durasi = (int)(transaksi.WaktuKeluar.Value - transaksi.WaktuMasuk).TotalHours;
            transaksi.BiayaTotal = (decimal)transaksi.Durasi * (decimal)tarif.TarifPerJam;

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Memperbarui Transaksi",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.SaveChanges();

            var area = _db.AreaParkirs.FirstOrDefault(p => p.Id == transaksi.AreaId);

            return NoContent();
        }
    }
}
