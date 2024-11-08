﻿using System.ComponentModel.DataAnnotations;

namespace MVCDemo1.Models
{
    public class OrderDetailModel
    {
        public int OrderDetailID { get; set; }
        [Required]
        [Display(Name = "Order date")]
        public int OrderID { get; set; }
        [Required]
        [Display(Name = "Product name")]
        public int ProductID { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        [Display(Name = "User name")]
        public int UserID { get; set; }
    }
}
