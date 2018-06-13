using System.ComponentModel.DataAnnotations;

namespace Auctions.Models
{
    public class RegisterViewModel 
    {        
        [Required]
        [Display(Name = "Username")]
        [RegularExpression(@"^[a-zA-Z]+$")]
        public string username{get;set;}
        
        [Required]
        [Display(Name = "First Name")]
        [RegularExpression(@"^[a-zA-Z]+$")]
        public string first_name{get;set;}
        
        [Required]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z]+$")]
        public string last_name{get;set;}

        [Required]
        [Display(Name = "Password")]
        [MinLength(8,ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string password{get;set;}

        [Required]
        [Display(Name="Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Passwords do not match.")]
        public string PasswordConfirmation{get;set;} 
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        [RegularExpression(@"^[a-zA-Z]+$")]
        public string LogUsername{get;set;}

        [Required]
        [Display(Name = "Password")]
        [MinLength(8,ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string LogPassword{get;set;}
    }
}