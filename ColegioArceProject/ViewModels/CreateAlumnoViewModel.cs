using ColegioArceProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColegioArceProject.ViewModels
{
    public class CreateAlumnoViewModel
    {
        public int id_Alumno { set; get; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Nombre { set; get; }

        [Display(Name = "Apellido Paterno")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string ApellidoP  { set; get; }

        [Display(Name = "Apellido Materno")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string ApellidoM { set; get; }

        [Display(Name = "Edad")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public int Edad { get; set; }

        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Direccion { set; get; }

        [Display(Name = "Dirección del Trabajo de Papá")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string DireccionP { set; get; }

        [Display(Name = "Dirección del Trabajo de Mamá ")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string DireccionM { set; get; }

        [Display(Name = "Nombre Mamá")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Nom_mama { set; get; }

        [Display(Name = "Nombre Papá")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Nom_papa { set; get; }

        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Ciudad { set; get; }

        [Display(Name = "Teléfono Alumno")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string tel_A { set; get; }

        [Display(Name = "Teléfono Papá")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string tel_p { set; get; }

        [Display(Name = "Teléfono Mamá")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string tel_m { set; get; }

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "{0} es requerido.")]
        [EmailAddress(ErrorMessage = "Dirección de correo electrónico inválida.")]
        public string Correo { set; get; }

        [Display(Name = "Grupo")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public int id_Grupo { set; get; }
        public List<Grupo> GrupoList { get; set; }

        public List<Alumno> AlumnoList { get; set; }

        [Display(Name = "Grupo")]
        public string GrupoDescripcion { set; get; }

        [Display(Name = "Escolaridad")]
        public int id_escolaridad { set; get; }
        public List<Escolaridad> EscolaridadList { get; set; }

        [Display(Name = "Escolaridad")]
        public string Escolaridad { set; get; }

        public bool Activo { get; set; }

        public string NombreCompleto { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "{0} es requerido.")]
        public DateTime? FechaNac { get; set; }

        [Display(Name = "Fecha de Inscripción")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public DateTime FechaInsc { get; set; }

        //Para enviar  con fecha de inscripciónformato corto
        [Display(Name = "Fecha de Inscripción")]
        public string FechaI { get; set; }

        //[Display(Name = "Fecha de Nacimiento")]
        //[Required(ErrorMessage = "{0} es requerido.")]
        //public DateTime FechaNacEditar { get; set; }

        [Display(Name = "Sexo (F-M)")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Sexo { get; set; }

        [Display(Name = "Escuela de Procedencia")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string EscuelaP { get; set; }

        [Display(Name = "Trabajo del Padre")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Trabajo_p { get; set; }

        [Display(Name = "Trabajo de la Madre")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Trabajo_m { get; set; }

        [Display(Name = "Correo de la Madre")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Correo_m { get; set; }

        [Display(Name = "Correo de la Padre")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Correo_p { get; set; }

        [Display(Name = "Tipo de Sangre")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string TipoSangre { get; set; }

        [Display(Name = "Alergia")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Alergias { get; set; }

        [Display(Name = "Peso")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Peso { get; set; }

        [Display(Name = "Estatura")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Estatura { get; set; }

        [Display(Name = "Cuenta con alguna lesión ")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Lesion { get; set; }

        [Display(Name = "Sigue con algun tratamiento medico ")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string TratamientoM { get; set; }

        [Display(Name = "Enfermedades que padezca ")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Enfermedad { get; set; }

        [Display(Name = "Usa lentes ")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Lentes { get; set; }

        [Display(Name = "Tiene Aparato Auditivo ")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string Auditivo { get; set; }

        [Display(Name = "En caso de accidente a donde se debe trasladar el Alumno ")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string TrasladoAcc { get; set; }

        [Display(Name = "Telefono adicional de Emergencia ")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string TelEmergencia { get; set; }

        [Display(Name = "Cuenta con servicio Medico ")]
        [Required(ErrorMessage = "{0} es requerido.")]
        public string ServicioM { get; set; }

        public static implicit operator CreateAlumnoViewModel(Alumno v)
        {
            throw new NotImplementedException();
        }
    }
}