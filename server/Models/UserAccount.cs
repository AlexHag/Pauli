namespace server.Models;

public class UserAccount
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class UserLoginDTO
{
    public string Username { get; set; }
    public string Password { get; set; }
}