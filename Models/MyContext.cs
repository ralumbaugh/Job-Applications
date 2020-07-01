using Microsoft.EntityFrameworkCore;

namespace JobApplications.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get; set;}
        public DbSet<Company> Companies {get; set;}
        public DbSet<Contact> Contacts {get; set;}
        public DbSet<Interview> Interviews {get; set;}
    }
}