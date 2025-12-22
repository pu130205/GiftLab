using System;
using GiftLab.Data.Entities;
using Microsoft.EntityFrameworkCore;
using DbAttribute = GiftLab.Data.Entities.Attribute;

namespace GiftLab.Data
{
    public partial class GiftLabDbContext : DbContext
    {
        public GiftLabDbContext(DbContextOptions<GiftLabDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<TransacStatus> TransacStatuses { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<DbAttribute> Attributes { get; set; }
        public virtual DbSet<AttributesPrice> AttributesPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== Accounts =====
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.AccountID).ValueGeneratedNever();

                entity.HasOne(d => d.Role)
                      .WithMany(p => p.Accounts)
                      .HasForeignKey(d => d.RoleID)
                      .HasConstraintName("FK_Accounts_Roles");
            });

            // ===== Roles =====
            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleID).ValueGeneratedNever();
            });

            // ===== Customers =====
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerID).ValueGeneratedOnAdd();

                // Email nvarchar(150) để khớp migration bạn đang theo
                entity.Property(e => e.Email).HasMaxLength(150);

                // Phone varchar(12)
                entity.Property(e => e.Phone)
                      .HasColumnType("varchar(12)")
                      .IsUnicode(false)
                      .HasMaxLength(12);
            });

            // ===== Categories =====
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CatID).ValueGeneratedNever();
            });

            // ===== Products =====
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductID).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Cat)
                      .WithMany(p => p.Products)
                      .HasForeignKey(d => d.CatID)
                      .HasConstraintName("FK_Products_Categories");
            });

            // ===== Attributes =====
            modelBuilder.Entity<DbAttribute>(entity =>
            {
                entity.Property(e => e.AttributeID).ValueGeneratedNever();
            });

            // ===== AttributesPrices =====
            modelBuilder.Entity<AttributesPrice>(entity =>
            {
                entity.Property(e => e.AttributesPriceID).ValueGeneratedNever();

                entity.HasOne(d => d.Attribute)
                      .WithMany(p => p.AttributesPrices)
                      .HasForeignKey(d => d.AttributeID)
                      .HasConstraintName("FK_AttributesPrices_Attributes");

                entity.HasOne(d => d.Product)
                      .WithMany(p => p.AttributesPrices)
                      .HasForeignKey(d => d.ProductID)
                      .HasConstraintName("FK_AttributesPrices_Products");
            });

            // ===== TransacStatus =====
            modelBuilder.Entity<TransacStatus>(entity =>
            {
                entity.Property(e => e.TransactStatusID).ValueGeneratedNever();

                // ✅ seed đồng bộ admin + customer
                entity.HasData(
                    new TransacStatus { TransactStatusID = 1, Status = "Pending", Description = "Đang chờ xử lý" },
                    new TransacStatus { TransactStatusID = 2, Status = "Processing", Description = "Đang xử lý" },
                    new TransacStatus { TransactStatusID = 3, Status = "Shipped", Description = "Đang giao hàng" },
                    new TransacStatus { TransactStatusID = 4, Status = "Completed", Description = "Hoàn thành" },
                    new TransacStatus { TransactStatusID = 5, Status = "Cancelled", Description = "Đã hủy" }
                );
            });

            // ===== Orders =====
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderID).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Customer)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(d => d.CustomerID)
                      .HasConstraintName("FK_Orders_Customers");

                entity.HasOne(d => d.TransactionStatus)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(d => d.TransactionStatusID)
                      .HasPrincipalKey(p => p.TransactStatusID)
                      .HasConstraintName("FK_Orders_TransacStatus");

                // ✅ ép kiểu & maxlen snapshot shipping cho chắc
                entity.Property(e => e.ReceiverName).HasMaxLength(150);
                entity.Property(e => e.ReceiverEmail).HasMaxLength(150);

                entity.Property(e => e.ReceiverPhone)
                      .HasColumnType("varchar(12)")
                      .IsUnicode(false)
                      .HasMaxLength(12);

                entity.Property(e => e.ShipAddress).HasMaxLength(255);
                entity.Property(e => e.ShipDistrict).HasMaxLength(100);
                entity.Property(e => e.ShipWard).HasMaxLength(100);
            });

            // ===== OrderDetails =====
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                // ✅ QUAN TRỌNG: OrderDetailID là Identity
                entity.Property(e => e.OrderDetailID).ValueGeneratedOnAdd();

                entity.Property(e => e.ProductName).HasMaxLength(255);

                entity.HasOne(d => d.Order)
                      .WithMany(p => p.OrderDetails)
                      .HasForeignKey(d => d.OrderID)
                      .HasConstraintName("FK_OrderDetails_Orders");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
