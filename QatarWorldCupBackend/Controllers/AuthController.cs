using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QatarWorldCupBackend.DTO;
using QatarWorldCupBackend.Interfaces;
using QatarWorldCupBackend.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QatarWorldCupBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;



        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                FirstName = userDto.FirstName,  
                LastName = userDto.LastName,
                PasswordHash = ""  
            };

            try
            {
                await _userService.Register(user, userDto.Password);
                return Ok(new { Message = "Registration successful" });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTOLogin userDto)
        {
            var user = await _userService.Authenticate(userDto.Username, userDto.Password);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token, FirstName = user.FirstName, LastName = user.LastName });
        }
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["JwtKey"];
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpDelete("deleteUsers")]
        public async Task<IActionResult> DeleteAllData()
        {
            var success = await _userService.DeleteAllUsersAsync();
            if (!success) return BadRequest("Error during the deletion of all data.");
            return NoContent(); // Successfully deleted all data
        }

    }
}
