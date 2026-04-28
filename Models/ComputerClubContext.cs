using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace computerclub.Models;

public partial class ComputerClubContext : DbContext
{
    public ComputerClubContext()
    {
    }

    public ComputerClubContext(DbContextOptions<ComputerClubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientRank> ClientRanks { get; set; }

    public virtual DbSet<Computer> Computers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Hall> Halls { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Tariff> Tariffs { get; set; }

    public virtual DbSet<ComputerGame> ComputerGames { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=ComputerClub;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Clients__E67E1A047B75BEDC");

            entity.HasIndex(e => e.Nickname, "UQ__Clients__CC6CD17EABB67035").IsUnique();

            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.Balance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Nickname).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RankId).HasColumnName("RankID");

            entity.HasOne(d => d.Rank).WithMany(p => p.Clients)
                .HasForeignKey(d => d.RankId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Clients__RankID__60A75C0F");
        });

        modelBuilder.Entity<ClientRank>(entity =>
        {
            entity.HasKey(e => e.RankId).HasName("PK__ClientRa__B37AFB96E9F21788");

            entity.Property(e => e.RankId).HasColumnName("RankID");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.RankName).HasMaxLength(50);
        });

        modelBuilder.Entity<Computer>(entity =>
        {
            entity.HasKey(e => e.ComputerId).HasName("PK__Computer__A6BE3C549FFFD905");

            entity.Property(e => e.ComputerId).HasColumnName("ComputerID");
            entity.Property(e => e.ComputerName).HasMaxLength(20);
            entity.Property(e => e.HallId).HasColumnName("HallID");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(15)
                .HasColumnName("IPAddress");

            entity.HasOne(d => d.Hall).WithMany(p => p.Computers)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Computers__HallI__59063A47");

            //entity.HasMany(d => d.Games).WithMany(p => p.Computers)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "ComputerGame",
            //        r => r.HasOne<Game>().WithMany()
            //            .HasForeignKey("GameId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK__ComputerG__GameI__5CD6CB2B"),
            //        l => l.HasOne<Computer>().WithMany()
            //            .HasForeignKey("ComputerId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK__ComputerG__Compu__5BE2A6F2"),
            //        j =>
            //        {
            //            j.HasKey("ComputerId", "GameId").HasName("PK__Computer__6415B5297E90242B");
            //            j.ToTable("ComputerGames");
            //            j.IndexerProperty<int>("ComputerId").HasColumnName("ComputerID");
            //            j.IndexerProperty<int>("GameId").HasColumnName("GameID");
            //        });
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF13074CC94");

            entity.HasIndex(e => e.Login, "IX_Employees_Login")
                .IsUnique()
                .HasFilter("([Login] IS NOT NULL)");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Employee");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employees__Posit__5629CD9C");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK__Games__2AB897DDBBAE10FA");

            entity.Property(e => e.GameId).HasColumnName("GameID");
            entity.Property(e => e.Developer).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.HallId).HasName("PK__Halls__7E60E274B2360471");

            entity.Property(e => e.HallId).HasColumnName("HallID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF4EE737E3");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.OrderTime).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.SessionId).HasColumnName("SessionID");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Employee__70DDC3D8");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__PaymentM__71D1E811");

            entity.HasOne(d => d.Session).WithMany(p => p.Orders)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__SessionI__6FE99F9F");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("PK__OrderDet__08D097C19BF93481");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.PriceAtPurchase).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__74AE54BC");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Produ__75A278F5");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1F3C0463972");

            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.MethodName).HasMaxLength(50);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__60BB9A59CEE888C8");

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED74780D26");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Catego__66603565");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ProductC__19093A2BAF6734A4");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__Sessions__C9F492708018F340");

            entity.Property(e => e.SessionId).HasColumnName("SessionID");
            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.ComputerId).HasColumnName("ComputerID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.TariffId).HasColumnName("TariffID");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Client).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__Client__693CA210");

            entity.HasOne(d => d.Computer).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.ComputerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__Comput__6A30C649");

            entity.HasOne(d => d.Employee).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__Employ__6B24EA82");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__Paymen__6D0D32F4");

            entity.HasOne(d => d.Tariff).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__Tariff__6C190EBB");
        });

        modelBuilder.Entity<Tariff>(entity =>
        {
            entity.HasKey(e => e.TariffId).HasName("PK__Tariffs__EBAF9D93A2BE31FF");

            entity.Property(e => e.TariffId).HasColumnName("TariffID");
            entity.Property(e => e.HallId).HasColumnName("HallID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TariffName).HasMaxLength(50);

            entity.HasOne(d => d.Hall).WithMany(p => p.Tariffs)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tariffs__HallID__6383C8BA");
        });

        modelBuilder.Entity<ComputerGame>(entity =>
        {
            entity.HasKey(e => new { e.ComputerId, e.GameId });

            entity.ToTable("ComputerGames");

            entity.Property(e => e.ComputerId).HasColumnName("ComputerID");
            entity.Property(e => e.GameId).HasColumnName("GameID");

            entity.HasOne(d => d.Computer)
                .WithMany(p => p.ComputerGames)
                .HasForeignKey(d => d.ComputerId)
                .HasConstraintName("FK_ComputerGames_Computers");

            entity.HasOne(d => d.Game)
                .WithMany(p => p.ComputerGames)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_ComputerGames_Games");
        });

    }




    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
