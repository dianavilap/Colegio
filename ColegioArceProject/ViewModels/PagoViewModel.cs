using ColegioArceProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ColegioArceProject.ViewModels
{
    public class PagoViewModel
    {
        public int id_pago { set; get; }

        [Display(Name = "Fecha de Pago")]
        [Required(ErrorMessage = "{0} es requerido.")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public String Fecha { set; get; }

        [Display(Name = "Alumno")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Alumno { get; set; }

        public int id_GrupoAlumno { set; get; }
        public int id_Alumno { set; get; }

        public bool Activo { set; get; }

        //[Display(Name = "Tipo de colegiatura")]
        //[Required(ErrorMessage = "{0} es requerido.")]
        public int id_Precio { set; get; }


        public string TipoPago { set; get; }

        [Display(Name = "Tipo de pago")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string comoPago { set; get; }

        public decimal Abono { set; get; }

        public decimal Promo { set; get; }

        public decimal? Importe { set; get; }
        //IndexPagos
        [Display(Name = "Descripción")]
        public string DescripcionPrecio { get; set; }

        public string Precio { get; set; }

        public string DescripcionEscolaridad { get; set; }

        //ReportesPagos
        [Required(ErrorMessage = "{0} es requerido.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha Final")]
        [Required(ErrorMessage = "{0} es requerido.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime FechaFin { set; get; }

        public int folio { get; set; }

        public List<Abono> Abonos { get; set; }

    }
}