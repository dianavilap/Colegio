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
    
    public partial class Precio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Precio()
        {
            this.Pago = new HashSet<Pago>();
        }
    
        public int id_Precio { get; set; }
        public string Concepto { get; set; }
        public decimal Importe { get; set; }
        public int id_Rango { get; set; }
        public int id_Escolaridad { get; set; }
    
        public virtual Escolaridad Escolaridad { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pago> Pago { get; set; }
        public virtual Rango Rango { get; set; }
    }
}
