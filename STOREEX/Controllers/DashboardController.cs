using STOREEX.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace STOREEX.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var companyId = CurrentCompanyId();
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);

            ViewBag.SalesToday = db.Invoices.Where(x => x.CompanyId == companyId && x.Status != InvoiceStatus.Cancelled && x.InvoiceDate >= today).Select(x => (decimal?)x.Total).Sum() ?? 0;
            ViewBag.SalesMonth = db.Invoices.Where(x => x.CompanyId == companyId && x.Status != InvoiceStatus.Cancelled && x.InvoiceDate >= month).Select(x => (decimal?)x.Total).Sum() ?? 0;
            ViewBag.Customers = db.Customers.Count(x => x.CompanyId == companyId && x.IsActive);
            ViewBag.LowStock = db.WarehouseStocks.Count(x => x.CompanyId == companyId && x.Quantity <= x.Product.MinimumStock);

            return View();
        }

        [HttpGet]
        public JsonResult MonthlySales()
        {
            var companyId = CurrentCompanyId();
            var from = DateTime.Today.AddMonths(-11);
            var data = db.Invoices
                .Where(x => x.CompanyId == companyId && x.InvoiceDate >= from && x.Status != InvoiceStatus.Cancelled)
                .GroupBy(x => new { x.InvoiceDate.Year, x.InvoiceDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(x => x.Total) })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private int CurrentCompanyId()
        {
            // Phase 1 default tenant. This will be replaced by a centralized tenant context
            // after Identity user/company seeding is added.
            return 1;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
