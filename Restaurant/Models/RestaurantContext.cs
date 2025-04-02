using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Restaurant.Models;

public partial class RestaurantContext : DbContext
{
    public RestaurantContext(DbContextOptions<RestaurantContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Order> Order { get; set; }

    public virtual DbSet<OrderDetails> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payment { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<Status> Status { get; set; }

    public virtual DbSet<Table> Table { get; set; }

    public virtual DbSet<UserLogin> UserLogin { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderNumber);

            entity.HasIndex(e => e.UserNumber, "IX_Order_MemberNumber");

            entity.HasIndex(e => e.PaymentCode, "IX_Order_PaymentCode");

            entity.HasIndex(e => e.StatusCode, "IX_Order_StatusCode");


            entity.Property(e => e.OrderNumber)
                .HasMaxLength(12)
                .HasDefaultValueSql("(dbo.getOrderNumber())")
                .IsFixedLength();
            entity.Property(e => e.PaymentCode)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.StatusCode)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.UserNumber)
                .HasMaxLength(14)
                .IsFixedLength();
            entity.Property(e => e.OrderDate)
                .IsRequired();
            entity.Property(e => e.ExpectedArrivalDate)
                .IsRequired()
                .HasMaxLength(12); 


            entity.HasOne(d => d.PaymentCodeNavigation).WithMany(p => p.Order)
                .HasForeignKey(d => d.PaymentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Payment");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Order)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Status");


            entity.HasOne(d => d.UserNumberNavigation).WithMany(p => p.Order)
                .HasPrincipalKey(p => p.UserNumber)
                .HasForeignKey(d => d.UserNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_UserLogin");
        });

        modelBuilder.Entity<OrderDetails>(entity =>
        {
            entity.HasKey(e => new { e.OrderNumber, e.TableID });

            entity.HasIndex(e => e.TableID, "IX_OrderDetails_TableID");

            entity.Property(e => e.OrderNumber)
                .HasMaxLength(12)
                .IsFixedLength();
            entity.Property(e => e.TableID)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.Pricing)
                .IsRequired(); 
            entity.Property(e => e.Qty)
                .IsRequired(); 

            entity.HasOne(d => d.OrderNumberNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Order");

            entity.HasOne(d => d.Table).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.TableID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Table");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentCode);

            entity.Property(e => e.PaymentCode)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.PaymentType).HasMaxLength(20);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleCode);

            entity.Property(e => e.RoleCode)
                .HasMaxLength(2)
                .HasDefaultValue("MB")
                .IsFixedLength();
            entity.Property(e => e.Title).HasMaxLength(10);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusCode);

            entity.Property(e => e.StatusCode)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.StatusCategory).HasMaxLength(20);
        });


        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableID);
            entity.Property(e => e.TableID)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.Pricing).HasColumnType("money");
            entity.Property(e => e.Remark).IsRequired();
            entity.Property(e => e.Picture);
            entity.Property(e => e.ImageType);
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.UserID);

            entity.HasIndex(e => e.Account, "UQ_UserLogin_Account").IsUnique();

            entity.HasIndex(e => e.UserNumber, "UQ_UserLogin_UserNumber").IsUnique();

            entity.Property(e => e.Account).HasMaxLength(50);
            entity.Property(e => e.ID)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ManagerNumber)
                .HasMaxLength(14)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(25);
            entity.Property(e => e.Password).HasMaxLength(64);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.RoleCode)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.UserNumber)
                .HasMaxLength(14)
                .HasDefaultValueSql("(dbo.fnGetNewUserNumber())")
                .IsFixedLength();

            entity.HasOne(d => d.RoleCodeNavigation).WithMany(p => p.UserLogin)
                .HasForeignKey(d => d.RoleCode)
                .HasConstraintName("FK_UserLogin_RoleCode");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
