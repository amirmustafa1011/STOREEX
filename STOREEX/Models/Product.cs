namespace STOREEX.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal VATRate { get; set; }
        public decimal MinStock { get; set; }
        public bool IsActive { get; set; }
    }
}
