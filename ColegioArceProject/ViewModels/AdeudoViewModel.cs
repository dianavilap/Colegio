using ColegioArceProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ColegioArceProject.ViewModels
{
    public class AdeudoViewModel
    {
        [Display(Name = "Grupo")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public int id_Grupo { get; set; }


        public string Grupo { get; set; }


        public List<GrupoAlumno> Alumnos { get; set; }
    }
}