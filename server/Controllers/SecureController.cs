using server.Models;
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

    public SecureController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        return Ok("Secure Data");
    }
}