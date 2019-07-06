using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ColegioArceProject.ViewModels
{
    public class ReporteViewModel
    {
        [Display(Name = "Nombre de Alumno")]
        public string Alumno { get; set; }

        public string id_GrupoAlumno { set; get; }

        public string id_Alumno { set; get; }

        public int id_pago { set; get; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Pago")]
        public string Fecha { set; get; }

        [Required(ErrorMessage = "{0} es requerido.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
       // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio {  get; set; }

        [Display(Name = "Fecha Final")]
        [Required(ErrorMessage = "{0} es requerido.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
       // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime FechaFin { set; get; }

       
        public string Folio { set; get; }

        public string TipoPago { set; get; }

        [Display(Name = "Alumno")]
        public string NombreCompleto { get; set; }

        [Display(Name = "Concepto de Pago")]
        public string ConceptoPago { get; set; }

        [Display(Name = "Importe")]
        public Decimal? Importe { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Nombre { set; get; }

        [Display(Name = "Apellido Paterno")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string ApellidoP { set; get; }

        [Display(Name = "Apellido Materno")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string ApellidoM { set; get; }

        // Reporte por Alumno
        [Display(Name = "Grupo")]
        public string Grupo { set; get; }


    }
}