using Backend.Entities;
using Microsoft.EntityFrameworkCore;
namespace Backend.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<User> Users { get; set; }
}
