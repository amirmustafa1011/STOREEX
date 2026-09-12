using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STOREEX.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required, StringLength(150)] public string Name { get; set; }
        [StringLength(50)] public string FiscalNumber { get; set; }
        [StringLength(50)] public string VatNumber { get; set; }
        [StringLength(250)] public string Address { get; set; }
        [StringLength(100)] public string City { get; set; }
        [StringLength(10)] public string Currency { get; set; } = "EUR";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public abstract class CompanyEntity
    {
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")] public virtual Company Company { get; set; }
    }

    public class Category : CompanyEntity
    {
        public int Id { get; set; }
        [Required, StringLength(120)] public string Name { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Unit : CompanyEntity
    {
        public int Id { get; set; }
        [Required, StringLength(50)] public string Name { get; set; }
        [Required, StringLength(10)] public string Symbol { get; set; }
    }

    public class Product : CompanyEntity
    {
        public int Id { get; set; }
        [Required, StringLength(150)] public string Name { get; set; }
        [StringLength(80)] public string SKU { get; set; }
        [StringLength(80)] public string Barcode { get; set; }
        public int? CategoryId { get; set; }
        public int? UnitId { get; set; }
        [Column(TypeName = "money")] public decimal PurchasePrice { get; set; }
        [Column(TypeName = "money")] public decimal SalePrice { get; set; }
        public decimal VatRate { get; set; }
        public decimal MinimumStock { get; set; }
        public bool TrackStock { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public virtual Category Category { get; set; }
        public virtual Unit Unit { get; set; }
    }

    public class Customer : CompanyEntity
    {
        public int Id { get; set; }
        [Required, StringLength(150)] public string Name { get; set; }
        [StringLength(50)] public string FiscalNumber { get; set; }
        [StringLength(120)] public string Email { get; set; }
        [StringLength(50)] public string Phone { get; set; }
        [StringLength(250)] public string Address { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Supplier : CompanyEntity
    {
        public int Id { get; set; }
        [Required, StringLength(150)] public string Name { get; set; }
        [StringLength(50)] public string FiscalNumber { get; set; }
        [StringLength(120)] public string Email { get; set; }
        [StringLength(50)] public string Phone { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Warehouse : CompanyEntity
    {
        public int Id { get; set; }
        [Required, StringLength(120)] public string Name { get; set; }
        [StringLength(250)] public string Address { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class WarehouseStock : CompanyEntity
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        [Timestamp] public byte[] RowVersion { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Product Product { get; set; }
    }

    public enum StockMovementType { Opening = 1, Purchase = 2, Sale = 3, Adjustment = 4, TransferIn = 5, TransferOut = 6, Return = 7 }

    public class StockMovement : CompanyEntity
    {
        public long Id { get; set; }
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public StockMovementType Type { get; set; }
        public decimal Quantity { get; set; }
        [StringLength(80)] public string Reference { get; set; }
        [StringLength(500)] public string Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum InvoiceStatus { Draft = 1, Issued = 2, PartiallyPaid = 3, Paid = 4, Cancelled = 5 }

    public class Invoice : CompanyEntity
    {
        public long Id { get; set; }
        [Required, StringLength(40)] public string InvoiceNumber { get; set; }
        public int? CustomerId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Issued;
        public decimal Subtotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal VatTotal { get; set; }
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }

    public class InvoiceItem
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public int ProductId { get; set; }
        [Required, StringLength(150)] public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal VatRate { get; set; }
        public decimal LineTotal { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Product Product { get; set; }
    }

    public enum PaymentMethod { Cash = 1, Card = 2, Bank = 3, Other = 4 }

    public class Payment : CompanyEntity
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;
        [StringLength(100)] public string Reference { get; set; }
    }

    public class ExpenseCategory : CompanyEntity
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; }
    }

    public class Expense : CompanyEntity
    {
        public long Id { get; set; }
        public int ExpenseCategoryId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        [Required, StringLength(200)] public string Description { get; set; }
    }

    public class AuditLog : CompanyEntity
    {
        public long Id { get; set; }
        [StringLength(128)] public string UserId { get; set; }
        [Required, StringLength(100)] public string Action { get; set; }
        [StringLength(100)] public string EntityName { get; set; }
        [StringLength(80)] public string EntityId { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
