namespace server.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public List<Friendship> Friends { get; set; }
    public List<FriendRequest> FriendRequests { get; set; }
}

public class Friendship
{
    public Guid Id { get; set; }
    public Guid FriendId { get; set; }
    public string FriendUsername { get; set; }
    public DateTime? TimeBeenFriends { get; set; }
    public int FriendshipPoints { get; set; }
}

public class FriendRequest
{
    public Guid Id { get; set; }
    public Guid FromId { get; set; }
    public string FromUsername { get; set; }
}


public class UserLoginDTO
{
    public string Username { get; set; }
    public string Password { get; set; }
}