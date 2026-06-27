namespace MiniStationeryManagement.Mvc.Data;

using Microsoft.EntityFrameworkCore;
using MiniStationeryManagement.Mvc.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<StationeryItem> StationeryItems => Set<StationeryItem>();
    public DbSet<StationeryCategory> StationeryCategories => Set<StationeryCategory>();
    public DbSet<StationeryOrder> StationeryOrders => Set<StationeryOrder>();
    public DbSet<StationeryOrderItem> StationeryOrderItems => Set<StationeryOrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StationeryCategory>(entity =>
        {
            entity.ToTable("StationeryCategory");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<StationeryItem>(entity =>
        {
            entity.ToTable("StationeryItem");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Sku).IsRequired().HasMaxLength(20);
            entity.HasIndex(s => s.Sku).IsUnique();
            entity.Property(s => s.Barcode).IsRequired().HasMaxLength(50);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(150);
            entity.Property(s => s.Price).HasColumnType("numeric(18,2)");
            entity.Property(s => s.RowVersion).IsRowVersion();
            entity.HasQueryFilter(s => !s.IsDeleted);
            entity
                .HasOne(s => s.Category)
                .WithMany(c => c.StationeryItems)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StationeryOrder>(entity =>
        {
            entity.ToTable("StationeryOrder");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.CustomerName).IsRequired().HasMaxLength(100);
            entity.Property(o => o.TotalAmount).HasColumnType("numeric(18,2)");
        });

        modelBuilder.Entity<StationeryOrderItem>(entity =>
        {
            entity.ToTable("StationeryOrderItem");
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.UnitPrice).HasColumnType("numeric(18,2)");

            entity
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(oi => oi.StationeryItem)
                .WithMany()
                .HasForeignKey(oi => oi.StationeryItemId).IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // seed data
        modelBuilder
            .Entity<StationeryCategory>()
            .HasData(
                new StationeryCategory { Id = 1, Name = "Bút & Viết" },
                new StationeryCategory { Id = 2, Name = "Giấy & Sổ viết" }
            );

        modelBuilder
            .Entity<StationeryItem>()
            .HasData(
                new StationeryItem
                {
                    Id = 1,
                    Sku = "VPP-BUT-01",
                    Barcode = "8935212312341",
                    Name = "Bút Bi Thiên Long",
                    Supplier = "Tập đoàn Thiên Long",
                    Price = 5000,
                    Quantity = 50,
                    MinStock = 10,
                    CategoryId = 1,
                    LastUpdatedAt = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                },
                new StationeryItem
                {
                    Id = 2,
                    Sku = "VPP-SO-02",
                    Barcode = "8935212312342",
                    Name = "Sổ Lò Xo A5 Hải Tiến",
                    Supplier = "Công ty Giấy Hải Tiến",
                    Price = 25000,
                    Quantity = 3,
                    MinStock = 5,
                    CategoryId = 2,
                    LastUpdatedAt = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                },
                new StationeryItem
                {
                    Id = 3,
                    Sku = "VPP-GIAY-03",
                    Barcode = "8935212312343",
                    Name = "Ram Giấy Double A A4",
                    Supplier = "Double A Quốc Tế",
                    Price = 85000,
                    Quantity = 15,
                    MinStock = 5,
                    CategoryId = 2,
                    LastUpdatedAt = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                }
            );
    }
}
