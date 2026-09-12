using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace STOREEX.Models
{
    public enum InvoiceStatus { Draft, Posted, Paid, PartiallyPaid, Cancelled, Returned }
    public enum PaymentMethod { Cash, Card, Bank, Other }
    public enum StockMovementType { Purchase, Sale, TransferIn, TransferOut, Adjustment, Return }
    public enum TransferStatus { Draft, Sent, Received, Cancelled }

    public class Company
    {
        public int Id { get; set; }
        [Required, StringLength(160)] public string Name { get; set; }
        [StringLength(40)] public string BusinessNumber { get; set; }
        [StringLength(40)] public string FiscalNumber { get; set; }
        [StringLength(30)] public string Phone { get; set; }
        [StringLength(180)] public string Email { get; set; }
        [StringLength(250)] public string Address { get; set; }
        [StringLength(80)] public string City { get; set; }
        [StringLength(10)] public string Currency { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual Company Company { get; set; }
    }

    public class Unit
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required, StringLength(30)] public string Name { get; set; }
        [StringLength(10)] public string ShortName { get; set; }
        public virtual Company Company { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? CategoryId { get; set; }
        public int? UnitId { get; set; }
        [Required, StringLength(180)] public string Name { get; set; }
        [StringLength(80)] public string SKU { get; set; }
        [StringLength(80)] public string Barcode { get; set; }
        [StringLength(500)] public string Description { get; set; }
        [Range(0, 999999999)] public decimal PurchasePrice { get; set; }
        [Range(0, 999999999)] public decimal SalePrice { get; set; }
        [Range(0, 100)] public decimal TaxRate { get; set; }
        public decimal MinimumStock { get; set; }
        public bool TrackStock { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Company Company { get; set; }
        public virtual Category Category { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
    }

    public class Customer
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required, StringLength(160)] public string Name { get; set; }
        [StringLength(40)] public string BusinessNumber { get; set; }
        [StringLength(40)] public string FiscalNumber { get; set; }
        [StringLength(30)] public string Phone { get; set; }
        [StringLength(180)] public string Email { get; set; }
        [StringLength(250)] public string Address { get; set; }
        [StringLength(80)] public string City { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal OpeningBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Company Company { get; set; }
    }

    public class Supplier
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required, StringLength(160)] public string Name { get; set; }
        [StringLength(40)] public string BusinessNumber { get; set; }
        [StringLength(40)] public string FiscalNumber { get; set; }
        [StringLength(30)] public string Phone { get; set; }
        [StringLength(180)] public string Email { get; set; }
        [StringLength(250)] public string Address { get; set; }
        [StringLength(80)] public string City { get; set; }
        public decimal OpeningBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Company Company { get; set; }
    }

    public class Warehouse
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; }
        [StringLength(30)] public string Code { get; set; }
        [StringLength(250)] public string Address { get; set; }
        [StringLength(80)] public string City { get; set; }
        [StringLength(30)] public string Phone { get; set; }
        public bool IsMain { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<WarehouseStock> Stocks { get; set; } = new List<WarehouseStock>();
    }

    public class WarehouseStock
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal AverageCost { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Product Product { get; set; }
    }

    public class StockMovement
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public StockMovementType Type { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        [StringLength(80)] public string Reference { get; set; }
        [StringLength(500)] public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        [StringLength(128)] public string CreatedBy { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Product Product { get; set; }
    }

    public class StockTransfer
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int FromWarehouseId { get; set; }
        public int ToWarehouseId { get; set; }
        public TransferStatus Status { get; set; }
        [StringLength(80)] public string Number { get; set; }
        [StringLength(500)] public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public virtual ICollection<StockTransferItem> Items { get; set; } = new List<StockTransferItem>();
    }

    public class StockTransferItem
    {
        public int Id { get; set; }
        public int StockTransferId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public virtual StockTransfer StockTransfer { get; set; }
        public virtual Product Product { get; set; }
    }

    public class Invoice
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public int? WarehouseId { get; set; }
        [Required, StringLength(50)] public string Number { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        [StringLength(500)] public string Note { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        [Required, StringLength(180)] public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Product Product { get; set; }
    }

    public class Payment
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? InvoiceId { get; set; }
        public int? CustomerId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public DateTime PaidAt { get; set; }
        [StringLength(100)] public string Reference { get; set; }
        [StringLength(300)] public string Note { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Customer Customer { get; set; }
    }

    public class Purchase
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
        [Required, StringLength(50)] public string Number { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public bool Posted { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    }

    public class PurchaseItem
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TaxRate { get; set; }
        public decimal LineTotal { get; set; }
        public virtual Purchase Purchase { get; set; }
        public virtual Product Product { get; set; }
    }

    public class ExpenseCategory
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; }
        public virtual Company Company { get; set; }
    }

    public class Expense
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? CategoryId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public DateTime ExpenseDate { get; set; }
        [StringLength(250)] public string Description { get; set; }
        public virtual ExpenseCategory Category { get; set; }
    }

    public class AuditLog
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        [StringLength(128)] public string UserId { get; set; }
        [StringLength(80)] public string Action { get; set; }
        [StringLength(80)] public string Entity { get; set; }
        public string EntityId { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}