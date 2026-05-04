using Microsoft.EntityFrameworkCore;
using TestExamApp.Models;

namespace TestExamApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Library> Libraries { get; set; }
    public DbSet<Book> Books { get; set; }
}