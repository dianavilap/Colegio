using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColegioArceProject.ViewModels
{
    public class ExcelViewModel
    {
        public string Fecha { set; get; }

        public string Folio { set; get; }

        public string NombreCompleto { set; get; }

        public string ConceptoPago  { set; get; }

        public string TipoPago { set; get; }

        public string Importe { set; get; }
    }
}