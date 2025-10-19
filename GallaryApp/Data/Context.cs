using GallaryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GallaryApp.Data;

public class Context : DbContext
{
    public DbSet<Photo> Photos { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=gallary.db");
    }
}