using STOREEX.Models;
using STOREEX.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace STOREEX.Controllers
{
    [Authorize(Roles = "Admin,Manager,Cashier")]
    public class POSController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private const int CompanyId = 1; // replaced by TenantContext in Identity phase

        public ActionResult Index()
        {
            ViewBag.Warehouses = new SelectList(db.Warehouses.Where(x => x.CompanyId == CompanyId && x.IsActive), "Id", "Name");
            return View();
        }

        [HttpGet]
        public JsonResult FindProduct(string q, int warehouseId)
        {
            q = (q ?? "").Trim();
            var product = db.Products.Where(x => x.CompanyId == CompanyId && x.IsActive && (x.Barcode == q || x.SKU == q || x.Name.Contains(q)))
                .Select(x => new { x.Id, x.Name, x.Barcode, x.SKU, x.SalePrice, x.VatRate }).FirstOrDefault();
            if (product == null) return Json(new { success = false, message = "Product not found." }, JsonRequestBehavior.AllowGet);
            var stock = db.WarehouseStocks.Where(x => x.CompanyId == CompanyId && x.WarehouseId == warehouseId && x.ProductId == product.Id).Select(x => (decimal?)x.Quantity).FirstOrDefault() ?? 0;
            return Json(new { success = true, product, stock }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Checkout(PosCheckoutViewModel model)
        {
            if (!ModelState.IsValid || model.Lines == null || model.Lines.Count == 0)
                return Json(new { success = false, message = "Cart is empty or invalid." });

            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    var invoice = new Invoice { CompanyId = CompanyId, WarehouseId = model.WarehouseId, CustomerId = model.CustomerId, InvoiceNumber = "INV-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"), InvoiceDate = DateTime.UtcNow, Status = InvoiceStatus.Issued };
                    decimal subtotal = 0, vatTotal = 0;

                    foreach (var requestLine in model.Lines)
                    {
                        if (requestLine.Quantity <= 0) throw new InvalidOperationException("Invalid quantity.");
                        var product = db.Products.Single(x => x.CompanyId == CompanyId && x.Id == requestLine.ProductId && x.IsActive);
                        var stock = db.WarehouseStocks.SingleOrDefault(x => x.CompanyId == CompanyId && x.WarehouseId == model.WarehouseId && x.ProductId == product.Id);
                        if (product.TrackStock && (stock == null || stock.Quantity < requestLine.Quantity)) throw new InvalidOperationException("Insufficient stock for " + product.Name);

                        var gross = product.SalePrice * requestLine.Quantity;
                        var discount = gross * (requestLine.DiscountPercent / 100m);
                        var net = gross - discount;
                        var vat = net * (product.VatRate / 100m);
                        invoice.Items.Add(new InvoiceItem { ProductId = product.Id, Description = product.Name, Quantity = requestLine.Quantity, UnitPrice = product.SalePrice, DiscountPercent = requestLine.DiscountPercent, VatRate = product.VatRate, LineTotal = net + vat });
                        subtotal += net; vatTotal += vat;

                        if (product.TrackStock)
                        {
                            stock.Quantity -= requestLine.Quantity;
                            db.StockMovements.Add(new StockMovement { CompanyId = CompanyId, WarehouseId = model.WarehouseId, ProductId = product.Id, Type = StockMovementType.Sale, Quantity = -requestLine.Quantity, Reference = invoice.InvoiceNumber });
                        }
                    }

                    var globalDiscount = subtotal * (model.GlobalDiscountPercent / 100m);
                    invoice.Subtotal = subtotal;
                    invoice.DiscountTotal = globalDiscount;
                    invoice.VatTotal = vatTotal;
                    invoice.Total = subtotal - globalDiscount + vatTotal;
                    invoice.PaidAmount = invoice.Total;
                    invoice.Status = InvoiceStatus.Paid;
                    db.Invoices.Add(invoice);
                    db.Payments.Add(new Payment { CompanyId = CompanyId, Invoice = invoice, Amount = invoice.Total, Method = (PaymentMethod)model.PaymentMethod, PaidAt = DateTime.UtcNow });
                    db.SaveChanges();
                    tx.Commit();
                    return Json(new { success = true, invoiceId = invoice.Id, invoiceNumber = invoice.InvoiceNumber, total = invoice.Total, change = Math.Max(0, model.AmountTendered - invoice.Total) });
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
    }
}
