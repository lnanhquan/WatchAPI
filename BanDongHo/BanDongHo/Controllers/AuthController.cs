using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WatchAPI.Constants;
using WatchAPI.DTOs;
using WatchAPI.Models.Entities;

namespace WatchAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _mapper = mapper;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[AuthConstants.RefreshTokenBytes];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string HashToken(string token)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is JwtSecurityToken jwt && jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return principal;
        }
        else
        {
            return null;
        }
    }

    private (string accessToken, string refreshToken) GenerateTokens(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpireMinutes"])),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        var refreshToken = GenerateRefreshToken();

        return (accessToken, refreshToken);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _userManager.FindByEmailAsync(dto.Email) != null)
        {
            Log.Warning("Registration failed: {Email} already exists", dto.Email);
            return BadRequest("Email already registered");
        }

        if (await _userManager.FindByNameAsync(dto.UserName) != null)
        {
            Log.Warning("Registration failed: {Username} already exists", dto.UserName);
            return BadRequest("Username already taken");
        }

        var user = new User { UserName = dto.UserName, Email = dto.Email, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            Log.Warning("Registration failed for {Email}: {Errors}", dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
            return BadRequest(result.Errors);
        }

        await _userManager.AddToRoleAsync(user, "User");
        Log.Information("User {Username} registered successfully: {Email}", dto.UserName, dto.Email);
        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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
        var (accessToken, refreshToken) = GenerateTokens(user, roles);

        user.RefreshToken = HashToken(refreshToken);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenExpiryDays"]));
        await _userManager.UpdateAsync(user);

        Log.Information("User {Username} logged in successfully", user.UserName);

        var authResponse = _mapper.Map<AuthResponseDTO>(user);
        authResponse.Roles = roles;
        authResponse.AccessToken = accessToken;
        authResponse.RefreshToken = refreshToken;

        return Ok(authResponse);
    }

    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return Ok(user != null);
    }

    [HttpGet("check-username")]
    public async Task<IActionResult> CheckUsername(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return Ok(user != null);
    }


    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO dto)
    {
        var principal = GetPrincipalFromExpiredToken(dto.AccessToken);
        if (principal == null)
        {
            return BadRequest("Invalid access token");
        }

        var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Unauthorized("User not found");
        }

        if (user.RefreshToken != HashToken(dto.RefreshToken))
        {
            return Unauthorized("Hash token is wrong");
        }

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Unauthorized("Refresh token is expired");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (newAccessToken, newRefreshToken) = GenerateTokens(user, roles);

        user.RefreshToken = HashToken(newRefreshToken);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenExpiryDays"]));
        await _userManager.UpdateAsync(user);

        Log.Information("Refresh token issued for user {Email} with Id {Id}", user.Email, user.Id);

        return Ok(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);

        Log.Information("User {Username} logged out", user.UserName);

        return Ok("Logged out");
    }
}
