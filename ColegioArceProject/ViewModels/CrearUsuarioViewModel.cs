using System.ComponentModel.DataAnnotations;
namespace ColegioArceProject.ViewModels
{
    public class CrearUsuarioViewModel
    {
        [Required(ErrorMessage = "{0} es requerido.")]
        [EmailAddress(ErrorMessage = "Dirección de correo electrónico inválida.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} es requerido.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "{0} es requerido.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "{0} es requerido.")]
        [Display(Name = "Rol de usuario")]
        public int id_Rol { get; set; }
    }
}