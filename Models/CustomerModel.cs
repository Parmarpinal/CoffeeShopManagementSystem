using System.ComponentModel.DataAnnotations;

namespace MVCDemo1.Models
{
    public class CustomerModel
    {
        public int CustomerID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Customer name")]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "address")]
        public string HomeAddress { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter valid mobile number")]
        [RegularExpression("^(\\+\\d{1,3}[- ]?)?\\d{10}$")]
        public string MobileNo { get; set; }

        [Required]
        [MaxLength(10)]
        [MinLength(4)]
        [Display(Name = "GST no")]
        public string GSTNo { get; set; }

        [Required]
        [Display(Name = "City name")]
        public string CityName { get; set; }

        [Required]
        public string Pincode { get; set; }

        [Required(ErrorMessage = "Net amount must not be null")]

        public decimal NetAmount { get; set; }

        [Required]
        [Display(Name = "User name")]
        public int UserID { get; set; }
    }
    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
