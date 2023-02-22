using DemoWebAPI.Data;
using DemoWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DemoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public static User user0 = new User();
        private readonly MyDbContext _context;
        private readonly AppSettings _appsettings;
        public UsersController(MyDbContext context, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _context = context;
            _appsettings = optionsMonitor.CurrentValue;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(RegisterModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == model.UserName);
            if (user == null)
            {
                CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var user1 = new User
                {
                    UserName = model.UserName,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    FullName = model.FullName,
                    Email = model.Email
                };
                _context.Users.Add(user1);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    Success = true,
                    Data = user1
                });
            }
            return BadRequest("Username already exists");
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == model.UserName
            );
            if (user == null)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Invalid user name"
                });
            }
            if(!VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Invalid password"
                });
            }
            var token = await GenerateToken(user);
            return Ok(new
            {
                Success = true,
                Message = "Authenticate Success",
                Data = token
            });
        }
        private async Task<String> GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var securityKeyBytes = Encoding.UTF8.GetBytes(_appsettings.SecretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim("UserName", user.UserName),
                    new Claim("Id", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKeyBytes),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            return accessToken;
        }
    }
}
