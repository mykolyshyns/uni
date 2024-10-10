using Microsoft.EntityFrameworkCore;

namespace DALEF.Models;

public partial class RegisteredUserDbContext : DbContext
{
    public RegisteredUserDbContext()
    {
    }

    public RegisteredUserDbContext(DbContextOptions<RegisteredUserDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblComment> TblComments { get; set; }

    public virtual DbSet<TblOrder> TblOrders { get; set; }

    public virtual DbSet<TblOrderProduct> TblOrderProducts { get; set; }

    public virtual DbSet<TblProduct> TblProducts { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-SKM2D9K;Initial Catalog=RegisteredUserDb;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblComment>(entity =>
        {
            entity.HasKey(e => e.CommentId);

            entity.ToTable("tblComments");

            entity.Property(e => e.CommentId).HasColumnName("commentId");
            entity.Property(e => e.CommentText).HasColumnName("commentText");
            entity.Property(e => e.CommentTime).HasColumnName("commentTime");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Product).WithMany(p => p.TblComments)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblComments_tblProducts");

            entity.HasOne(d => d.User).WithMany(p => p.TblComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblComments_tblUsers");
        });

        modelBuilder.Entity<TblOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("tblOrders");

            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.OrderDate).HasColumnName("orderDate");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("totalPrice");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.TblOrders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblOrders_tblUsers");
        });

        modelBuilder.Entity<TblOrderProduct>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("PK__OrderPro__08D097A39EB1F9D0");

            entity.ToTable("tblOrderProducts");

            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.TblOrderProducts)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderProd__Order__5441852A");

            entity.HasOne(d => d.Product).WithMany(p => p.TblOrderProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderProd__Produ__5535A963");
        });

        modelBuilder.Entity<TblProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.ToTable("tblProducts");

            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("productName");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("tblUsers");

            entity.HasIndex(e => e.UserName, "UserName").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("passwordHash");
            entity.Property(e => e.UserName)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("userName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
