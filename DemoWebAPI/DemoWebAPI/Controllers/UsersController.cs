using DemoWebAPI.Data;
using DemoWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly AppSettings _appsettings;
        public UsersController(MyDbContext context, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _context = context;
            _appsettings = optionsMonitor.CurrentValue;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == model.UserName);
            if (user == null)
            {
                var user1 = new User
                {
                    UserName = model.UserName,
                    Password = model.Password,
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
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == model.UserName &&
            u.Password == model.Password);
            if (user == null)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Invalid user name/password"
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
