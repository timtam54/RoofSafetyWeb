using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RoofSafety.Models
{
    public class LoginModel
    {
        public string Error { get; set; }
        [BindProperty]
        [Required]
        [Display(Name = "Username/Email")]
        public string? Email { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]

        public string? password { get; set; }
    }
}
