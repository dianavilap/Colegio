//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ColegioArceProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pago
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pago()
        {
            this.Abono1 = new HashSet<Abono>();
        }
    
        public int id_pago { get; set; }
        public System.DateTime Fecha { get; set; }
        public int id_Precio { get; set; }
        public bool Activo { get; set; }
        public decimal Importe { get; set; }
        public Nullable<bool> Abono { get; set; }
        public int id_GrupoAlumno { get; set; }
        public string TipoPago { get; set; }
        public int Folio { get; set; }
        public string DescripcionPago { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Abono> Abono1 { get; set; }
        public virtual GrupoAlumno GrupoAlumno { get; set; }
        public virtual Precio Precio { get; set; }
    }
}
