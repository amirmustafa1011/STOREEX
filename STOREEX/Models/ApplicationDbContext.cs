using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace STOREEX.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? CompanyId { get; set; }
        public string FullName { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false) { }

        public static ApplicationDbContext Create() => new ApplicationDbContext();

        public DbSet<Company> Companies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseStock> WarehouseStocks { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WarehouseStock>()
                .HasIndex(x => new { x.CompanyId, x.WarehouseId, x.ProductId })
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(x => x.PurchasePrice).HasPrecision(18, 4);
            modelBuilder.Entity<Product>()
                .Property(x => x.SalePrice).HasPrecision(18, 4);
            modelBuilder.Entity<Invoice>()
                .Property(x => x.Total).HasPrecision(18, 4);
        }
    }
}
