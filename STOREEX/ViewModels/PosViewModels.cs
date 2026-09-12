using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace STOREEX.ViewModels
{
    public class PosCheckoutViewModel
    {
        [Required] public int WarehouseId { get; set; }
        public int? CustomerId { get; set; }
        public int PaymentMethod { get; set; }
        public decimal AmountTendered { get; set; }
        public decimal GlobalDiscountPercent { get; set; }
        [Required] public List<PosLineViewModel> Lines { get; set; }
    }

    public class PosLineViewModel
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}
