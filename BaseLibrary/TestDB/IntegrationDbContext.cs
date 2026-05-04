using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.TestDB;

public partial class IntegrationDbContext : DbContext
{
    public IntegrationDbContext()
    {
    }

    public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cargo> Cargos { get; set; }

    public virtual DbSet<CargoType> CargoTypes { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<ChatMember> ChatMembers { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<DriversShift> DriversShifts { get; set; }

    public virtual DbSet<Incident> Incidents { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Shipping> Shippings { get; set; }

    public virtual DbSet<ShippingOrder> ShippingOrders { get; set; }

    public virtual DbSet<TransportCargoType> TransportCargoTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=toor;database=IntegrationLab", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.14-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("cargos")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CargoTypeId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("cargo_type_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DangerLevel)
                .HasColumnType("enum('Low','Medium','High','Extreme')")
                .HasColumnName("danger_level");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.DimensionsHeight).HasColumnName("dimensions_height");
            entity.Property(e => e.DimensionsLength).HasColumnName("dimensions_length");
            entity.Property(e => e.DimensionsWidth).HasColumnName("dimensions_width");
            entity.Property(e => e.Name)
                .HasMaxLength(40)
                .HasColumnName("name");
            entity.Property(e => e.ShippingId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("shipping_id");
            entity.Property(e => e.ShippingOrderId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("shipping_order_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Weight).HasColumnName("weight");
        });

        modelBuilder.Entity<CargoType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("cargo_types")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Title)
                .HasMaxLength(60)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("chats")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.IsPrivateChat)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_private_chat");
            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ChatMember>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("chat_members")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.ChatId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("chat_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("drivers")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Rights, "drivers_rights_index");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DriversLicense)
                .HasMaxLength(40)
                .HasColumnName("drivers_license");
            entity.Property(e => e.Rights)
                .HasColumnType("mediumint(8) unsigned")
                .HasColumnName("rights");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<DriversShift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("drivers_shifts")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DriverId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("driver_id");
            entity.Property(e => e.End)
                .HasColumnType("datetime")
                .HasColumnName("end");
            entity.Property(e => e.Start)
                .HasColumnType("datetime")
                .HasColumnName("start");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });
        
        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("incidents")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.DriverId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("driver_id");
            entity.Property(e => e.IncidentDate)
                .HasColumnType("datetime")
                .HasColumnName("incident_date");
            entity.Property(e => e.ShippingId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("shipping_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Pending'")
                .HasColumnType("enum('Pending','InProgress','Resolved')")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("messages")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.ChatId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("chat_id");
            entity.Property(e => e.Content)
                .HasMaxLength(300)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("deleted_at");
            entity.Property(e => e.SenderId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("sender_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Shipping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("shippings")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("delivery_date");
            entity.Property(e => e.DeliveryPoint)
                .HasMaxLength(120)
                .HasColumnName("delivery_point");
            entity.Property(e => e.DesignatedDriverId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("designated_driver_id");
            entity.Property(e => e.EstimatedDeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("estimated_delivery_date");
            entity.Property(e => e.ShippedDate)
                .HasColumnType("datetime")
                .HasColumnName("shipped_date");
            entity.Property(e => e.ShippingDate).HasColumnName("shipping_date");
            entity.Property(e => e.ShippingStatus)
                .HasDefaultValueSql("'InProcessing'")
                .HasColumnType("enum('InProcessing','ReadyToShip','Shipping','Delivered','Incident')")
                .HasColumnName("shipping_status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.VehicleId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("vehicle_id");
        });

        modelBuilder.Entity<ShippingOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("shipping_orders")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.ReceivedDate)
                .HasColumnType("datetime")
                .HasColumnName("received_date");
            entity.Property(e => e.ReceiverFio)
                .HasMaxLength(255)
                .HasColumnName("receiver_fio");
            entity.Property(e => e.ReceiverPhone)
                .HasMaxLength(255)
                .HasColumnName("receiver_phone");
            entity.Property(e => e.SentDate)
                .HasColumnType("datetime")
                .HasColumnName("sent_date");
            entity.Property(e => e.ShippingDate)
                .HasColumnType("datetime")
                .HasColumnName("shipping_date");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'InProcessing'")
                .HasColumnType("enum('InProcessing','InProgress')")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<TransportCargoType>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("transport_cargo_types")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.CargoTypeId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("cargo_type_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.VehicleId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("vehicle_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("users")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Email, "users_email_unique").IsUnique();

            entity.HasIndex(e => e.Login, "users_login_unique").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(64)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerifiedAt)
                .HasColumnType("timestamp")
                .HasColumnName("email_verified_at");
            entity.Property(e => e.FirstName)
                .HasMaxLength(40)
                .HasColumnName("first_name");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");
            entity.Property(e => e.LastName)
                .HasMaxLength(40)
                .HasColumnName("last_name");
            entity.Property(e => e.Login)
                .HasMaxLength(32)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(512)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(40)
                .HasColumnName("patronymic");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RememberToken)
                .HasMaxLength(100)
                .HasColumnName("remember_token");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'1'")
                .HasColumnType("mediumint(8) unsigned")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("vehicles")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.VehicleNumberPlate, "vehicles_vehicle_number_plate_unique").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.BodySizeHeight).HasColumnName("body_size_height");
            entity.Property(e => e.BodySizeLength).HasColumnName("body_size_length");
            entity.Property(e => e.BodySizeWidth).HasColumnName("body_size_width");
            entity.Property(e => e.BodyType)
                .HasColumnType("enum('Awning','Van','Isothermal','Refrigerator','OnboardPlatform','DumpTruck','Tank','ContainerShip')")
                .HasColumnName("body_type");
            entity.Property(e => e.Brand)
                .HasMaxLength(20)
                .HasColumnName("brand");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.LiftingCapacity).HasColumnName("lifting_capacity");
            entity.Property(e => e.MaxCargoVolume).HasColumnName("max_cargo_volume");
            entity.Property(e => e.Model)
                .HasMaxLength(40)
                .HasColumnName("model");
            entity.Property(e => e.NeededRights)
                .HasColumnType("mediumint(8) unsigned")
                .HasColumnName("needed_rights");
            entity.Property(e => e.NumberOfAxes)
                .HasColumnType("tinyint(3) unsigned")
                .HasColumnName("number_of_axes");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.VehicleNumberPlate)
                .HasMaxLength(15)
                .HasColumnName("vehicle_number_plate");
            entity.Property(e => e.VehicleSizeHeight).HasColumnName("vehicle_size_height");
            entity.Property(e => e.VehicleSizeLength).HasColumnName("vehicle_size_length");
            entity.Property(e => e.VehicleSizeWidth).HasColumnName("vehicle_size_width");
            entity.Property(e => e.VehicleWeight).HasColumnName("vehicle_weight");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
