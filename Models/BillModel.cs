using System.ComponentModel.DataAnnotations;

namespace MVCDemo1.Models
{
    public class BillModel
    {
        public int BillID { get; set; }
        [Required]
        [MaxLength(10)]
        [MinLength(3)]
        [Display(Name = "Bill number")]
        public string BillNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Bill date")]
        public DateTime BillDate { get; set; }

        [Required]
        [Display(Name = "Order date")]
        public int OrderID { get; set; }

        [Required]
        [Display(Name = "Total amount")]
        public decimal TotalAmount { get; set; }
        [Required]
        public decimal Discount { get; set; }
        [Required(ErrorMessage = "Net amount must not be null")]
        public decimal NetAmount { get; set; }
        [Required]
        [Display(Name = "User name")]
        public int UserID { get; set; }
    }
}
