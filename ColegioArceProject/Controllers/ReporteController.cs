using ColegioArceProject.AuthControllers;
using ColegioArceProject.Models;
using ColegioArceProject.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ColegioArceProject.Controllers
{
    [Authorize]
    public class ReporteController : UserAuthController
    {
        private ColegioArce_Entities db = new ColegioArce_Entities();

        public ActionResult Alumno()
        {
            //Buscar los años de los pagos, para que muestre los años que tenemos pagos solamente
            //var ReporteVM = new ReporteViewModel();
            //var ListaAnos = new List<int>();
            //List<DateTime> fechaPago = db.Pago.Where(x => x.Fecha.Year != 0).Select(x => x.Fecha).ToList();


            //foreach (var item in fechaPago)
            //{
            //        var year = item.Year;
            //        ListaAnos.Add(year);

            //}
            var reporte = new ReporteAlumnoViewModel();
            reporte.ListaPagos = new List<ReporteViewModel>();

            return View(reporte);
        }

        [ChildActionOnly]
        public PartialViewResult _BuscarAlumno()
        {
            var Reporte = new ReporteAlumnoViewModel();
            Reporte.ListaPagos = new List<ReporteViewModel>();
            return PartialView(Reporte);
        }

        [HttpPost]
        public JsonResult _BuscarAlumno(string Alumno)
        {
            var Pagos = new List<Pago>();
            var Reportevm = new List<ReporteViewModel>();
            var ReporteAlumno = new ReporteAlumnoViewModel();

            var alumnobusqueda = db.Alumno
                .Where(x => x.Activo == true)
                .Select(x => new { NombreCompleto = x.ApellidoP + " " + x.ApellidoM + " " + x.Nombre, id = x.id_Alumno })
                .Distinct().ToList();

            var AlumnoPagos = alumnobusqueda.Where(x => x.NombreCompleto.Equals(Alumno)).Select(x => new { idAlumno = x.id }).FirstOrDefault();


            Pagos = db.Pago
                .Include(x => x.GrupoAlumno)
                .Include(x => x.GrupoAlumno.Grupo)
                .Include(x => x.GrupoAlumno.Alumno)
                .Include(x => x.Precio)
                .Include(x => x.Abono1)
                .Where(x => x.GrupoAlumno.id_Alumno == AlumnoPagos.idAlumno)
                .Where(x => x.Activo == true)
                .ToList();


            List<Abono> Listabono = null;
            foreach (var Pagodb in Pagos)
            {
                if (Pagodb.Abono == true)
                {
                    Listabono = db.Abono.Where(x => x.id_Pago == Pagodb.id_pago).Where(x => x.Cancelado == false).ToList();
                    foreach (Abono abono in Listabono)
                    {
                        Reportevm.Add(new ReporteViewModel
                        {
                            Fecha = Pagodb.Fecha.ToString("dd/MM/yyyy"),
                            Folio = Pagodb.id_pago.ToString().Length >= 2 ? "000" + Pagodb.Folio : "0000" + Pagodb.Folio,
                            //DescripcionEscolaridad = pago.Alumno.Grupo.Escolaridad.Descripcion,
                            ConceptoPago = abono.Cancelado == true ? "Folio CANCELADO " + Pagodb.DescripcionPago : "Abono de " + Pagodb.DescripcionPago,
                            TipoPago = abono.TipoPago,
                            Importe = abono.Importe
                        });
                    }
                }
                else
                {
                    Reportevm.Add(new ReporteViewModel
                    {
                        Fecha = Pagodb.Fecha.ToString("dd/MM/yyyy"),
                        Folio = Pagodb.id_pago.ToString().Length >= 2 ? "000" + Pagodb.Folio : "0000" + Pagodb.Folio,
                        ConceptoPago = Pagodb.Activo == true ? Pagodb.DescripcionPago : "Folio CANCELADO " + Pagodb.DescripcionPago,
                        TipoPago = Pagodb.TipoPago,
                        Importe = Pagodb.Importe
                    });
                }
            }
            var datos = Pagos
                 .Where(x => x.GrupoAlumno.id_Alumno == AlumnoPagos.idAlumno)
                 .Select(x => new { Grupo = x.GrupoAlumno.Grupo.Descripcion, Escolaridad = x.GrupoAlumno.Grupo.Escolaridad.Descripcion }).FirstOrDefault();
            ReporteAlumno.Alumno = Alumno;
            ReporteAlumno.Escolaridad = datos.Escolaridad;
            ReporteAlumno.Grupo = datos.Grupo;
            ReporteAlumno.NombreCompleto = Alumno;
            ReporteAlumno.ListaPagos = Reportevm;


            return Json(ReporteAlumno);
        }


        public ActionResult Adeudo(int? id)
        {

            var model = new AdeudoViewModel { };

            if (id.HasValue)
            {
                var alumnos = db.GrupoAlumno
                    .Include(x => x.Grupo)
                    .Include(x => x.Alumno)
                    .Include(x => x.Pago)
                    .Where(x => x.Grupo.id_Grupo == id)
                    .Where(x => x.Activo == true)
                    .OrderBy(x=>x.Alumno.ApellidoP)
                    .ToList();

                var grupo = db.Grupo.SingleOrDefault(x => x.id_Grupo == id);

                model.Grupo = grupo.Descripcion;

                model.Alumnos = alumnos;
                ViewBag.id_Grupo = new SelectList(db.Grupo, "id_Grupo", "Descripcion", id);

                return View(model);
            }

            ViewBag.id_Grupo = new SelectList(db.Grupo, "id_Grupo", "Descripcion");

            return View(model);
        }

        [HttpPost]
        public ActionResult Adeudo(AdeudoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return Redirect("/Reporte/Adeudo/" + model.id_Grupo);
        }


        [ChildActionOnly]
        public PartialViewResult BuscarFecha()
        {
            var reporte = new ReporteViewModel();

            return PartialView("_BuscarFecha", reporte);
        }

        [HttpPost]
        public JsonResult BuscarFecha(DateTime FechaInicio, DateTime FechaFin, string Alumno)
        {
            var Pagos = new List<Pago>();
            var Reportevm = new List<ReporteViewModel>();


            var alumnobusqueda = db.Alumno
                              .Select(x => new { NombreCompleto = x.ApellidoP + " " + x.ApellidoM + " " + x.Nombre, id = x.id_Alumno })
                             .Distinct().ToList();

            var AlumnoPagos = alumnobusqueda.Where(x => x.NombreCompleto.Equals(Alumno)).Select(x => new { idAlumno = x.id }).FirstOrDefault();

            if (Alumno == "")
            {

                Pagos = db.Pago
                       .Include(x => x.GrupoAlumno.Grupo)
                       .Include(x => x.GrupoAlumno.Alumno)
                       .Include(x => x.Precio)
                       .Include(x => x.Abono1)
                       //.Where(x => x.Activo == true)
                       .Where(x => x.Fecha >= FechaInicio && x.Fecha <= FechaFin)
                       .ToList();

            }
            else
            {

                Pagos = db.Pago
                    .Include(x => x.GrupoAlumno)
                    .Include(x => x.GrupoAlumno.Grupo)
                    .Include(x => x.GrupoAlumno.Alumno)
                    .Include(x => x.Precio)
                    .Include(x => x.Abono1)
                    //.Where(x => x.Activo == true)
                    .Where(x => x.GrupoAlumno.id_Alumno == AlumnoPagos.idAlumno && x.Fecha >= FechaInicio && x.Fecha <= FechaFin)
                    .ToList();
            }

            List<Abono> Listabono = null;
            foreach (var Pagodb in Pagos)
            {
                if (Pagodb.Abono == true)
                {
                    Listabono = db.Abono.Where(x => x.id_Pago == Pagodb.id_pago).ToList();
                    foreach (Abono abono in Listabono)
                    {
                        Reportevm.Add(new ReporteViewModel
                        {
                            Fecha = Pagodb.Fecha.ToString("dd/MM/yyyy"),
                            Folio = abono.Folio.ToString(),
                            NombreCompleto = Pagodb.GrupoAlumno.Alumno.ApellidoP + " " + Pagodb.GrupoAlumno.Alumno.ApellidoM + " " + Pagodb.GrupoAlumno.Alumno.Nombre,
                            //DescripcionEscolaridad = pago.Alumno.Grupo.Escolaridad.Descripcion,
                            ConceptoPago = abono.Cancelado == true ? "Folio CANCELADO " + Pagodb.DescripcionPago : "Abono de " + Pagodb.DescripcionPago,
                            TipoPago = Pagodb.TipoPago,
                            Importe = abono.Importe
                        });
                    }
                }
                else
                {
                    Reportevm.Add(new ReporteViewModel
                    {
                        Fecha = Pagodb.Fecha.ToString("dd/MM/yyyy"),
                        Folio = Pagodb.Folio.ToString(),
                        NombreCompleto = Pagodb.GrupoAlumno.Alumno.ApellidoP + " " + Pagodb.GrupoAlumno.Alumno.ApellidoM + " " + Pagodb.GrupoAlumno.Alumno.Nombre,
                        ConceptoPago = Pagodb.Activo == true ? Pagodb.DescripcionPago : "Folio CANCELADO " + Pagodb.DescripcionPago,
                        TipoPago = Pagodb.TipoPago,
                        Importe = Pagodb.Importe
                    });
                }
            }
            return Json(Reportevm);
        }



        [HttpGet]
        public JsonResult GetAlumnos(string Areas, string buscar = "")
        {
            //var alumnoslist = db.Alumno
            //                .Where(x => x.Nombre.ToUpper().Contains(buscar.ToUpper()) || x.ApellidoP.ToUpper().Contains(buscar.ToUpper()) || x.ApellidoM.ToUpper().Contains(buscar.ToUpper()))
            //                .Select(x => new { Alumno = x.Nombre + " " + x.ApellidoP + " " + x.ApellidoM, id = x.id_Alumno })
            //                .Distinct().ToList();


            var alumnobusqueda = db.Alumno
                             .Select(x => new { Alumno = x.ApellidoP + " " + x.ApellidoM + " " + x.Nombre, id = x.id_Alumno })
                             .Distinct().ToList();

            alumnobusqueda = alumnobusqueda.Where(x => x.Alumno.StartsWith(buscar, StringComparison.OrdinalIgnoreCase)).ToList();

            return Json(alumnobusqueda, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Index()
        {
            //Buscar los años de los pagos, para que muestre los años que tenemos pagos solamente
            //var ReporteVM = new ReporteViewModel();
            //var ListaAnos = new List<int>();
            //List<DateTime> fechaPago = db.Pago.Where(x => x.Fecha.Year != 0).Select(x => x.Fecha).ToList();


            //foreach (var item in fechaPago)
            //{
            //        var year = item.Year;
            //        ListaAnos.Add(year);

            //}
            var reporte = new List<ReporteViewModel>();


            return View(reporte);
        }

        public JsonResult ExporttoExcel(DateTime FechaInicio, DateTime FechaFin, string Alumno)

        {
            try
            {
                var Pagos = new List<Pago>();
                var Reportevm = new List<ReporteViewModel>();
                string NombreExcel;

                var alumnobusqueda = db.Alumno
                                  .Select(x => new { NombreCompleto = x.ApellidoP + " " + x.ApellidoM + " " + x.Nombre, id = x.id_Alumno })
                                 .Distinct().ToList();

                var AlumnoPagos = alumnobusqueda.Where(x => x.NombreCompleto.Equals(Alumno)).Select(x => new { idAlumno = x.id }).FirstOrDefault();

                if (Alumno == "")
                {

                    Pagos = db.Pago
                           .Include(x => x.GrupoAlumno.Grupo.Escolaridad)
                           .Include(x => x.Precio)
                           .Include(x => x.Abono1)
                           .Where(x => x.Activo == true)
                           .Where(x => x.Fecha >= FechaInicio && x.Fecha <= FechaFin && x.Activo == true)
                           .ToList();

                    NombreExcel = FechaInicio.ToString("dd-MM-yyyy") + " al " + FechaFin.ToString("dd-MM-yyyy");
                }
                else
                {

                    Pagos = db.Pago
                        .Include(x => x.GrupoAlumno)
                        .Include(x => x.GrupoAlumno.Grupo)
                        .Include(x => x.GrupoAlumno.Alumno)
                        .Include(x => x.Precio)
                        .Include(x => x.Abono1)
                        .Where(x => x.Activo == true)
                        .Where(x => x.GrupoAlumno.id_Alumno == AlumnoPagos.idAlumno)
                        .ToList();

                    NombreExcel = Alumno + " " + FechaInicio.ToString("dd-MM-yyyy") + " al " + FechaFin.ToString("dd-MM-yyyy");
                }


                List<Abono> Listabono = null;
                foreach (var Pagodb in Pagos)
                {
                    if (Pagodb.Abono != null)
                    {
                        Listabono = db.Abono.Where(x => x.id_Pago == Pagodb.id_pago).Where(x => x.Cancelado == false).ToList();
                        foreach (Abono abono in Listabono)
                        {
                            Reportevm.Add(new ReporteViewModel
                            {
                                Fecha = Pagodb.Fecha.ToString("dd/MM/yyyy"),
                                Folio = Pagodb.id_pago.ToString().Length >= 2 ? "000" + Pagodb.id_pago : "0000" + Pagodb.id_pago,
                                NombreCompleto = Pagodb.GrupoAlumno.Alumno.ApellidoP + " " + Pagodb.GrupoAlumno.Alumno.ApellidoM + " " + Pagodb.GrupoAlumno.Alumno.Nombre,
                                //DescripcionEscolaridad = pago.Alumno.Grupo.Escolaridad.Descripcion,
                                ConceptoPago = "Abono de " + Pagodb.DescripcionPago,
                                TipoPago = Pagodb.TipoPago,
                                Importe = abono.Importe
                            });
                        }
                    }
                    Reportevm.Add(new ReporteViewModel
                    {
                        Fecha = Pagodb.Fecha.ToShortDateString(),
                        Folio = Pagodb.id_pago.ToString().Length >= 2 ? "000" + Pagodb.id_pago : "0000" + Pagodb.id_pago,
                        NombreCompleto = Pagodb.GrupoAlumno.Alumno.ApellidoP + " " + Pagodb.GrupoAlumno.Alumno.ApellidoM + " " + Pagodb.GrupoAlumno.Alumno.Nombre,
                        ConceptoPago = Pagodb.DescripcionPago,
                        TipoPago = Pagodb.TipoPago,
                        Importe = Pagodb.Importe
                    });
                }

                var Excelvm = new List<ExcelViewModel>();

                foreach (var R in Reportevm)
                {
                    Excelvm.Add(new ExcelViewModel
                    {
                        Fecha = R.Fecha,
                        Folio = R.Folio,
                        NombreCompleto = R.NombreCompleto,
                        ConceptoPago = R.ConceptoPago,
                        TipoPago = R.TipoPago,
                        Importe = R.Importe.ToString()
                    });
                }

                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Worksheets.Add("Reporte");
                    //excel.Workbook.Worksheets.Add("Worksheet2");
                    //excel.Workbook.Worksheets.Add("Worksheet3");

                    var headerRow = new List<string[]>()
                          {
                            new string[] { "Fecha", "Folio", "Nombre Completo", "Concepto de Pago","Metodo de Pago", "Importe" }
                          };

                    // Determine the header range (e.g. A1:E1)
                    string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

                    // Target a worksheet
                    var worksheet = excel.Workbook.Worksheets["Reporte"];

                    var Content = new List<string[]>();

                    foreach (var row in Excelvm)
                    {
                        Content.Add(new string[]
                        {
                            row.Fecha, row.Folio, row.NombreCompleto, row.ConceptoPago, row.TipoPago, row.Importe
                        });
                    }
                    string headerContent = "A2:" + Char.ConvertFromUtf32(Content[0].Length + 64) + "2";
                    // Popular header row data
                    worksheet.Cells[headerRange].LoadFromArrays(headerRow);
                    worksheet.Cells[headerContent].LoadFromArrays(Content);

                    var fecha = DateTime.Now.ToShortDateString().ToString();

                    FileInfo excelFile = new FileInfo(@"C:\temp\Reporte de Pagos " + NombreExcel + ".xlsx");
                    excel.SaveAs(excelFile);
                }

                return Json(Reportevm);
            }
            catch (Exception Ex)
            {
                return Json("");

            }

        }

        [HttpPost]
        public JsonResult ExportAlumno(string Alumno)
        {
            try
            {
                var Pagos = new List<Pago>();
                var Reportevm = new List<ReporteViewModel>();
                var ReporteAlumno = new ReporteAlumnoViewModel();


                var alumnobusqueda = db.Alumno
                    .Select(x => new { NombreCompleto = x.ApellidoP + " " + x.ApellidoM + " " + x.Nombre, id = x.id_Alumno })
                    .Distinct().ToList();

                var AlumnoPagos = alumnobusqueda.Where(x => x.NombreCompleto.Equals(Alumno)).Select(x => new { idAlumno = x.id }).FirstOrDefault();

                Pagos = db.Pago
                    .Include(x => x.GrupoAlumno)
                    .Include(x => x.GrupoAlumno.Grupo)
                    .Include(x => x.GrupoAlumno.Alumno)
                    .Include(x => x.Precio)
                    .Include(x => x.Abono1)
                    .Where(x => x.Activo == true)
                    .Where(x => x.GrupoAlumno.id_Alumno == AlumnoPagos.idAlumno)
                    .ToList();


                List<Abono> Listabono = null;
                foreach (var Pagodb in Pagos)
                {
                    if (Pagodb.Abono != null)
                    {
                        Listabono = db.Abono.Where(x => x.id_Pago == Pagodb.id_pago).Where(x => x.Cancelado == false).ToList();
                        foreach (Abono abono in Listabono)
                        {
                            Reportevm.Add(new ReporteViewModel
                            {
                                Fecha = Pagodb.Fecha.ToString("dd/MM/yyyy"),
                                Folio = Pagodb.id_pago.ToString().Length >= 2 ? "000" + Pagodb.id_pago : "0000" + Pagodb.id_pago,
                                //DescripcionEscolaridad = pago.Alumno.Grupo.Escolaridad.Descripcion,
                                ConceptoPago = "Abono de " + Pagodb.DescripcionPago,
                                TipoPago = Pagodb.TipoPago,
                                Importe = abono.Importe
                            });
                        }
                    }
                    Reportevm.Add(new ReporteViewModel
                    {
                        Fecha = Pagodb.Fecha.ToShortDateString(),
                        Folio = Pagodb.id_pago.ToString().Length >= 2 ? "000" + Pagodb.id_pago : "0000" + Pagodb.id_pago,
                        ConceptoPago = Pagodb.DescripcionPago,
                        TipoPago = Pagodb.TipoPago,
                        Importe = Pagodb.Importe
                    });
                }
                var datos = Pagos
                     .Where(x => x.GrupoAlumno.id_Alumno == AlumnoPagos.idAlumno)
                     .Select(x => new { Grupo = x.GrupoAlumno.Grupo.Descripcion, Escolaridad = x.GrupoAlumno.Grupo.Escolaridad.Descripcion }).FirstOrDefault();
                ReporteAlumno.Alumno = Alumno;
                ReporteAlumno.Escolaridad = datos.Escolaridad;
                ReporteAlumno.Grupo = datos.Grupo;
                ReporteAlumno.NombreCompleto = Alumno;
                ReporteAlumno.ListaPagos = Reportevm;

                var Excelvm = new List<ExcelViewModel>();

                foreach (var R in Reportevm)
                {
                    Excelvm.Add(new ExcelViewModel
                    {
                        Fecha = R.Fecha,
                        Folio = R.Folio,
                        NombreCompleto = R.NombreCompleto,
                        ConceptoPago = R.ConceptoPago,
                        TipoPago = R.TipoPago,
                        Importe = R.Importe.ToString()
                    });
                }

                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Worksheets.Add("Reporte");
                    //excel.Workbook.Worksheets.Add("Worksheet2");
                    //excel.Workbook.Worksheets.Add("Worksheet3");

                    var headerRow = new List<string[]>()
                                  {
                                    new string[] { "Fecha", "Folio", "Concepto de Pago", "Metodo de Pago", "Importe" }
                                  };

                    // Determine the header range (e.g. A1:E1)
                    string headerRange = "A4:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "4";

                    // Target a worksheet
                    var worksheet = excel.Workbook.Worksheets["Reporte"];

                    var Content = new List<string[]>();
                    var alum = new List<string[]>();
                    alum.Add(new string[] { Alumno });
                    var esc = new List<string[]>();
                    esc.Add(new string[] { datos.Escolaridad });
                    var grup = new List<string[]>();
                    grup.Add(new string[] { datos.Grupo });
                    foreach (var row in Excelvm)
                    {
                        Content.Add(new string[]
                        {
                                    row.Fecha, row.Folio, row.ConceptoPago,row.TipoPago, row.Importe
                        });
                    }
                    string headerContent = "A5:" + Char.ConvertFromUtf32(Content[0].Length + 64) + "5";
                    string headerNombre = "A1:" + Char.ConvertFromUtf32(alum[0].Length + 64) + "1";
                    string headerEscolaridad = "A2:" + Char.ConvertFromUtf32(esc[0].Length + 64) + "2";
                    string headerGrupo = "A3:" + Char.ConvertFromUtf32(grup[0].Length + 64) + "3";

                    // Popular header row data
                    worksheet.Cells[headerRange].LoadFromArrays(headerRow);
                    worksheet.Cells[headerContent].LoadFromArrays(Content);
                    worksheet.Cells[headerNombre].LoadFromArrays(alum);
                    worksheet.Cells[headerEscolaridad].LoadFromArrays(esc);
                    worksheet.Cells[headerGrupo].LoadFromArrays(grup);



                    FileInfo excelFile = new FileInfo(@"C:\temp\" + Alumno + " - Reporte de Pagos.xlsx");
                    excel.SaveAs(excelFile);
                }
                return Json(Reportevm);
            }
            catch (Exception Ex)
            {
                return Json(" " + Ex.Message);
            }
        }

        // GET: Reporte/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago pago = db.Pago.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            return View(pago);
        }



        // POST: Reporte/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_pago,Fecha,id_alumno,id_Precio,Activo")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                db.Pago.Add(pago);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_alumno = new SelectList(db.Alumno, "id_Alumno", "Nombre", pago.GrupoAlumno.id_Alumno);
            ViewBag.id_Precio = new SelectList(db.Precio, "id_Precio", "Concepto", pago.id_Precio);
            return View(pago);
        }

        // GET: Reporte/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago pago = db.Pago.Include(x => x.GrupoAlumno).SingleOrDefault(x => x.id_pago == id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_alumno = new SelectList(db.Alumno, "id_Alumno", "Nombre", pago.GrupoAlumno.id_Alumno);
            ViewBag.id_Precio = new SelectList(db.Precio, "id_Precio", "Concepto", pago.id_Precio);
            return View(pago);
        }

        // POST: Reporte/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_pago,Fecha,id_alumno,id_Precio,Activo")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pago).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_alumno = new SelectList(db.Alumno, "id_Alumno", "Nombre", pago.GrupoAlumno.id_Alumno);
            ViewBag.id_Precio = new SelectList(db.Precio, "id_Precio", "Concepto", pago.id_Precio);
            return View(pago);
        }

        // GET: Reporte/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago pago = db.Pago.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            return View(pago);
        }

        // POST: Reporte/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pago pago = db.Pago.Find(id);
            db.Pago.Remove(pago);
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
    }
}
