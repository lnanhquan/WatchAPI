using BanDongHo.DTOs;
using BanDongHo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BanDongHo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _userManager.FindByEmailAsync(dto.Email) != null)
                {
                    Log.Warning("Registration failed: {Email} already exists", dto.Email);
                    return BadRequest("Email already registered");
                }
                var user = new User { UserName = dto.Username, Email = dto.Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    Log.Warning("Registration failed for {Email}: {Errors}", dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return BadRequest(result.Errors);
                }

                await _userManager.AddToRoleAsync(user, "User");
                Log.Information("User {Username} registered successfully: {Email}", dto.Username, dto.Email);
                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during registration for {Email}", dto.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during registration");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    Log.Warning("Login failed: {Email} not found", dto.Email);
                    return Unauthorized("Invalid email");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

                if (!result.Succeeded)
                {
                    Log.Warning("Login failed for {Email}: Invalid password", dto.Email);
                    return Unauthorized("Invalid password");
                }

                var roles = await _userManager.GetRolesAsync(user);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };
                claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                    Issuer = _config["Jwt:Issuer"],
                    Audience = _config["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwt = tokenHandler.WriteToken(token);

                Log.Information("User {Username} logged in successfully: {Email}", user.UserName, dto.Email);

                return Ok(new AuthResponseDTO
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = roles,
                    Token = jwt
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during login for {Email}", dto.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during login");
            }
        }
    }
}
