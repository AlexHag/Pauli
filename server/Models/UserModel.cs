namespace server.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
}

public class Friendship
{
    public Guid Id { get; set; }
    public Guid AliceId { get; set; }
    public string AliceUsername { get; set; }
    public Guid BobId { get; set; }
    public string BobUsername { get; set; }
    public DateTime? TimeBeenFriends { get; set; }
    public int FriendshipPoints { get; set; }
}

public class FriendRequest
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderUsername { get; set; }
    public Guid RecieverId { get; set; }
    public string RecieverUsername { get; set; }
}


public class UserLoginDTO
{
    public string Username { get; set; }
    public string Password { get; set; }
}