using server.Models;
using server.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserAccountController : ControllerBase
{
    private ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public UserAccountController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserLoginDTO UserRequest)
    {
        var existingUser = _context.UserAccounts
            .FirstOrDefault(u => u.Username == UserRequest.Username);

        if (existingUser != null)
        {
            return BadRequest("Username already exists");
        }

        await _context.UserAccounts.AddAsync(new UserAccount 
        {
            Id = Guid.NewGuid(),
            Username = UserRequest.Username,
            // Hash and salt this asap!
            Password = UserRequest.Password
        });
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginDTO UserRequest)
    {
        var existingUser = _context.UserAccounts
            .Where(u => u.Username == UserRequest.Username && u.Password == UserRequest.Password).FirstOrDefault();

        if (existingUser == null)
        {
            return BadRequest();
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("Id", existingUser.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"]
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new { token = tokenHandler.WriteToken(token) });
    }
}
