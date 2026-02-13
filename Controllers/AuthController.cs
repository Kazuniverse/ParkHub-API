using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.IdentityModel.Tokens;
using ParkHub_API.DTOs;
using ParkHub_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ParkHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(ParkHubContext _db, IConfiguration _config) : ControllerBase
    {
        [HttpPost("Login")]
        public IActionResult Login(LoginDTO dto)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(p => p.Username == dto.Username);
                if (user == null) return NotFound("User Tidak Ditemukan!");

                var aktivitas = new LogAktivita
                {
                    UserId = user.Id,
                    Aktivitas = "Telah Login",
                    WaktuAktivitas = DateTime.Now
                };

                _db.LogAktivitas.Add(aktivitas);
                _db.SaveChanges();

                var valid = user.Password == dto.Password;
                if (valid)
                {
                    return Ok(new
                    {
                        message = "Login Berhasil!",
                        data = new
                        {
                            name = user.NamaLengkap,
                            username = user.Username
                        },
                        token = generateJwt(user)
                    });
                }
                else if (!valid)
                {
                    return UnprocessableEntity(new
                    {
                        message = "Password Salah!",
                        data = new
                        {
                            name = user.NamaLengkap,
                            username = user.Username
                        }
                    });
                }
                else
                {
                    return UnprocessableEntity(new
                    {
                        message = "Something Went Wrong!"
                    });
                }
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("${id}")]
        public IActionResult ShowById(int id)
        {
            var user = _db.Users.Select(p => new
            {
                p.Id,
                p.NamaLengkap,
                p.Username
            }).FirstOrDefault(p => p.Id == id);

            return Ok(user);
        }

        [Authorize(Roles = "1")]
        [HttpPost("Register")]
        public IActionResult Register(RegisterDTO dto)
        {
            var role = _db.Roles.FirstOrDefault(p => p.RoleName == dto.Role);
            if (role == null) return NotFound();

            var user = new User
            {
                NamaLengkap = dto.NamaLengkap,
                Username = dto.Username.ToLower(),
                Password = dto.Password,
                RoleId = role.Id
            };

            if (dto.NamaLengkap.Length < 1)
            {
                return UnprocessableEntity("Nama Lengkap Tidak Boleh Kosong!");
            }
            else if (dto.Username.Length < 4)
            {
                return UnprocessableEntity("Username Length Minimum 4 Karakter"!);
            } else if (dto.Password.Length < 8)
            {
                return UnprocessableEntity("Password Length Minimum 8 Karakter!");
            }

            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Menambahkan User",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(new
            {
                message = "Registrasi Berhasil, Silahkan Login!",
                data = new
                {
                    name = user.NamaLengkap,
                    username = user.Username
                }
            });
        }

        [Authorize]
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            int uid = 0;

            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);

            var aktivitas = new LogAktivita
            {
                UserId = uid,
                Aktivitas = "Telah Logout",
                WaktuAktivitas = DateTime.Now
            };

            _db.LogAktivitas.Add(aktivitas);
            _db.SaveChanges();

            return Ok(new
            {
                message = "Logged Out"
            });
        }

        [Authorize]
        [HttpGet("Me")]
        public IActionResult Me()
        {
            int uid = 0;
            int.TryParse(User.FindFirstValue(ClaimTypes.Sid), out uid);
            var user = _db.Users.FirstOrDefault(p => p.Id == uid);

            return Ok(new
            {
                namaLengkap = user.NamaLengkap,
                username = user.Username,
                roleId = user.RoleId
            });
        }

        private string generateJwt(User user)
        {
            var key = _config["JWT:Key"];
            var ssk = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var cred = new SigningCredentials(ssk, SecurityAlgorithms.HmacSha256);

            var claim = new[]
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var token = new JwtSecurityToken(
                claims: claim,
                signingCredentials: cred,
                expires: DateTime.Now.AddMonths(1));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
