using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace jupiterCore.jupiterContext
{
    public partial class jupiterContext : DbContext
    {
        public jupiterContext()
        {
        }

        public jupiterContext(DbContextOptions<jupiterContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<CartProd> CartProd { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<ContactEmail> ContactEmail { get; set; }
        public virtual DbSet<EventType> EventType { get; set; }
        public virtual DbSet<Faq> Faq { get; set; }
        public virtual DbSet<HomepageCarouselMedia> HomepageCarouselMedia { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductDetail> ProductDetail { get; set; }
        public virtual DbSet<ProductMedia> ProductMedia { get; set; }
        public virtual DbSet<ProductType> ProductType { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectMedia> ProjectMedia { get; set; }
        public virtual DbSet<Testimonial> Testimonial { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=35.197.166.191;port=3306;user=dbuser;password=ToMPyaJzCW88JPRqBkxqZpiiEElX7Tv1;database=jupiter");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin", "jupiter");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Password)
                    .HasMaxLength(160)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart", "jupiter");

                entity.HasIndex(e => e.ContactId)
                    .HasName("FK_Reference_11");

                entity.Property(e => e.CartId)
                    .HasColumnName("CartID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ContactId).HasColumnType("int(11)");

                entity.Property(e => e.IsActivate).HasColumnType("tinyint(4)");

                entity.Property(e => e.Location)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.Cart)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("FK_Reference_11");
            });

            modelBuilder.Entity<CartProd>(entity =>
            {
                entity.ToTable("CartProd", "jupiter");

                entity.HasIndex(e => e.CartId)
                    .HasName("FK_Reference_10");

                entity.HasIndex(e => e.ProdId)
                    .HasName("FK_Reference_14");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CartId).HasColumnType("int(11)");

                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");

                entity.Property(e => e.ProdId).HasColumnType("int(11)");

                entity.Property(e => e.Quantity).HasColumnType("int(11)");

                entity.Property(e => e.SubTitle)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartProd)
                    .HasForeignKey(d => d.CartId)
                    .HasConstraintName("FK_Reference_10");

                entity.HasOne(d => d.Prod)
                    .WithMany(p => p.CartProd)
                    .HasForeignKey(d => d.ProdId)
                    .HasConstraintName("FK_Reference_14");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact", "jupiter");

                entity.Property(e => e.ContactId).HasColumnType("int(11)");

                entity.Property(e => e.Email)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Message)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNum)
                    .HasMaxLength(12)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ContactEmail>(entity =>
            {
                entity.ToTable("ContactEmail", "jupiter");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Company)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.FindUs)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.LocationOfEvent)
                    .HasMaxLength(2500)
                    .IsUnicode(false);

                entity.Property(e => e.Message)
                    .HasMaxLength(2500)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.TypeOfEvent)
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EventType>(entity =>
            {
                entity.HasKey(e => e.TypeId);

                entity.ToTable("EventType", "jupiter");

                entity.Property(e => e.TypeId).HasColumnType("int(11)");

                entity.Property(e => e.EventName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.EventTypeImage)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Faq>(entity =>
            {
                entity.ToTable("Faq", "jupiter");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Answer)
                    .HasMaxLength(2500)
                    .IsUnicode(false);

                entity.Property(e => e.Question)
                    .HasMaxLength(2500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HomepageCarouselMedia>(entity =>
            {
                entity.ToTable("HomepageCarouselMedia", "jupiter");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProdId);

                entity.ToTable("Product", "jupiter");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("FK_Reference_9");

                entity.HasIndex(e => e.ProdTypeId)
                    .HasName("FK_Reference_8");

                entity.Property(e => e.ProdId).HasColumnType("int(11)");

                entity.Property(e => e.AvailableStock).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasMaxLength(2048)
                    .IsUnicode(false);

                entity.Property(e => e.Discount).HasColumnType("decimal(10,2)");

                entity.Property(e => e.IsActivate).HasColumnType("tinyint(4)");

                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");

                entity.Property(e => e.ProdTypeId).HasColumnType("int(11)");

                entity.Property(e => e.SpcOrDisct).HasColumnType("smallint(6)");

                entity.Property(e => e.SpecialOrder).HasColumnType("tinyint(4)");

                entity.Property(e => e.SubTitle)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TotalStock).HasColumnType("int(11)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Reference_9");

                entity.HasOne(d => d.ProdType)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.ProdTypeId)
                    .HasConstraintName("FK_Reference_8");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.ToTable("ProductCategory", "jupiter");

                entity.HasIndex(e => e.ProdTypeId)
                    .HasName("FK_Reference_17");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProdTypeId).HasColumnType("int(11)");

                entity.HasOne(d => d.ProdType)
                    .WithMany(p => p.ProductCategory)
                    .HasForeignKey(d => d.ProdTypeId)
                    .HasConstraintName("FK_Reference_17");
            });

            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.ToTable("ProductDetail", "jupiter");

                entity.HasIndex(e => e.ProdId)
                    .HasName("FK_Reference_15");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AvailableStock).HasColumnType("int(11)");

                entity.Property(e => e.Discount).HasColumnType("decimal(10,2)");

                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");

                entity.Property(e => e.ProdId).HasColumnType("int(11)");

                entity.Property(e => e.ProductDetail1)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TotalStock).HasColumnType("int(11)");

                entity.HasOne(d => d.Prod)
                    .WithMany(p => p.ProductDetail)
                    .HasForeignKey(d => d.ProdId)
                    .HasConstraintName("FK_Reference_15");
            });

            modelBuilder.Entity<ProductMedia>(entity =>
            {
                entity.ToTable("ProductMedia", "jupiter");

                entity.HasIndex(e => e.ProdId)
                    .HasName("FK_Reference_3");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ProdId).HasColumnType("int(11)");

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Prod)
                    .WithMany(p => p.ProductMedia)
                    .HasForeignKey(d => d.ProdId)
                    .HasConstraintName("FK_Reference_3");
            });

            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.HasKey(e => e.ProdTypeId);

                entity.ToTable("ProductType", "jupiter");

                entity.Property(e => e.ProdTypeId).HasColumnType("int(11)");

                entity.Property(e => e.TypeName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.ProdjectId);

                entity.ToTable("Project", "jupiter");

                entity.HasIndex(e => e.EventtypeId)
                    .HasName("FK_Reference_12");

                entity.Property(e => e.ProdjectId).HasColumnType("int(11)");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(2048)
                    .IsUnicode(false);

                entity.Property(e => e.EventtypeId).HasColumnType("int(11)");

                entity.HasOne(d => d.Eventtype)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.EventtypeId)
                    .HasConstraintName("FK_Reference_12");
            });

            modelBuilder.Entity<ProjectMedia>(entity =>
            {
                entity.ToTable("ProjectMedia", "jupiter");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("FK_Reference_6");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ProjectId).HasColumnType("int(11)");

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectMedia)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Reference_6");
            });

            modelBuilder.Entity<Testimonial>(entity =>
            {
                entity.ToTable("Testimonial", "jupiter");

                entity.HasIndex(e => e.EventtypeId)
                    .HasName("FK_Reference_13");

                entity.Property(e => e.TestimonialId).HasColumnType("int(11)");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.EventtypeId).HasColumnType("int(11)");

                entity.Property(e => e.Message)
                    .HasMaxLength(2500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Eventtype)
                    .WithMany(p => p.Testimonial)
                    .HasForeignKey(d => d.EventtypeId)
                    .HasConstraintName("FK_Reference_13");
            });
        }
    }
}
