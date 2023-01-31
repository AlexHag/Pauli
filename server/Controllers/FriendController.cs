using server.Models;
using server.DataBase;
using server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace server.Controllers;

[ApiController]
[Route("api")]
public class FriendsController : ControllerBase
{
    private ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly IPauliHelper _pauliHelper;

    public FriendsController(ApplicationDbContext context, IConfiguration config, IPauliHelper pauliHelper)
    {
        _context = context;
        _config = config;
        _pauliHelper = pauliHelper;
    }

    [HttpPost]
    [Authorize]
    [Route("sendfriendrequest")]
    public async Task<IActionResult> SendFriendRequest([FromBody] string Username)
    {
        var userId = _pauliHelper.GetRequestUserId(HttpContext);
        var user = _context.Users.Include(u => u.FriendRequests).SingleOrDefault(u => u.Id == userId);
        
        if (user == null) return Unauthorized("User not found from JWT claim.");

        var FriendWhoGotTheRequest = _context.Users.Include(u => u.FriendRequests).Where(u => u.Username == Username).FirstOrDefault();
        if(FriendWhoGotTheRequest == null) return BadRequest("Unknown username");
        
        var FriendRequestAlreadySent = FriendWhoGotTheRequest.FriendRequests.Where(p => p.FromId == user.Id).FirstOrDefault();
        if(FriendRequestAlreadySent != null) return BadRequest("Friend request already sent");
        
        FriendWhoGotTheRequest.FriendRequests.Add(new FriendRequest
        {
            FromId = user.Id,
            FromUsername = user.Username
        });

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    [Authorize]
    [Route("getfriendrequests")]
    public IActionResult GetFriendRequests()
    {
        var userId = _pauliHelper.GetRequestUserId(HttpContext);
        var user = _context.Users.Include(u => u.FriendRequests).SingleOrDefault(u => u.Id == userId);
        if (user == null) return Unauthorized("User not found from JWT claim. Should not happen");
        return Ok(user.FriendRequests);
    }

    [HttpPost]
    [Authorize]
    [Route("acceptfriendrequest")]
    public IActionResult AcceptFriendRequest([FromBody] Guid FriendRequestId)
    {
        var userId = _pauliHelper.GetRequestUserId(HttpContext);
        var user = _context.Users.Include(u => u.FriendRequests).Include(u => u.Friends).SingleOrDefault(u => u.Id == userId);
        if (user == null) return Unauthorized("User not found from JWT claim. Should not happen");

        var TheFriendRequest = user.FriendRequests.SingleOrDefault(fr => fr.Id == FriendRequestId);
        if(TheFriendRequest == null) return BadRequest("No such friend request");

        user.Friends.Add(new Friendship
        {
            Id = Guid.NewGuid(),
            FriendId = TheFriendRequest.FromId,
            FriendUsername = TheFriendRequest.FromUsername,
            TimeBeenFriends = DateTime.UtcNow
        });

        var TheFriend = _context.Users.Include(u => u.Friends).SingleOrDefault(u => u.Id == TheFriendRequest.FromId);
        if(TheFriend == null) return BadRequest("The friend does not exist, this should defenetly not happen...");
        TheFriend.Friends.Add(new Friendship
        {
            Id = Guid.NewGuid(),
            FriendId = user.Id,
            FriendUsername = user.Username,
            TimeBeenFriends = DateTime.UtcNow
        });

        _context.SaveChanges();
        return Ok();
    }
}