using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using server.Models;
using server.DataBase;

namespace server.Services;

public class PauliHelper : IPauliHelper
{
    private static Random random = new Random();
    private ApplicationDbContext _context;
    public PauliHelper(ApplicationDbContext context) 
    {
        _context = context;
    }
    public string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public string HashString(string input)
    {
        StringBuilder Sb = new StringBuilder();

        using (var hash = SHA256.Create())            
        {
            Encoding enc = Encoding.UTF8;
            byte[] result = hash.ComputeHash(enc.GetBytes(input));

            foreach (byte b in result)
                Sb.Append(b.ToString("x2"));
        }

        return Sb.ToString();
    }

    public User? GetRequestUser(HttpContext context)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        if(identity == null)
        {
            return null;
        }

        IEnumerable<Claim> claims = identity.Claims; 
        var claimId = identity.FindFirst("Id").Value;
        var user = _context.Users.Where(u => u.Id == Guid.Parse(claimId)).FirstOrDefault();
        
        if(user == null)
        {
            return null;
        }

        return user;
    }
}