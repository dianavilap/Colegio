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
    
    public partial class Alumno
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Alumno()
        {
            this.GrupoAlumno = new HashSet<GrupoAlumno>();
        }
    
        public int id_Alumno { get; set; }
        public string Nombre { get; set; }
        public string ApellidoP { get; set; }
        public string ApellidoM { get; set; }
        public string Direccion { get; set; }
        public string Nom_mama { get; set; }
        public string Nom_papa { get; set; }
        public string Ciudad { get; set; }
        public string tel_p { get; set; }
        public string tel_m { get; set; }
        public string Correo { get; set; }
        public bool Activo { get; set; }
        public Nullable<System.DateTime> FechaNac { get; set; }
        public string Sexo { get; set; }
        public string Trabajo_p { get; set; }
        public string Trabajo_m { get; set; }
        public string Correo_m { get; set; }
        public string Correo_p { get; set; }
        public string TipoSangre { get; set; }
        public string Alergias { get; set; }
        public int Edad { get; set; }
        public string Tel_Alumno { get; set; }
        public string DireccionTP { get; set; }
        public string DireccionTM { get; set; }
        public System.DateTime FechaInsc { get; set; }
        public string EscuelaP { get; set; }
        public string Peso { get; set; }
        public string Estatura { get; set; }
        public string Lesion { get; set; }
        public string TratamientoMed { get; set; }
        public string Enfermedad { get; set; }
        public string Lentes { get; set; }
        public string Auditivo { get; set; }
        public string TrasladoAcc { get; set; }
        public string TelEmergencia { get; set; }
        public string ServicioMed { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrupoAlumno> GrupoAlumno { get; set; }
    }
}
