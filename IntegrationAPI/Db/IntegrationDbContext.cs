using Microsoft.EntityFrameworkCore;
using Models.Model;

namespace IntegrationAPI.Db;

public class IntegrationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite("Data Source=integration.db");
    
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Shipping>  Shippings { get; set; }
    public DbSet<ShippingOrder> ShippingOrders { get; set; }
    public DbSet<CargoType> CargoTypes { get; set; }
    public DbSet<User>  Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cargo>().ComplexProperty(c => c.Dimensions);
        
        base.OnModelCreating(modelBuilder);
    }
}