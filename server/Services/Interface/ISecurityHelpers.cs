using server.Models;

namespace server.Services;

public interface IPauliHelper
{
    public string RandomString(int length);
    public string HashString(string input);
    public Guid GetRequestUserId(HttpContext context);
}