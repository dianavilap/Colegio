using ColegioArceProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ColegioArceProject.ViewModels
{
    public class GrupoViewModel
    {
        
        public int id_grupo { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Descripcion { set; get; }

        [Display(Name = "Escolaridad")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public int id_escolaridad { get; set; }

        public List<Escolaridad> EscolaridadList { get; set; }

        [Display(Name = "Escolaridad")]
        public string DescripcionEscolaridad { get; set; }

        public bool Activo { get; set; }
    }
}