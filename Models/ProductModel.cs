using System.ComponentModel.DataAnnotations;

namespace MVCDemo1.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Product name")]
        public string ProductName { get; set; }

        [Required]
        [Display(Name = "Price of the product")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Product code must be not null")]
        [MaxLength(20)]
        [MinLength(4)]
        public string ProductCode { get; set; }

        [Required]
        [StringLength(30)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "User name")]
        public int UserID { get; set; }
    }
    public class ProductDropDownModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
    }
}
