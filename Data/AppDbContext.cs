using Microsoft.EntityFrameworkCore;
using TaskPlanner.Api.Models;

namespace TaskPlanner.Api.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options)
  {
  }

  public DbSet<TaskItem> Tasks { get; set; }
}