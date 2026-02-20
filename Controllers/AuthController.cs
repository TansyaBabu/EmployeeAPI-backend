namespace EmployeeAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using EmployeeAPI.Model;          // ✅ FIXED
    using EmployeeAPI.Services;       // ✅ FIXED
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EmployeeService _service;
        private readonly IConfiguration _configuration;

        public AuthController(EmployeeService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.Username) || string.IsNullOrEmpty(req.Password))
                return BadRequest(new { message = "Username and password are required." });

            var user = _service.Login(req.Username, req.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            if (user.Status != "Active")
                return StatusCode(403, new { message = "Your account is inactive. Please contact admin." });

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.EmployeeId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var jwtSettings = _configuration.GetSection("Jwt");

            var jwtKey = jwtSettings["Key"]
                ?? throw new Exception("JWT Key missing in appsettings.json");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(jwtSettings["DurationInMinutes"])
                ),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                message = "Login successful",
                token = tokenString,
                role = user.Role,
                user = new
                {
                    user.EmployeeId,
                    user.Name,
                    user.Username,
                    user.Department,
                    user.Designation,
                    user.Skillset,
                    user.Address,
                    user.JoiningDate,
                    user.Role,
                    user.Status
                }
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] Employee emp)
        {
            bool result = _service.Register(emp);

            if (!result)
                return BadRequest(new { message = "Registration failed" });

            return Ok(new { message = "Employee registered successfully" });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
