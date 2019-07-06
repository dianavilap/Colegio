using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ColegioArceProject.ViewModels
{
    

    public class ReporteAlumnoViewModel
    {
        [Display(Name = "Alumno")]
        public string NombreCompleto { get; set; }

        [Display(Name = "Escolaridad")]
        public string Escolaridad { get; set; }

        [Display(Name = "Grupo")]
        public string Grupo { get; set; }

        [Display(Name = "Nombre del Alumno")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Alumno { get; set; }

        public List<ReporteViewModel> ListaPagos { get; set; }
    }
}