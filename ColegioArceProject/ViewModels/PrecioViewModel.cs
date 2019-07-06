using System.ComponentModel.DataAnnotations;

namespace ColegioArceProject.ViewModels
{
    public class PrecioViewModel
    {
        public int id_Precio { get; set; }

        [Display(Name = "Concepto")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Concepto { set; get; }

        [Display(Name = "Importe")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public decimal Importe { get; set; }

        [Display(Name = "Rango de días")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public int Escolaridad { get; set; }

        [Display(Name = "Escolaridad")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Descripcion { get; set; }
    }
}