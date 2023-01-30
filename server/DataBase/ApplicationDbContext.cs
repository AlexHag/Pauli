using server.Models;
using Microsoft.EntityFrameworkCore;

namespace server.DataBase;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
    { }

    public DbSet<UserAccount> UserAccounts { get; set; }
}