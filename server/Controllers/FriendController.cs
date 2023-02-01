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
        // Verify user.
        var userId = _pauliHelper.GetRequestUserId(HttpContext);
        var user = _context.Users.Find(userId);
        if (user == null) return BadRequest("User not found from JWT claim.");

        // Find friend who recieved the request.
        var FriendWhoGotTheRequest = _context.Users.Where(u => u.Username == Username).FirstOrDefault();
        if(FriendWhoGotTheRequest == null) return BadRequest("Unknown username");
        
        // See if the request was already sent.
        var FriendRequestAlreadySent = _context.FriendRequests
            .Where(p => p.SenderId == user.Id && p.RecieverId == FriendWhoGotTheRequest.Id).FirstOrDefault();
        if(FriendRequestAlreadySent != null) return BadRequest("Friend request already sent");
        
        // Add friend request to the database.
        _context.FriendRequests.Add(new FriendRequest
        {
            Id = Guid.NewGuid(),
            SenderId = user.Id,
            SenderUsername = user.Username,
            RecieverId = FriendWhoGotTheRequest.Id,
            RecieverUsername = FriendWhoGotTheRequest.Username
        });
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    [Authorize]
    [Route("getrecievedfriendrequests")]
    public IActionResult GetFriendRequests()
    {
        // Verify user.
        var userId = _pauliHelper.GetRequestUserId(HttpContext);
        var user = _context.Users.Find(userId);
        if (user == null) return BadRequest("User not found from JWT claim.");

        // Find friend requests the user has recieved and return them.
        var FriendRequests = _context.FriendRequests.Where(p => p.RecieverId == user.Id).ToList();
        return Ok(FriendRequests);
    }

    [HttpGet]
    [Authorize]
    [Route("getsentfriendrequests")]
    public IActionResult GetSentFriendRequests()
    {
        // Verify user.
        var userId = _pauliHelper.GetRequestUserId(HttpContext);
        var user = _context.Users.Find(userId);
        if (user == null) return BadRequest("User not found from JWT claim.");

        // Find friend requests the user has sent and return them.
        var FriendRequests = _context.FriendRequests.Where(p => p.SenderId == user.Id).ToList();
        return Ok(FriendRequests);
    }

    [HttpDelete]
    [Authorize]
    [Route("removesentfriendrequests")]
    public IActionResult RemoveSentFriendRequests([FromBody] Guid FriendRequestId)
    {
        // Verify user
        var userId = _pauliHelper.GetRequestUserId(HttpContext);
        var user = _context.Users.Find(userId);
        if (user == null) return BadRequest("User not found from JWT claim.");
        
        // Find friend requests the user has sent.
        var FriendRequest = _context.FriendRequests.Where(p => p.SenderId == user.Id && p.Id == FriendRequestId).SingleOrDefault();
        if(FriendRequest == null) return NotFound();
        
        // Delete friend Request
        _context.FriendRequests.Remove(FriendRequest);
        _context.SaveChanges();
        
        return NoContent();
    }

    [HttpPost]
    [Authorize]
    [Route("acceptfriendrequest")]
    public IActionResult AcceptFriendRequest([FromBody] Guid FriendRequestId)
    {
        // Verify user
        var userId = _pauliHelper.GetRequestUserId(HttpContext);
        var user = _context.Users.Where(p => p.Id == userId).FirstOrDefault();
        if (user == null) return BadRequest("User not found from JWT claim.");

        // Find the friend request the user has recieved.
        var TheFriendRequest = _context.FriendRequests.FirstOrDefault(p => p.Id == FriendRequestId && p.RecieverId == userId);
        if(TheFriendRequest == null) return NotFound("No such friend request");

        // Create a new friendship
        var Friendship = new Friendship
        {
            Id = Guid.NewGuid(),
            AliceId = user.Id,
            AliceUsername = user.Username,
            BobId = TheFriendRequest.SenderId,
            BobUsername = TheFriendRequest.SenderUsername,
            TimeBeenFriends = DateTime.UtcNow,
            FriendshipPoints = 0
        };

        // Add friendship to database and delete the friend request.
        _context.Friendships.Add(Friendship);
        _context.FriendRequests.Remove(TheFriendRequest);
        _context.SaveChanges();

        return Ok();
    }

    [HttpGet]
    [Authorize]
    [Route("detmyfriends")]
    public IActionResult GetMyFriends()
    {
        // Verify user
        var userId = _pauliHelper.GetRequestUserId(HttpContext);
        var user = _context.Users.Where(p => p.Id == userId).FirstOrDefault();
        if (user == null) return BadRequest("User not found from JWT claim.");

        // Get list of friends
        var UsersFriends = _context.Friendships.Where(f => f.AliceId == user.Id || f.BobId == user.Id).ToList();
        return Ok(UsersFriends);
    }

    [HttpDelete]
    [Authorize]
    [Route("deleteonefriend")]
    public IActionResult DeleteOneFriend([FromBody] Guid FriendshipId)
    {
        // Verify user
        var userId = _pauliHelper.GetRequestUserId(HttpContext);
        var user = _context.Users.Where(p => p.Id == userId).FirstOrDefault();
        if (user == null) return BadRequest("User not found from JWT claim.");

        // Look for friendship
        var TheFriendShip = _context.Friendships.Find(FriendshipId);
        if(TheFriendShip == null) return NotFound();
        
        // Delete friendship
        _context.Friendships.Remove(TheFriendShip);
        _context.SaveChanges();
        return NoContent();
    }
}