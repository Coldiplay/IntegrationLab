using Microsoft.EntityFrameworkCore;
using BaseLibrary.Model;

namespace IntegrationAPI.Db;

public class IntegrationDbContext : DbContext
{
    //TODO: Потом вынести в переменные окружения
    private const string Connection = "server=localhost;user=root;password=toor;database=IntegrationLabDB";
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSqlite("Data Source=integration.db");
        optionsBuilder.UseMySql(Connection, ServerVersion.AutoDetect(Connection));
        //this.Database.EnsureCreated();
    }
    
    public DbSet<Break> Breaks { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<CargoType> CargoTypes { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMember> ChatMembers { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<DriversShift>  DriversShifts { get; set; }
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Shipping>  Shippings { get; set; }
    public DbSet<ShippingOrder> ShippingOrders { get; set; }
    public DbSet<TransportCargoTypes> TransportCargoTypes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cargo>().ComplexProperty(c => c.Dimensions);
        modelBuilder.Entity<Vehicle>().ComplexProperty(v => v.VehicleSize);
        modelBuilder.Entity<Vehicle>().ComplexProperty(v => v.BodySize);
        //modelBuilder.Entity<TransportCargoTypes>().
        
        base.OnModelCreating(modelBuilder);
    }
}