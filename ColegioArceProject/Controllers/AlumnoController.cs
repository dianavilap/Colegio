using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ColegioArceProject.AuthControllers;
using ColegioArceProject.Models;
using ColegioArceProject.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using PagedList;


namespace ColegioArceProject.Controllers
{
    [Authorize]
    public class AlumnoController : UserAuthController /*CAMBIO PRUEBA*/
    {
        private ColegioArce_Entities db = new ColegioArce_Entities();

        public ActionResult Index(String name, int pageNumber = 1, int items = 50)
        {
            List<CreateAlumnoViewModel> Alumnovm = new List<CreateAlumnoViewModel>();

            var Alumnos = db.GrupoAlumno
               .Where(x => x.Activo == true)
              .Include(x => x.Grupo)
              .Include(x => x.Alumno)
              .OrderBy(x => x.Alumno.ApellidoP)
              .ToList();


            foreach (var alumnoDb in Alumnos)
            {
                Alumnovm.Add(new CreateAlumnoViewModel
                {
                    id_Alumno = alumnoDb.id_Alumno,
                    Nombre = alumnoDb.Alumno.Nombre,
                    ApellidoP = alumnoDb.Alumno.ApellidoP,
                    ApellidoM = alumnoDb.Alumno.ApellidoM,
                    Direccion = alumnoDb.Alumno.Direccion,
                    Nom_mama = alumnoDb.Alumno.Nom_mama,
                    Nom_papa = alumnoDb.Alumno.Nom_papa,
                    Ciudad = alumnoDb.Alumno.Ciudad,
                    tel_p = alumnoDb.Alumno.tel_p,
                    tel_m = alumnoDb.Alumno.tel_m,
                    Correo = alumnoDb.Alumno.Correo,
                    GrupoDescripcion = alumnoDb.Grupo.Descripcion,
                    Activo = alumnoDb.Activo,
                    NombreCompleto = alumnoDb.Alumno.ApellidoP + " " + alumnoDb.Alumno.ApellidoM + " " + alumnoDb.Alumno.Nombre
                });
            }



            return View(Alumnovm.ToPagedList(pageNumber,items));
            

        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var alumnoDb = db.GrupoAlumno
                .Include(x => x.Alumno)
                .Include(x => x.Grupo)
                .Where(x => x.id_Alumno == id)
                .Where(x => x.Activo == true)
                .FirstOrDefault();

            if (alumnoDb == null)
            {
                return HttpNotFound();
            }
            var Alumno = new CreateAlumnoViewModel();

            Alumno.id_Alumno = alumnoDb.id_Alumno;
            Alumno.Nombre = alumnoDb.Alumno.Nombre;
            Alumno.ApellidoP = alumnoDb.Alumno.ApellidoP;
            Alumno.ApellidoM = alumnoDb.Alumno.ApellidoM;
            Alumno.Direccion = alumnoDb.Alumno.Direccion;
            Alumno.Nom_mama = alumnoDb.Alumno.Nom_mama;
            Alumno.Nom_papa = alumnoDb.Alumno.Nom_papa;
            Alumno.Ciudad = alumnoDb.Alumno.Ciudad;
            Alumno.tel_p = alumnoDb.Alumno.tel_p;
            Alumno.tel_m = alumnoDb.Alumno.tel_m;
            Alumno.Correo = alumnoDb.Alumno.Correo;
            Alumno.GrupoDescripcion = alumnoDb.Grupo.Descripcion;
            Alumno.Activo = alumnoDb.Alumno.Activo;
            Alumno.Sexo = alumnoDb.Alumno.Sexo;
            Alumno.Correo_m = alumnoDb.Alumno.Correo_m;
            Alumno.Correo_p = alumnoDb.Alumno.Correo_p;
            Alumno.TipoSangre = alumnoDb.Alumno.TipoSangre;
            Alumno.Alergias = alumnoDb.Alumno.Alergias;
            Alumno.Trabajo_m = alumnoDb.Alumno.Trabajo_m;
            Alumno.Trabajo_p = alumnoDb.Alumno.Trabajo_p;
            Alumno.EscuelaP = alumnoDb.Alumno.EscuelaP;
            Alumno.FechaInsc = alumnoDb.Alumno.FechaInsc.Date;
            Alumno.Edad = alumnoDb.Alumno.Edad;
            Alumno.tel_A = alumnoDb.Alumno.Tel_Alumno;
            Alumno.DireccionP = alumnoDb.Alumno.DireccionTP;
            Alumno.Peso = alumnoDb.Alumno.DireccionTM;
            Alumno.Estatura = alumnoDb.Alumno.DireccionTM;
            Alumno.Lesion = alumnoDb.Alumno.Lesion;
            Alumno.TratamientoM = alumnoDb.Alumno.TratamientoMed;
            Alumno.Enfermedad = alumnoDb.Alumno.Enfermedad;
            Alumno.Lentes = alumnoDb.Alumno.Lentes;
            Alumno.Auditivo = alumnoDb.Alumno.Auditivo;
            Alumno.TrasladoAcc = alumnoDb.Alumno.TrasladoAcc;
            Alumno.TelEmergencia = alumnoDb.Alumno.TelEmergencia;
            Alumno.ServicioM= alumnoDb.Alumno.ServicioMed;


            if (Alumno.FechaNac != null)
            {
                Alumno.FechaNac = alumnoDb.Alumno.FechaNac.Date;
            }
            return View(Alumno);
        }

        public ActionResult Create()
        {
            var Alumno = new CreateAlumnoViewModel();

            using (db = new ColegioArce_Entities())
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                Alumno.GrupoList = db.Grupo.ToList();
                Alumno.EscolaridadList = db.Escolaridad.ToList();
                string Hoy = DateTime.Now.ToShortDateString();
                Alumno.FechaI = DateTime.Now.ToShortDateString();/*DateTime.Parse(Hoy, MyCultureInfo);*/

                return View(Alumno);
            }
        }

        // POST: Alumnoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(CreateAlumnoViewModel Alumno)
        {
            if (ModelState.IsValid)
            {
                var alumnoDb = new Alumno();
                var GrupoA = new GrupoAlumno();

                alumnoDb.Nombre = Alumno.Nombre;
                alumnoDb.ApellidoP = Alumno.ApellidoP;
                alumnoDb.ApellidoM = Alumno.ApellidoM;
                alumnoDb.Direccion = Alumno.Direccion;
                alumnoDb.Nom_mama = Alumno.Nom_mama;
                alumnoDb.Nom_papa = Alumno.Nom_papa;
                alumnoDb.Ciudad = Alumno.Ciudad;
                alumnoDb.tel_p = Alumno.tel_p;
                alumnoDb.tel_m = Alumno.tel_m;
                alumnoDb.Correo = Alumno.Correo;            
                alumnoDb.Activo = true;
                alumnoDb.Sexo = Alumno.Sexo;
                alumnoDb.Correo_m = Alumno.Correo_m;
                alumnoDb.Correo_p = Alumno.Correo_p;
                alumnoDb.TipoSangre = Alumno.TipoSangre;
                alumnoDb.Trabajo_m = Alumno.Trabajo_m;
                alumnoDb.Trabajo_p = Alumno.Trabajo_p;
                alumnoDb.FechaNac = Alumno.FechaNac;
                alumnoDb.Alergias = Alumno.Alergias;
                alumnoDb.Edad = Alumno.Edad;
                alumnoDb.FechaInsc = DateTime.Now;
                alumnoDb.Tel_Alumno = Alumno.tel_A;
                alumnoDb.DireccionTP = Alumno.DireccionP;
                alumnoDb.DireccionTM = Alumno.DireccionM;
                alumnoDb.EscuelaP = Alumno.EscuelaP;
                alumnoDb.Lesion = Alumno.Lesion;
                alumnoDb.TratamientoMed = Alumno.TratamientoM;
                alumnoDb.Enfermedad = Alumno.Enfermedad;
                alumnoDb.Lentes =Alumno.Lentes;
                alumnoDb.Auditivo = Alumno.Auditivo;
                alumnoDb.TrasladoAcc = Alumno.TrasladoAcc;
                alumnoDb.TelEmergencia = Alumno.TelEmergencia;
                alumnoDb.ServicioMed = Alumno.ServicioM;
                alumnoDb.Peso = Alumno.Peso;
                alumnoDb.Estatura = Alumno.Estatura;

                db.Alumno.Add(alumnoDb);
                db.SaveChanges();

                var idAlumno = db.Alumno
                    .OrderByDescending(x => x.id_Alumno)
                    .Select(x => x.id_Alumno).First();

                if(idAlumno == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }


                GrupoA.id_Grupo = Alumno.id_Grupo;
                GrupoA.id_Alumno = idAlumno;
                GrupoA.Anio = DateTime.Now.Year;
                GrupoA.Activo = true;

                db.GrupoAlumno.Add(GrupoA);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            using (db = new ColegioArce_Entities())
            {
                Alumno.EscolaridadList = db.Escolaridad.ToList();
                Alumno.GrupoList = db.Grupo.ToList();
            }
            return View(Alumno);
        }

        // GET: Alumnoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var GrupoA = db.GrupoAlumno
             .Include(x => x.Alumno)
             .Include(x => x.Grupo)
             .Where(x => x.id_Alumno == id)
             .FirstOrDefault();

            if (GrupoA == null)
            {
                return HttpNotFound();
            }
            var Alumno = new CreateAlumnoViewModel();

            Alumno.id_Alumno = GrupoA.Alumno.id_Alumno;
            Alumno.Nombre = GrupoA.Alumno.Nombre;
            Alumno.ApellidoP = GrupoA.Alumno.ApellidoP;
            Alumno.ApellidoM = GrupoA.Alumno.ApellidoM;
            Alumno.Direccion = GrupoA.Alumno.Direccion;
            Alumno.Nom_mama = GrupoA.Alumno.Nom_mama;
            Alumno.Nom_papa = GrupoA.Alumno.Nom_papa;
            Alumno.Ciudad = GrupoA.Alumno.Ciudad;
            Alumno.tel_p = GrupoA.Alumno.tel_p;
            Alumno.tel_m = GrupoA.Alumno.tel_m;
            Alumno.Correo = GrupoA.Alumno.Correo;
            Alumno.id_escolaridad = GrupoA.Grupo.Escolaridad.id_escolaridad;
            Alumno.GrupoDescripcion = GrupoA.Grupo.Descripcion;
            Alumno.id_Grupo = GrupoA.id_Grupo;
            Alumno.Activo = GrupoA.Alumno.Activo;
            Alumno.Sexo = GrupoA.Alumno.Sexo;
            Alumno.Correo_m = GrupoA.Alumno.Correo_m;
            Alumno.Correo_p = GrupoA.Alumno.Correo_p;
            Alumno.TipoSangre = GrupoA.Alumno.TipoSangre;
            Alumno.Alergias = GrupoA.Alumno.Alergias;
            Alumno.Trabajo_m = GrupoA.Alumno.Trabajo_m;
            Alumno.Trabajo_p = GrupoA.Alumno.Trabajo_p;
            Alumno.FechaNacEditar = GrupoA.Alumno.FechaNac;
            Alumno.Edad = GrupoA.Alumno.Edad;
            Alumno.FechaInsc = GrupoA.Alumno.FechaInsc;
            Alumno.tel_A = GrupoA.Alumno.Tel_Alumno ;
            Alumno.DireccionP = GrupoA.Alumno.DireccionTP;
            Alumno.DireccionM = GrupoA.Alumno.DireccionTM;
            Alumno.EscuelaP = GrupoA.Alumno.EscuelaP;
            Alumno.Peso = GrupoA.Alumno.DireccionTM;
            Alumno.Estatura = GrupoA.Alumno.DireccionTM;
            Alumno.Lesion = GrupoA.Alumno.Lesion;
            Alumno.TratamientoM = GrupoA.Alumno.TratamientoMed;
            Alumno.Enfermedad = GrupoA.Alumno.Enfermedad;
            Alumno.Lentes = GrupoA.Alumno.Lentes;
            Alumno.Auditivo = GrupoA.Alumno.Auditivo;
            Alumno.TrasladoAcc = GrupoA.Alumno.TrasladoAcc;
            Alumno.TelEmergencia = GrupoA.Alumno.TelEmergencia;
            Alumno.ServicioM = GrupoA.Alumno.ServicioMed;
            

            using (db = new ColegioArce_Entities())
            {
                Alumno.GrupoList = db.Grupo.ToList();
                Alumno.EscolaridadList = db.Escolaridad.ToList();
                //ViewBag.ListaEscolaridades = new SelectList(db.Escolaridad.ToList().Select(x => new { id = x.id_escolaridad, Descripcion = x.Descripcion }), "id", "Descripcion");
               
            }

            return View(Alumno);
        }

        // POST: Alumno/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateAlumnoViewModel Alumno)
        {
            
            
                var alumnoDb = new Alumno();

                alumnoDb.id_Alumno = Alumno.id_Alumno;
                alumnoDb.Nombre = Alumno.Nombre;
                alumnoDb.ApellidoP = Alumno.ApellidoP;
                alumnoDb.ApellidoM = Alumno.ApellidoM;
                alumnoDb.Direccion = Alumno.Direccion;
                alumnoDb.Nom_mama = Alumno.Nom_mama;
                alumnoDb.Nom_papa = Alumno.Nom_papa;
                alumnoDb.Ciudad = Alumno.Ciudad;
                alumnoDb.tel_p = Alumno.tel_p;
                alumnoDb.tel_m = Alumno.tel_m;
                alumnoDb.Correo = Alumno.Correo;
                alumnoDb.Activo = true;
                alumnoDb.Sexo = Alumno.Sexo;
                alumnoDb.Correo_m = Alumno.Correo_m;
                alumnoDb.Correo_p = Alumno.Correo_p;
                alumnoDb.TipoSangre = Alumno.TipoSangre;
                alumnoDb.Trabajo_m = Alumno.Trabajo_m;
                alumnoDb.Trabajo_p = Alumno.Trabajo_p;
                alumnoDb.FechaNac = Alumno.FechaNacEditar;
                alumnoDb.Alergias = Alumno.Alergias;
                alumnoDb.Edad = Alumno.Edad;
                alumnoDb.FechaInsc = Alumno.FechaInsc;
                alumnoDb.Tel_Alumno = Alumno.tel_A;
                alumnoDb.DireccionTP = Alumno.DireccionP;
                alumnoDb.DireccionTM = Alumno.DireccionM;
            alumnoDb.EscuelaP = Alumno.EscuelaP;
            alumnoDb.Lesion = Alumno.Lesion;
            alumnoDb.TratamientoMed = Alumno.TratamientoM;
            alumnoDb.Enfermedad = Alumno.Enfermedad;
            alumnoDb.Lentes = Alumno.Lentes;
            alumnoDb.Estatura = Alumno.Estatura;
            alumnoDb.Auditivo = Alumno.Auditivo;
            alumnoDb.TrasladoAcc = Alumno.TrasladoAcc;
            alumnoDb.TelEmergencia = Alumno.TelEmergencia;
            alumnoDb.ServicioMed = Alumno.ServicioM;
            alumnoDb.Peso = Alumno.Peso;

            db.Entry(alumnoDb).State = EntityState.Modified;
                db.SaveChanges();

                var GrupoA = db.GrupoAlumno.Where(x=>x.id_Alumno == Alumno.id_Alumno).FirstOrDefault();
                    

                if (GrupoA == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Probar
                GrupoA.Grupo.id_Escolaridad = Alumno.id_escolaridad;
                GrupoA.id_Grupo = Alumno.id_Grupo;
                GrupoA.Activo = true;
                GrupoA.Anio = DateTime.Now.Year;
                
                db.Entry(GrupoA).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            
            
        }

        // GET: Alumnoes/Delete/5
        public ActionResult Delete(int? id)
        {
            var Alumno = new CreateAlumnoViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var alumnoDb = db.GrupoAlumno
                .Include(x => x.Alumno)
                .Include(x => x.Grupo)
                .FirstOrDefault(x => x.id_Alumno == id);

            if (alumnoDb == null)
            {
                return HttpNotFound();
            }

            Alumno.id_Alumno = alumnoDb.Alumno.id_Alumno;
            Alumno.Nombre = alumnoDb.Alumno.Nombre;
            Alumno.ApellidoP = alumnoDb.Alumno.ApellidoP;
            Alumno.ApellidoM = alumnoDb.Alumno.ApellidoM;
            Alumno.Direccion = alumnoDb.Alumno.Direccion;
            Alumno.Nom_mama = alumnoDb.Alumno.Nom_mama;
            Alumno.Nom_papa = alumnoDb.Alumno.Nom_papa;
            Alumno.Ciudad = alumnoDb.Alumno.Ciudad;
            Alumno.tel_p = alumnoDb.Alumno.tel_p;
            Alumno.tel_m = alumnoDb.Alumno.tel_m;
            Alumno.Correo = alumnoDb.Alumno.Correo;
            Alumno.GrupoDescripcion = alumnoDb.Grupo.Descripcion;
            Alumno.Escolaridad = alumnoDb.Grupo.Escolaridad.Descripcion;
            Alumno.Activo = alumnoDb.Alumno.Activo;
            Alumno.Sexo = alumnoDb.Alumno.Sexo;
            Alumno.Correo_m = alumnoDb.Alumno.Correo_m;
            Alumno.Correo_p = alumnoDb.Alumno.Correo_p;
            Alumno.TipoSangre = alumnoDb.Alumno.TipoSangre;
            Alumno.Trabajo_m = alumnoDb.Alumno.Trabajo_m;
            Alumno.Trabajo_p = alumnoDb.Alumno.Trabajo_p;
            Alumno.FechaNac = alumnoDb.Alumno.FechaNac;
            Alumno.Alergias = alumnoDb.Alumno.Alergias;
            Alumno.Edad = alumnoDb.Alumno.Edad;
            Alumno.FechaInsc = alumnoDb.Alumno.FechaInsc;
            Alumno.tel_A = alumnoDb.Alumno.Tel_Alumno;
            Alumno.DireccionP = alumnoDb.Alumno.DireccionTP;
            Alumno.DireccionM = alumnoDb.Alumno.DireccionTM;
            Alumno.EscuelaP = alumnoDb.Alumno.EscuelaP;
            Alumno.Peso = alumnoDb.Alumno.DireccionTM;
            Alumno.Estatura = alumnoDb.Alumno.DireccionTM;
            Alumno.Lesion = alumnoDb.Alumno.Lesion;
            Alumno.TratamientoM = alumnoDb.Alumno.TratamientoMed;
            Alumno.Enfermedad = alumnoDb.Alumno.Enfermedad;
            Alumno.Lentes = alumnoDb.Alumno.Lentes;
            Alumno.Auditivo = alumnoDb.Alumno.Auditivo;
            Alumno.TrasladoAcc = alumnoDb.Alumno.TrasladoAcc;
            Alumno.TelEmergencia = alumnoDb.Alumno.TelEmergencia;
            return View(Alumno);
        }

        // POST: Alumnoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            var GrupoA = db.GrupoAlumno
                .Include(x => x.Alumno)
                .Where(x => x.id_Alumno == id)
                .FirstOrDefault();

            GrupoA.Alumno.Activo = false;
            GrupoA.Activo = false;
            db.Entry(GrupoA).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult ExporttoExcel(string Nombre, 
                                string ApellidoP,
                                string ApellidoM,
                                DateTime FechaNac,
                                string Sexo,
                                string TipoSangre,
                                string Direccion,
                                string Nom_mama,
                                string Correo_m,
                                string Trabajo_m,
                                string Nom_papa,
                                string Correo_p,
                                string Trabajo_p,
                                string Ciudad,
                                string Correo,
                                string GrupoDescripcion)

        {
            try
            {

                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Worksheets.Add("Reporte");
                    //excel.Workbook.Worksheets.Add("Worksheet2");
                    //excel.Workbook.Worksheets.Add("Worksheet3");

                    var headerNombre = new List<string[]>() { new string[] { "Nombre: ", Nombre } };
                    string headerRange = "A1:" + Char.ConvertFromUtf32(headerNombre[0].Length + 64) + "1";
                    var headerApellidoP = new List<string[]>() { new string[] { "Apellido Paterno: ", ApellidoP } };
                    string headerRangeAP = "A2:" + Char.ConvertFromUtf32(headerApellidoP[0].Length + 64) + "2";
                    var headerApellidoM = new List<string[]>() { new string[] { "Apellido Materno: ", ApellidoM } };
                    string headerRangeAM = "A3:" + Char.ConvertFromUtf32(headerApellidoM[0].Length + 64) + "3";
                    var headerFechaNac = new List<string[]>() { new string[] { "Fecha de Nacimiento: ", FechaNac.ToString() } };
                    string headerRangeFec = "A4:" + Char.ConvertFromUtf32(headerFechaNac[0].Length + 64) + "4";
                    var headerSexo = new List<string[]>() { new string[] { "Sexo: ",Sexo } };
                    string headerRangeSex = "A5:" + Char.ConvertFromUtf32(headerSexo[0].Length + 64) + "5";
                    var headerTipoSangre = new List<string[]>() { new string[] { "Tipo de Sangre: ",TipoSangre } };
                    string headerRangeSan = "A6:" + Char.ConvertFromUtf32(headerTipoSangre[0].Length + 64) + "6";
                    var headerDireccion = new List<string[]>() { new string[] { "Dirección: " ,Direccion} };
                    string headerRangeDir = "A7:" + Char.ConvertFromUtf32(headerDireccion[0].Length + 64) + "7";
                    var headerNom_mama = new List<string[]>() { new string[] { "Nombre de la Madre: ",Nom_mama } };
                    string headerRangeNM = "A8:" + Char.ConvertFromUtf32(headerNom_mama[0].Length + 64) + "8";
                    var headerCorreo_m = new List<string[]>() { new string[] { "Correo de la Madre: ",Correo_m } };
                    string headerRangeCorreom = "A9:" + Char.ConvertFromUtf32(headerCorreo_m[0].Length + 64) + "9";
                    var headerTrabajo_m = new List<string[]>() { new string[] { "Trabajo de la Madre: ",Trabajo_m } };
                    string headerRangeTM = "A10:" + Char.ConvertFromUtf32(headerTrabajo_m[0].Length + 64) + "10";
                    var headerNom_papa = new List<string[]>() { new string[] { "Nombre del Padre: ",Nom_papa } };
                    string headerRangeNP = "A11:" + Char.ConvertFromUtf32(headerNom_papa[0].Length + 64) + "11";
                    var headerCorreo_p = new List<string[]>() { new string[] { "Correo del Padre: " ,Correo_p} };
                    string headerRangeCP = "A12:" + Char.ConvertFromUtf32(headerCorreo_p[0].Length + 64) + "12";
                    var headerTrabajo_p = new List<string[]>() { new string[] { "Trabajo del Padre: ",Trabajo_p } };
                    string headerRangeTP = "A13:" + Char.ConvertFromUtf32(headerTrabajo_p[0].Length + 64) + "13";
                    var headerCiudad = new List<string[]>() { new string[] { "Ciudad: " ,Ciudad} };
                    string headerRangeC = "A14:" + Char.ConvertFromUtf32(headerCiudad[0].Length + 64) + "14";
                    var headerCorreo = new List<string[]>() { new string[] { "Correo: ",Correo } };
                    string headerRangeMail = "A15:" + Char.ConvertFromUtf32(headerCorreo[0].Length + 64) + "15";
                    var headerGrupo = new List<string[]>() { new string[] { "Grupo: ",GrupoDescripcion } };
                    string headerRangeGrupo = "A16:" + Char.ConvertFromUtf32(headerGrupo[0].Length + 64) + "16";


                    // Target a worksheet
                    var worksheet = excel.Workbook.Worksheets["Reporte"];

                  

                    // Popular header row data
                    worksheet.Cells[headerRange].LoadFromArrays(headerNombre);
                    worksheet.Cells[headerRangeAP].LoadFromArrays(headerApellidoP);
                    worksheet.Cells[headerRangeAM].LoadFromArrays(headerApellidoM);
                    worksheet.Cells[headerRangeFec].LoadFromArrays(headerFechaNac);
                    worksheet.Cells[headerRangeSex].LoadFromArrays(headerSexo);
                    worksheet.Cells[headerRangeSan].LoadFromArrays(headerTipoSangre);
                    worksheet.Cells[headerRangeDir].LoadFromArrays(headerDireccion);
                    worksheet.Cells[headerRangeNM].LoadFromArrays(headerNom_mama);
                    worksheet.Cells[headerRangeCorreom].LoadFromArrays(headerCorreo_m);
                    worksheet.Cells[headerRangeTM].LoadFromArrays(headerTrabajo_m);
                    worksheet.Cells[headerRangeNP].LoadFromArrays(headerNom_papa);
                    worksheet.Cells[headerRangeCP].LoadFromArrays(headerCorreo_p);
                    worksheet.Cells[headerRangeTP].LoadFromArrays(headerTrabajo_p);
                    worksheet.Cells[headerRangeC].LoadFromArrays(headerCiudad);
                    worksheet.Cells[headerRangeMail].LoadFromArrays(headerCorreo);
                    worksheet.Cells[headerRangeGrupo].LoadFromArrays(headerGrupo);


                    FileInfo excelFile = new FileInfo(@"C:\temp\Informacion de " + ApellidoP+" "+ApellidoM+" "+Nombre+ ".xlsx");
                    excel.SaveAs(excelFile);
                }

                var Reportevm = new CreateAlumnoViewModel();

                return Json(Reportevm);
            }
            catch (Exception Ex)
            {
                return Json(""+Ex.Message);

            }

        }

        public PartialViewResult BuscarAlumno()
        {
            var Alumno = new CreateAlumnoViewModel();

            using (db = new ColegioArce_Entities())
            {
                Alumno.GrupoList = db.Grupo.ToList();
                Alumno.EscolaridadList = db.Escolaridad.ToList();
            }
            ViewBag.Escolaridad = new SelectList(db.Escolaridad, "id_escolaridad", "Escolaridad");
            return PartialView("BuscarAlumno", Alumno);
        }

        [HttpPost]
        public JsonResult BuscarAlumno(int id_Grupo, int id_escolaridad, string Nombre, string ApellidoP, string ApellidoM)
        {
            List<CreateAlumnoViewModel> Alumnovm = new List<CreateAlumnoViewModel>();

            
            var Alumnos = db.GrupoAlumno
                .Include(x=>x.Alumno)
                .Include(x => x.Grupo)
                .Where(x => x.Alumno.Activo == true
            && x.id_Grupo == id_Grupo
            && x.Grupo.Escolaridad.id_escolaridad == id_escolaridad
            && ((x.Alumno.Nombre == null) || (x.Alumno.Nombre.Contains(Nombre)))
            && ((x.Alumno.ApellidoP == null) || (x.Alumno.ApellidoP.Contains(ApellidoP)))
            && ((x.Alumno.ApellidoM == null) || (x.Alumno.ApellidoM.Contains(ApellidoM))))
           .OrderBy(x => x.Alumno.ApellidoP)
           .ToList();


            foreach (var alumnoDb in Alumnos)
            {
                Alumnovm.Add(new CreateAlumnoViewModel
                {
                    id_Alumno = alumnoDb.Alumno.id_Alumno,
                    Nombre = alumnoDb.Alumno.Nombre,
                    ApellidoP = alumnoDb.Alumno.ApellidoP,
                    ApellidoM = alumnoDb.Alumno.ApellidoM,
                    Direccion = alumnoDb.Alumno.Direccion,
                    Nom_mama = alumnoDb.Alumno.Nom_mama,
                    Nom_papa = alumnoDb.Alumno.Nom_papa,
                    Ciudad = alumnoDb.Alumno.Ciudad,
                    tel_p = alumnoDb.Alumno.tel_p,
                    tel_m = alumnoDb.Alumno.tel_m,
                    Correo = alumnoDb.Alumno.Correo,
                    GrupoDescripcion = alumnoDb.Grupo.Descripcion,
                    Activo = alumnoDb.Alumno.Activo,
                    NombreCompleto = alumnoDb.Alumno.Nombre + " " + alumnoDb.Alumno.ApellidoP + " " + alumnoDb.Alumno.ApellidoM
                });
            }
           

            return Json(Alumnovm);
        }

        [HttpPost]
        public JsonResult GrupoList(int id)
        {
            var Listaescolaridad = db.Escolaridad.ToList();
            var ListaGrupo = db.Grupo.Where(x => x.id_Escolaridad == id).ToList();
            List<SelectListItem> Grupos = new List<SelectListItem>();

            Grupos.Add(new SelectListItem { Text = "--- Seleccionar ---", Value = "0" });
            foreach (var item in ListaGrupo)
            {
                Grupos.Add(new SelectListItem { Text = item.Descripcion, Value = item.id_Grupo.ToString() });
            }

            return Json(new SelectList(Grupos, "Value", "Text"));
        }

        public ActionResult Imprimir(int id)
        {

            var alumno = db.Alumno
                 .Include(x => x.GrupoAlumno)
                 .SingleOrDefault(x => x.id_Alumno == id);

            var memoryStream = new MemoryStream();
            CrearSolicitudInscripcion(memoryStream, alumno);
            return File(memoryStream.GetBuffer(), "application/pdf", "Solicitud de Inscripción - " + alumno.id_Alumno + ".pdf");
        }

        public void CrearSolicitudInscripcion(Stream output, Alumno alumno)
        {
            var documento = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(documento, output);
            documento.Open();
            var fontTitle = new Font(Font.FontFamily.HELVETICA, 26, 2);
            var fontParagraph = new Font(Font.FontFamily.HELVETICA, 11, 2);
            var fontBlack = new Font(Font.FontFamily.HELVETICA, 11, 1);

            var grupo = db.GrupoAlumno
                .Include(x => x.Alumno)
                .Where(x => x.id_Alumno == alumno.id_Alumno).FirstOrDefault();

            Paragraph title = new Paragraph(" COLEGIO ARCE ", fontBlack);
            Paragraph Solicitud = new Paragraph(" Solicitud de Inscripción ", fontBlack);
            Paragraph Fecha = new Paragraph(" Fecha de Inscripcion : "+alumno.FechaInsc, fontParagraph);
            Paragraph Nombre = new Paragraph(" Nombre:  "+alumno.ApellidoP+" "+alumno.ApellidoM+" "+alumno.Nombre + "     Sexo : "+alumno.Sexo, fontParagraph);
            Paragraph FechaEdad = new Paragraph(" Fecha de Nacimiento : "+alumno.FechaNac.ToString("dd/MM/yyyy") +"       Edad : "+ alumno.Edad, fontParagraph);
            Paragraph DireccionTel = new Paragraph(" Dirección : "+alumno.Direccion+"       Telefono : "+alumno.Tel_Alumno, fontParagraph);
            Paragraph EscuelaP = new Paragraph(" Escuela de Procedencia : "+ alumno.EscuelaP, fontParagraph);
            Paragraph Correo = new Paragraph(" Email : "+alumno.Correo, fontParagraph);
            Paragraph Grupo = new Paragraph(" Grado en el que se inscribe : "+ grupo.Grupo.Descripcion,fontParagraph);
            Paragraph Datos = new Paragraph("  DATOS PERSONALES DE LOS PADRES  ", fontBlack);
            Paragraph NombrePadre = new Paragraph("  Nombre del Padre:  "+alumno.Nom_papa, fontParagraph);
            Paragraph TrabajoP = new Paragraph("  Lugar de trabajo  "+alumno.Trabajo_p, fontParagraph);
            Paragraph DireccionTP = new Paragraph("  Direccion  "+alumno.DireccionTP, fontParagraph);
            Paragraph CorreoPTel = new Paragraph("  Email Papá:   "+alumno.Correo_p+"       Telefono: "+alumno.tel_p, fontParagraph);
            Paragraph NombreMadre = new Paragraph("  Nombre de la Madré:  "+alumno.Nom_mama, fontParagraph);
            Paragraph TrabajoM = new Paragraph("  Lugar de trabajo  " + alumno.Trabajo_m, fontParagraph);
            Paragraph DireccionTM = new Paragraph("  Direccion  "+alumno.DireccionTM, fontParagraph);
            Paragraph CorreoMTel = new Paragraph("  Email Mamá:   " + alumno.Correo_m + "       Telefono: " + alumno.tel_m, fontParagraph);




            Paragraph Documentacion = new Paragraph("DOCUMENTACION ENTREGADA ", fontBlack);
            Paragraph Acta = new Paragraph("ACTA DE NACIMIENTO                        ORIGINAL(    )                     COPIA(    )\n" +
                                           "CERTIFICADO DE SEC.                       ORIGINAL(    )                     COPIA(    )\n" +
                                           "CURP               				                                     ORIGINAL(    )		                      COPIA(	  )\n" +
                                           "KARDEX     	                                          ORIGINAL(    )	             	          COPIA(    )\n" +
                                           "CERT. PARCIAL	     		                             ORIGINAL(    )		                       COPIA(    )\n\n\n" +
                                           "NOTA: LOS PAGOS EFECTUADOS POR INSCRIPCION Y MESES CORRIENDO NO SERAN REEMBOLSADOS EN CASO DE BAJA DEL ALUMNO.\n\n\n\n" +
                                           "_______________________________________                _____________________________________\n" +
                                           "    NOMBRE  Y FIRMA DEL PADRE O TUTOR			                       NOMBRE Y FIRMA DEL ALUMNO", fontParagraph);
            //Paragraph Certificado = new Paragraph("CERTIFICADO DE SEC.                       ORIGINAL( )                        COPIA( )", fontParagraph);


            title.Alignment = Element.ALIGN_CENTER;
            Solicitud.Alignment = Element.ALIGN_CENTER;
            Fecha.Alignment = Element.ALIGN_RIGHT;
            Datos.Alignment = Element.ALIGN_CENTER;
            Documentacion.Alignment = Element.ALIGN_CENTER;

            documento.Add(title);
            documento.Add(Solicitud);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Fecha);   
            documento.Add(Nombre);           
            documento.Add(FechaEdad);           
            documento.Add(DireccionTel);           
            documento.Add(EscuelaP);           
            documento.Add(Correo);           
            documento.Add(Grupo);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Datos);
            documento.Add(Chunk.NEWLINE);
            documento.Add(NombrePadre);           
            documento.Add(TrabajoP);           
            documento.Add(DireccionTP);           
            documento.Add(CorreoPTel);           
            documento.Add(NombreMadre);           
            documento.Add(TrabajoM);           
            documento.Add(DireccionTM);           
            documento.Add(CorreoMTel);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Documentacion);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Acta);

            Paragraph Historial = new Paragraph(" HISTORIAL MEDICO DEL ALUMNO ", fontBlack);
            Paragraph sangre = new Paragraph(" Tipo de Sangre :  "+ alumno.TipoSangre +"         Peso : "+ alumno.Peso, fontParagraph);
            Paragraph EstaturaS = new Paragraph(" Estatura :  " + alumno.Estatura+"                 Servicio Medico : "+ alumno.ServicioMed, fontParagraph);
            Paragraph Lesion = new Paragraph(" Cuenta con alguna lesion :  " + alumno.Lesion, fontParagraph);
            Paragraph Tratamiento = new Paragraph(" Sigue algún tratamiento Medico :  " + alumno.Lesion, fontParagraph);
            Paragraph Enfermedades = new Paragraph(" Enfermedades que padezca :  " + alumno.Enfermedad, fontParagraph);
            Paragraph Lentes = new Paragraph(" Usa lentes :  " + alumno.Lentes + "                   Aparato auditivo: " +alumno.Auditivo, fontParagraph);
            Paragraph Accidente = new Paragraph(" En caso de accidente a donde se debe trasladar :  " + alumno.TrasladoAcc, fontParagraph);
            Paragraph tel = new Paragraph(" Numero adicional al que llamar en caso de emergencia :  " + alumno.Enfermedad, fontParagraph);

            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Historial);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(sangre);
            documento.Add(EstaturaS);
            documento.Add(Lesion);
            documento.Add(Tratamiento);
            documento.Add(Enfermedades);
            documento.Add(Lentes);
            documento.Add(Accidente);
            documento.Add(tel);
            documento.Add(Chunk.NEWLINE);

            Historial.Alignment = Element.ALIGN_CENTER;

            documento.Close();
            writer.Close();
        }

    }
}
