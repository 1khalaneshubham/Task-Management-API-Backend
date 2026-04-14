using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;

using TaskManagementAPI.Models;  // You'll create Models folder next

namespace TaskManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets will go here after you create models
        public DbSet<User> Users { get; set; }
        public DbSet<TodoItem> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}