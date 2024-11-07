using System.ComponentModel.DataAnnotations;

namespace MVCDemo1.Models
{
    public class UserModel
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "User name")]

        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(12)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Enter valid mobile number")]
        [RegularExpression("^(\\+\\d{1,3}[- ]?)?\\d{10}$")]
        public string MobileNo { get; set; }

        [Required]
        [StringLength(100)]
        public string Address { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
    public class UserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }

    public class UserLoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
