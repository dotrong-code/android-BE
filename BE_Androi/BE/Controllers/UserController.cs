using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repositories.Models;
using Services;

namespace BE.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _UsersService;

        public UserController(IConfiguration config, IUserService UsersService)
        {
            _config = config;
            _UsersService = UsersService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _UsersService.Authenticate(request.Username, request.PasswordHash);

            if (user == null)
                return Unauthorized();

            var token = GenerateJSONWebToken(user);
            return Ok(new { token = token });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Kiểm tra xem username đã tồn tại chưa
            var existingUser = await _UsersService.GetUserByUsername(request.Username);
            if (existingUser != null)
                return BadRequest("Username already exists.");

            // Hash mật khẩu trước khi lưu
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = hashedPassword,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                Role = "User" // Mặc định role là "User", có thể thay đổi theo yêu cầu
            };

            await _UsersService.Register(newUser);
            return Ok(new { message = "User registered successfully." });
        }
        [Authorize]
        [HttpGet("GetUser")]
        public IActionResult GetUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated)
                return Unauthorized("Invalid token or user not authenticated.");

            var claims = identity.Claims;
            var username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return Ok(new { Username = username, Role = role });
        }

        private string GenerateJSONWebToken(User systemUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                new Claim[]
                {
                new(ClaimTypes.Name, systemUser.Username),
                new(ClaimTypes.Role, systemUser.Role.ToString()),
                },
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize]
        [HttpGet("GetUser")]
        public IActionResult GetUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated)
                return Unauthorized("Invalid token or user not authenticated.");

            var claims = identity.Claims;
            var username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return Ok(new { Username = username, Role = role });
        }

        public sealed record LoginRequest(string Username, string PasswordHash);
        public sealed record RegisterRequest(string Username, string Password, string? Email, string? PhoneNumber, string? Address);
    }

}
