using System.ComponentModel.DataAnnotations;

namespace ColegioArceProject.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordar esta cuenta?")]
        public bool RememberMe { get; set; }
    }
}