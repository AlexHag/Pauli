using server.Models;
using server.DataBase;
using server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace server.Controllers;

[ApiController]
[Route("api")]
public class UserAuthenticationController : ControllerBase
{
    private ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly IPauliHelper _pauliHelper;

    public UserAuthenticationController(ApplicationDbContext context, IConfiguration config, IPauliHelper pauliHelper)
    {
        _context = context;
        _config = config;
        _pauliHelper = pauliHelper;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] UserLoginDTO UserRequest)
    {
        var existingUser = _context.Users
            .FirstOrDefault(u => u.Username == UserRequest.Username);

        if (existingUser != null) return BadRequest("Username already exists");

        var userSalt = _pauliHelper.RandomString(16);
        var passwordHash = _pauliHelper.HashString(UserRequest.Password + userSalt);

        await _context.Users.AddAsync(new User 
        {
            Id = Guid.NewGuid(),
            Username = UserRequest.Username,
            Password = passwordHash,
            Salt = userSalt,
            // FriendRequests = new List<FriendRequest>(),
            // Friends = new List<Friendship>()
        });
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromBody] UserLoginDTO UserRequest)
    {
        var userSalt = _context.Users.Where(u => u.Username == UserRequest.Username).Select(u => u.Salt).FirstOrDefault();
        if (userSalt == null) return BadRequest("Wrong username or password");
        var passwordHash = _pauliHelper.HashString(UserRequest.Password + userSalt);
        
        var existingUser = _context.Users
            .Where(u => u.Username == UserRequest.Username && u.Password == passwordHash).FirstOrDefault();

        if (existingUser == null) return BadRequest("Wrong username or password");

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
