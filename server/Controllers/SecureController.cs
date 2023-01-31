using server.Models;
using server.Services;
using server.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

[ApiController]
[Route("api/[controller]")]
public class SecureController : ControllerBase
{
    private ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly IPauliHelper pauliHelper;


    public SecureController(ApplicationDbContext context, IConfiguration config, IPauliHelper securityHelpers)
    {
        _context = context;
        _config = config;
        pauliHelper = securityHelpers;
    }

    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        var user = pauliHelper.GetRequestUser(HttpContext);
        if (user == null)
        {
            return BadRequest("User not found from JWT claim. This should not be possible.");
        }
        
        return Ok($"Hello {user.Username}");
    }
}