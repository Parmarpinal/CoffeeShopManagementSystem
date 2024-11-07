using System.ComponentModel.DataAnnotations;

namespace MVCDemo1.Models
{
    public class OrderModel
    {
        public int OrderID { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public int CustomerID { get; set; }

        [Required]
        [Display(Name = "Order number")]
        public int OrderNumber { get; set; }

        [Required]
        [Display(Name = "date")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Payment mode must not be null")]
        public string PaymentMode { get; set; }

        [Required]
        [Display(Name = "total amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "shipping address")]
        public string ShippingAddress { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public int UserID { get; set; }

    }
    public class OrderDropDownModel
    {
        public int OrderID { get; set; }
        public int OrderNumber { get; set; }
    }
}
