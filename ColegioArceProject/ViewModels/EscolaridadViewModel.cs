using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ColegioArceProject.ViewModels
{
    public class EscolaridadViewModel
    {
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Descripcion { set; get; }

        [Display(Name = "No. de Periodos")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public int No_periodos { get; set; }
        
    }
}