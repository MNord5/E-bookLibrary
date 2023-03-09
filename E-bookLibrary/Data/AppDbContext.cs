using E_bookLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace E_bookLibrary.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Ebooks { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}
