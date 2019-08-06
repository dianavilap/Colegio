using ColegioArceProject.AuthControllers;
using ColegioArceProject.enums;
using ColegioArceProject.Models;
using ColegioArceProject.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Image = iTextSharp.text.Image;

namespace ColegioArceProject.Controllers
{
    public class PagoController : UserAuthController
    {//cambio
        private ColegioArce_Entities db = new ColegioArce_Entities();

        // GET: Pago cambio prueba
        public ActionResult Index()
        {
            var Pagodb = db.Pago
                .Include(x => x.GrupoAlumno)
                .Include(x => x.Abono1)
                .Include(x => x.GrupoAlumno.Alumno)
                .Include(x => x.Precio)
                .Where(x => x.Activo == true)
                .OrderByDescending(x => x.id_pago);


            var ListPagos = new List<PagoViewModel>();

            foreach (Pago pago in Pagodb)
            {
                ListPagos.Add(new PagoViewModel
                {
                    id_pago = pago.id_pago,
                    Fecha = pago.Fecha.ToString("dd/MM/yyyy"),
                    Alumno = pago.GrupoAlumno.Alumno.ApellidoP + " " + pago.GrupoAlumno.Alumno.ApellidoM + " " + pago.GrupoAlumno.Alumno.Nombre,
                    DescripcionPrecio = pago.DescripcionPago,
                    Abonos = pago.Abono1.ToList(),
                    Importe = pago.Importe,
                    folio = pago.Folio
                    //DescripcionEscolaridad = pago.Alumno.Grupo.Escolaridad.Descripcion,
                });
            }


            return View(ListPagos);
        }

        // GET: Pago/Details/5
        public ActionResult Details(int id)
        {

            Pago pago = db.Pago.Include(x => x.GrupoAlumno).Include(x => x.Precio).Include(x => x.Abono1).Include(x => x.GrupoAlumno.Alumno).SingleOrDefault(x => x.id_pago == id);
            if (pago == null)
            {
                SetMessage("No se encontró el pago, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }

            return View(pago);
        }

        // GET: Pago/Create
        public ActionResult Create()
        {

            var model = new PagoViewModel();
            model.Fecha = DateTime.Now.ToString("dd/MM/yyyy");

            var comoPago = new List<SelectListItem>()
            {
                new SelectListItem() {Text="Efectivo", Value="Efectivo", Selected= true},
                new SelectListItem() {Text="Tarjeta de Crédito", Value="Tarjeta de Credito"},
                new SelectListItem() {Text="Tarjeta de Débito", Value="Tarjeta de Debito"},
                new SelectListItem() {Text="Transferencia Bancaria", Value="Transferencia Bancaria"},
                new SelectListItem() {Text="Depósito", Value="Deposito"},
            };

            ViewBag.comoPago = comoPago;

            //var precios = db.Precio.ToList();

            //var showprecios = precios.Select(x => new { id_Precio = x.id_Precio, Concepto = x.Concepto + " " + x.Importe.ToString("C") }).ToList();

            //ViewBag.id_Alumno = new SelectList(db.Alumno, "id_Alumno", "Nombre");
            //ViewBag.id_Precio = new SelectList(showprecios, "id_Precio", "Concepto");

            return View(model);
        }


        // POST: Pago/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PagoViewModel pago)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.id_Alumno = new SelectList(db.Alumno, "id_Alumno", "Nombre", pago.id_alumno);
                //ViewBag.id_Precio = new SelectList(db.Precio, "id_Precio", "Concepto", pago.id_Precio);
                var comoPago = new List<SelectListItem>()
            {
                new SelectListItem() {Text="Efectivo", Value="Efectivo", Selected= true},
                new SelectListItem() {Text="Tarjeta de Crédito", Value="Tarjeta de Credito"},
                new SelectListItem() {Text="Tarjeta de Débito", Value="Tarjeta de Debito"},
                new SelectListItem() {Text="Transferencia Bancaria", Value="Transferencia Bancaria"},
                new SelectListItem() {Text="Depósito", Value="Deposito"},
            };

                ViewBag.comoPago = comoPago;


                SetMessage("No se puedo registrar el pago, por favor intente de nuevo", BootstrapAlertTypes.Danger);
                return View(pago);
            }


            var precio = db.Precio
                .SingleOrDefault(x => x.id_Precio == pago.id_Precio);

            if (precio == null)
            {
                SetMessage("Ocurrió un error, por favor intente de nuevo", BootstrapAlertTypes.Danger);
                return View(pago);
            }

            var pagos = db.Pago.ToList();
            var abonos = db.Abono.ToList();
            int folio = 0;

            if (pagos.Count == 0)
            {
                folio = 1;
            }
            else
            {
                if (abonos.Count > 0)
                {
                    var folio1 = pagos.LastOrDefault().Folio;
                    var folio2 = abonos.LastOrDefault().Folio;

                    if (folio1 >= folio2)
                    {
                        folio = folio1 + 1;
                    }
                    else
                    {
                        folio = folio2 + 1;
                    }
                }
                else
                {
                    folio = pagos.LastOrDefault().Folio + 1;
                }
            }



            if (pago.TipoPago.Equals("Pago"))
            {
                if (pago.id_pago != 0)
                {
                    var pagototal = db.Pago
                   .Include(x => x.Precio)
                   .SingleOrDefault(x => x.id_pago == pago.id_pago);

                    if (pagototal == null)
                    {
                        SetMessage("No se encontró el pago para generar un abono.", BootstrapAlertTypes.Danger);
                        return RedirectToAction("Index");
                    }

                    var abonopagototal = precio.Importe - pagototal.Importe;

                    pagototal.Importe = precio.Importe;

                    db.Entry(pagototal).State = EntityState.Modified;
                    db.SaveChanges();

                    var abono = new Abono
                    {
                        Importe = abonopagototal,
                        Fecha = DateTime.Now,
                        id_Pago = pago.id_pago,
                        TipoPago = pago.comoPago,
                        Folio = folio
                    };

                    db.Abono.Add(abono);
                    db.SaveChanges();

                    SetMessage("Se liquidó totalmente la colegiatura", BootstrapAlertTypes.Success);
                    return Redirect("/Pago/VerificarPago/" + pagototal.id_pago);

                }

                pago.Abono = precio.Importe;
                pago.Activo = true;

                var guardarpago = new Pago
                {
                    Fecha = DateTime.Now,
                    id_GrupoAlumno = pago.id_GrupoAlumno,
                    id_Precio = pago.id_Precio,
                    Importe = pago.Abono,
                    Activo = pago.Activo,
                    TipoPago = pago.comoPago,
                    Abono = false,
                    Folio = folio,
                    DescripcionPago = pago.Precio
                };

                db.Pago.Add(guardarpago);
                db.SaveChanges();

                SetMessage("Pago registrado.", BootstrapAlertTypes.Success);
                return Redirect("/Pago/VerificarPago/" + guardarpago.id_pago);
            }

            if (pago.TipoPago.Equals("Promocion"))
            {
                pago.Abono = pago.Promo;
                pago.Activo = true;
                pago.Importe = pago.Promo;

                var promo = new Pago
                {
                    Fecha = DateTime.Now,
                    id_GrupoAlumno = pago.id_GrupoAlumno,
                    id_Precio = 46,
                    Importe = pago.Promo,
                    Activo = true,
                    TipoPago = pago.comoPago,
                    Abono = false,
                    Folio = folio,
                    DescripcionPago = pago.Precio.Substring(0, 6) + " PROMOCIÓN"
                };

                db.Pago.Add(promo);
                db.SaveChanges();

                SetMessage("Pago registrado.", BootstrapAlertTypes.Success);
                return Redirect("/Pago/VerificarPago/" + promo.id_pago);
            }


            if (pago.id_pago != 0)
            {
                var pagoconabono = db.Pago
                    .Include(x => x.Precio)
                    .SingleOrDefault(x => x.id_pago == pago.id_pago);

                if (pagoconabono == null)
                {
                    SetMessage("No se encontró el pago para generar un abono.", BootstrapAlertTypes.Danger);
                    return RedirectToAction("Index");
                }

                pagoconabono.Importe = pagoconabono.Importe + pago.Abono;

                db.Entry(pagoconabono).State = EntityState.Modified;
                db.SaveChanges();

                var abono = new Abono
                {
                    Importe = pago.Abono,
                    Fecha = DateTime.Now,
                    id_Pago = pago.id_pago,
                    TipoPago = pago.comoPago,
                    Folio = folio
                };

                db.Abono.Add(abono);
                db.SaveChanges();

                SetMessage("Abono registrado.", BootstrapAlertTypes.Success);
                return Redirect("/Pago/VerificarPago/" + pagoconabono.id_pago);

            }

            var guardarpagoabono = new Pago
            {
                Fecha = DateTime.Now,
                id_GrupoAlumno = pago.id_GrupoAlumno,
                id_Precio = pago.id_Precio,
                Importe = pago.Abono,
                Activo = true,
                TipoPago = pago.comoPago,
                Abono = true,
                Folio = folio,
                DescripcionPago = pago.Precio
            };

            db.Pago.Add(guardarpagoabono);
            db.SaveChanges();

            if (pago.TipoPago.Equals("Abono"))
            {
                var abono = new Abono
                {
                    Importe = pago.Abono,
                    Fecha = DateTime.Now,
                    id_Pago = guardarpagoabono.id_pago,
                    TipoPago = pago.comoPago,
                    Folio = folio
                };

                db.Abono.Add(abono);
                db.SaveChanges();

                SetMessage("Abono registrado.", BootstrapAlertTypes.Success);
                return Redirect("/Pago/VerificarPago/" + guardarpagoabono.id_pago);
            }



            imprimir(guardarpagoabono.id_pago);
            SetMessage("Pago registrado.", BootstrapAlertTypes.Success);
            return RedirectToAction("Index");


        }

        public ActionResult VerificarPago(int? idPago)
        {
            var pago = db.Pago
                 .Include(x => x.Abono1)
                 .Include(x => x.GrupoAlumno)
                 .Include(x => x.GrupoAlumno.Alumno)
                 .Include(x => x.Precio)
                 .Include(x => x.GrupoAlumno.Grupo)
                 .Include(X => X.GrupoAlumno.Grupo.Escolaridad)
                 .SingleOrDefault(x => x.id_pago == idPago);

            if (pago == null)
            {
                SetMessage("No se encontró el pago, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }


            return View(pago);

        }



        // GET: Pago/Delete/5
        public ActionResult Delete(int id)
        {
            Pago pago = db.Pago.Find(id);
            if (pago == null)
            {
                SetMessage("No se encontró el pago, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }


            return View(pago);
        }

        // POST: Pago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pago pago = db.Pago.Find(id);
            if (pago == null)
            {
                SetMessage("No se encontró el pago, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }


            pago.Activo = false;
            db.Entry(pago).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult DeleteAbono(int id)
        {
            var abono = db.Abono.SingleOrDefault(x => x.id_Abono == id);

            if (abono == null)
            {
                SetMessage("No se encotró el abono", BootstrapAlertTypes.Danger);
                return Redirect("/Pago");
            }

            var pago = db.Pago.SingleOrDefault(x => x.id_pago == abono.id_Pago);

            if (pago == null)
            {
                if (abono == null)
                {
                    SetMessage("No se encotró el pago para descontar abono", BootstrapAlertTypes.Danger);
                    return Redirect("/Pago");
                }
            }

            abono.Cancelado = true;
            db.Entry(abono).State = EntityState.Modified;
            db.SaveChanges();

            pago.Importe = pago.Importe - abono.Importe;
            db.Entry(pago).State = EntityState.Modified;
            db.SaveChanges();


            SetMessage("El abono ha sido eliminado.", BootstrapAlertTypes.Success);
            return Redirect("/Pago/Details/" + abono.id_Pago);
        }

        [HttpGet]
        public JsonResult GetAlumnos(string Areas, string buscar = "")
        {
            //var alumnoslist = db.Alumno
            //                .Where(x => x.Nombre.ToUpper().Contains(buscar.ToUpper()) || x.ApellidoP.ToUpper().Contains(buscar.ToUpper()) || x.ApellidoM.ToUpper().Contains(buscar.ToUpper()))
            //                .Select(x => new { Alumno = x.Nombre + " " + x.ApellidoP + " " + x.ApellidoM, id = x.id_Alumno })
            //                .Distinct().ToList();


            var alumnobusqueda = db.Alumno
                              .Where(x => x.Activo == true)
                             .Select(x => new { Alumno = x.ApellidoP + " " + x.ApellidoM + " " + x.Nombre, id = x.id_Alumno })
                             .Distinct().ToList();

            alumnobusqueda = alumnobusqueda.Where(x => x.Alumno.StartsWith(buscar, StringComparison.OrdinalIgnoreCase)).ToList();

            return Json(alumnobusqueda, JsonRequestBehavior.AllowGet);

        }


        public ActionResult imprimir(int id)
        {

            var pago = db.Pago.Include(x => x.Precio).Include(x => x.Abono1).Include(x => x.GrupoAlumno).Include(x => x.GrupoAlumno.Alumno).Include(x => x.GrupoAlumno.Grupo).Include(x => x.GrupoAlumno.Grupo.Escolaridad).SingleOrDefault(x => x.id_pago == id);

            var memoryStream = new MemoryStream();
            crearReciboPago(memoryStream, pago);
            return File(memoryStream.GetBuffer(), "application/pdf", "Pago-" + pago.id_pago + ".pdf");
        }

        public ActionResult imprimirAbono(int id)
        {
            var abono = db.Abono.Include(x => x.Pago).Include(x => x.Pago.GrupoAlumno).Include(x => x.Pago.GrupoAlumno.Alumno).Include(x => x.Pago.Precio).Include(x => x.Pago.GrupoAlumno.Grupo.Escolaridad).SingleOrDefault(x => x.id_Abono == id);

            var memoryStream = new MemoryStream();
            imprimirReciboAbono(memoryStream, abono);
            return File(memoryStream.GetBuffer(), "application/pdf", "Pago-" + abono.id_Pago + ".pdf");
        }


        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetPrecios(int id)
        {
            var alumno = db.Alumno
                .SingleOrDefault(x => x.id_Alumno == id);

            if (alumno == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            List<Precio> precios = db.Precio.ToList();

            var pagos = db.Pago
               .Include(x => x.Precio)
               .Include(x => x.GrupoAlumno)
               .Where(x => x.Activo == true)
               .Where(x => x.GrupoAlumno.id_Alumno == id)
               .OrderByDescending(x => x.Fecha)
               .ToList();

            // PARA ADEUDO DE SEMESTRES ANTERIORES
            string descripcion = "";

            var gruposAnteriores = db.GrupoAlumno.Include(x => x.Grupo).Include(x => x.Grupo.Escolaridad).Include(x => x.Pago).Where(x => x.id_Alumno == id).Where(x => x.Activo == false).ToList();


            foreach (var item in gruposAnteriores)
            {
                if (item.Pago.Where(x => x.Activo == true).ToList().Count < 5)
                {
                    var pagopendiente = pagos.Where(x => x.id_GrupoAlumno == item.id_GrupoAlumno).OrderBy(x => x.id_pago).LastOrDefault();
                    var totalpagos = 0;
                    if ((pagopendiente.Activo == true) && (pagopendiente.Precio.Importe > pagopendiente.Importe))
                    {
                        var precioabono = db.Precio.Where(x => x.id_Precio == pagopendiente.id_Precio).ToList();
                        var importe = pagopendiente.Precio.Importe - pagopendiente.Importe;

                        //totalpagos = item.Pago.Where(x => x.Activo == true).ToList().Count();
                        //descripcion = "PAGO " + totalpagos.ToString();

                        var valoresabono = precioabono.Select(x => new { Descrpcion = x.Concepto + " " + "$" + importe + ".00", Id = x.id_Precio, Pago = pagopendiente.id_pago, idGrupo = item.id_GrupoAlumno }).ToList();

                        return Json(valoresabono, JsonRequestBehavior.AllowGet);

                    }


                    if (item.Pago.Count == 0)
                    {

                        precios = precios.Where(x => x.id_Precio == 42).ToList();
                        return Json(precios.Select(x => new { Descrpcion = x.Concepto + " " + x.Importe.ToString("C"), Id = x.id_Precio, Pago = 0, idGrupo = item.id_GrupoAlumno }).ToList(), JsonRequestBehavior.AllowGet);
                    }

                    totalpagos = item.Pago.Where(x => x.Activo == true).ToList().Count();

                    descripcion = "PAGO " + totalpagos.ToString();

                    precios = precios.Where(x => x.id_Precio == 45).ToList();
                    return Json(precios.Select(x => new { Descrpcion = descripcion + " " + x.Concepto + " " + x.Importe.ToString("C"), Id = x.id_Precio, Pago = 0, idGrupo = item.id_GrupoAlumno }).ToList(), JsonRequestBehavior.AllowGet);

                }
            }

            // PARA ADEUDO DEL SEMESTRE ACTUAL

            var grupo = db.GrupoAlumno.Include(x => x.Grupo).Include(x => x.Grupo.Escolaridad).Include(x => x.Pago).Where(x => x.id_Alumno == id).Where(x => x.Activo == true).ToList();

            if (grupo == null || grupo.Count() == 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var grupoActual = grupo.FirstOrDefault();

            var escolaridad = grupo.FirstOrDefault().Grupo.Escolaridad;

            var dia = DateTime.Now.Day;

            var format = "MMMM";

            var mes = DateTime.Now.ToString("MMMM").ToUpperInvariant();
            int mesactual = DateTime.Now.Month;


            var noPagos = grupoActual.Pago.Where(x => x.Activo == true).Count();

            if (noPagos > 0)
            {
                var pagopendiente = pagos.Where(x => x.id_GrupoAlumno == grupoActual.id_GrupoAlumno).OrderBy(x => x.id_pago).LastOrDefault();
                if ((pagopendiente.Activo == true) && (pagopendiente.Precio.Importe > pagopendiente.Importe))
                {
                    var precioabono = db.Precio.Where(x => x.id_Precio == pagopendiente.id_Precio).ToList();
                    var importe = pagopendiente.Precio.Importe - pagopendiente.Importe;

                    var valoresabono = precioabono.Select(x => new { Descrpcion = x.Concepto + " " + "$" + importe + ".00", Id = x.id_Precio, Pago = pagopendiente.id_pago, idGrupo = grupoActual.id_GrupoAlumno }).ToList();

                    return Json(valoresabono, JsonRequestBehavior.AllowGet);

                }
            }
            var rango = db.Rango
                .Where(x => x.id_Rango != 5)
                .Where(x => x.Inicio <= dia && x.Fin >= dia).FirstOrDefault();



            switch (noPagos)
            {
                case 0:
                    precios = precios.Where(x => x.id_Precio == 42).ToList();
                    break;

                case 1:
                    switch (mesactual)
                    {
                        case 3:
                        case 8:
                        case 11:
                            precios = db.Precio
                                .Include(x => x.Rango)
                                .Where(x => x.id_Escolaridad == escolaridad.id_escolaridad)
                                .Where(x => x.id_Rango == rango.id_Rango)
                                .ToList();
                            descripcion = "PAGO 1";
                            break;


                        default:
                            precios = precios.Where(x => x.id_Precio == 45).ToList();
                            descripcion = "PAGO 1";
                            break;
                    }
                    break;
                case 2:
                    switch (mesactual)
                    {
                        case 4:
                        case 9:
                        case 12:

                            precios = db.Precio
                                .Include(x => x.Rango)
                                .Where(x => x.id_Escolaridad == escolaridad.id_escolaridad)
                                .Where(x => x.id_Rango == rango.id_Rango)
                                .ToList();
                            descripcion = "PAGO 2";
                            break;

                        case 3:
                        case 8:
                        case 11:
                            precios = precios.Where(x => x.id_Precio == 43).ToList();
                            descripcion = "PAGO 2";
                            break;
                        default:
                            precios = precios.Where(x => x.id_Precio == 45).ToList();
                            descripcion = "PAGO 2";
                            break;


                    }
                    break;
                case 3:
                    switch (mesactual)
                    {
                        case 1:
                        case 5:
                        case 10:
                            precios = db.Precio
                                 .Include(x => x.Rango)
                                 .Where(x => x.id_Escolaridad == escolaridad.id_escolaridad)
                                 .Where(x => x.id_Rango == rango.id_Rango)
                                 .ToList();
                            descripcion = "PAGO 3";
                            break;
                        case 3:
                        case 8:
                        case 11:
                        case 4:
                        case 9:
                        case 12:
                            precios = precios.Where(x => x.id_Precio == 43).ToList();
                            descripcion = "PAGO 3";
                            break;
                        default:
                            precios = precios.Where(x => x.id_Precio == 45).ToList();
                            descripcion = "PAGO 3";
                            break;

                    }
                    break;
                case 4:
                    switch (mesactual)
                    {
                        case 2:
                        case 6:
                        case 11:
                            precios = db.Precio
                                 .Include(x => x.Rango)
                                 .Where(x => x.id_Escolaridad == escolaridad.id_escolaridad)
                                 .Where(x => x.id_Rango == rango.id_Rango)
                                 .ToList();
                            descripcion = "PAGO 4";
                            break;

                        case 3:
                        case 8:
                        case 4:
                        case 9:
                        case 12:
                        case 1:
                        case 5:
                        case 10:
                            precios = precios.Where(x => x.id_Precio == 43).ToList();
                            descripcion = "PAGO 4";
                            break;
                        default:
                            precios = precios.Where(x => x.id_Precio == 45).ToList();
                            descripcion = "PAGO 4";
                            break;

                    }
                    break;
            };



            var valores = precios.Select(x => new { Descrpcion = descripcion + " " + x.Concepto + " " + x.Importe.ToString("C"), Id = x.id_Precio, Pago = 0, idGrupo = grupoActual.id_GrupoAlumno }).ToList();

            return Json(valores, JsonRequestBehavior.AllowGet);

            //var precio = new List { descripcion =  };
        }

        public void crearReciboPago(Stream output, Pago pago)
        {
            var documento = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(documento, output);
            documento.Open();
            var fontTitle = new Font(Font.FontFamily.HELVETICA, 26, 2);
            var fontParagraph = new Font(Font.FontFamily.HELVETICA, 12, 2);

            //var pagoimprimir = db.Pago.Include(x => x.Precio).Include(x => x.Alumno).FirstOrDefault(x => x.id_Precio == pago.id_pago);

            //int count = pago.id_pago.ToString().Length;

            //Paragraph folio = new Paragraph(count > 2 ? "Folio - 000" + pago.id_pago : "Folio - 0000" + pago.id_pago, fontParagraph);
            //folio.Alignment = Element.ALIGN_RIGHT;


            Paragraph alumno = new Paragraph("Nombre del Alumno: " + pago.GrupoAlumno.Alumno.Nombre + " " + pago.GrupoAlumno.Alumno.ApellidoP + " " + pago.GrupoAlumno.Alumno.ApellidoM, fontParagraph);
            Paragraph escolaridad = new Paragraph("Grupo y escolaridad: " + pago.GrupoAlumno.Grupo.Descripcion + " " + pago.GrupoAlumno.Grupo.Escolaridad.Descripcion, fontParagraph);
            Paragraph concepto = new Paragraph("Colegiatura: " + pago.DescripcionPago, fontParagraph);
            Paragraph formaPago = new Paragraph("Forma de Pago: " + pago.TipoPago, fontParagraph);

            Paragraph fecha = new Paragraph("       " + pago.Folio + "          " + "       " + DateTime.Now.ToString("dd/MM/yyyy"), fontParagraph);
            fecha.Alignment = Element.ALIGN_RIGHT;

            var path = Server.MapPath("~/Content/images/LogoCA.png");
            Image logo = Image.GetInstance(path);
            logo.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
            logo.IndentationLeft = 9f;
            logo.SpacingAfter = 9f;

            String montoletras = "";

            montoletras = enletras(pago.Importe.ToString());
            montoletras = ToUpperFirstLetter(montoletras.ToLower());

            documento.AddHeader("Recibo de pago", pago.GrupoAlumno.Alumno.Nombre + " " + pago.GrupoAlumno.Alumno.ApellidoP + " " + pago.GrupoAlumno.Alumno.ApellidoM);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            // documento.Add(new Paragraph("Colegio Arce", fontTitle));
            //documento.Add(folio);
            documento.Add(fecha);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(alumno);
            documento.Add(escolaridad);
            documento.Add(concepto);
            documento.Add(formaPago);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);


            if (pago.Abono1.Count > 0)
            {
                var abonos = pago.Abono1.Where(x => x.Cancelado == false).OrderBy(x => x.id_Abono);
                int contador = 1;
                foreach (var item in abonos)
                {
                    Paragraph importe = new Paragraph(item.Fecha.ToString("dd/MM/yyyy") + " Abono " + contador + ":     " + item.Importe.ToString("C"), fontParagraph);
                    importe.Alignment = Element.ALIGN_RIGHT;

                    documento.Add(importe);
                    contador = contador + 1;
                }

                Paragraph importetotal = new Paragraph("Importe total:   " + pago.Importe.ToString("C"), fontParagraph);
                Paragraph enletrastotal = new Paragraph("(SON: " + montoletras + " pesos 00/M.N.)", fontParagraph);
                Paragraph line = new Paragraph("________", fontParagraph);
                importetotal.Alignment = Element.ALIGN_RIGHT;
                enletrastotal.Alignment = Element.ALIGN_RIGHT;
                line.Alignment = Element.ALIGN_RIGHT;

                documento.Add(line);
                documento.Add(importetotal);
                documento.Add(enletrastotal);
            }
            else
            {
                Paragraph importe = new Paragraph("Importe total: " + pago.Importe.ToString("C"), fontParagraph);
                Paragraph enletras = new Paragraph("(SON: " + montoletras + " pesos 00/M.N.)", fontParagraph);
                importe.Alignment = Element.ALIGN_RIGHT;
                enletras.Alignment = Element.ALIGN_RIGHT;
                documento.Add(importe);
                documento.Add(enletras);
            }

            documento.Add(Chunk.NEWLINE);

            documento.Close();
            writer.Close();

        }


        public void imprimirReciboAbono(Stream output, Abono abono)
        {
            var documento = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(documento, output);
            documento.Open();
            var fontTitle = new Font(Font.FontFamily.HELVETICA, 26, 2);
            var fontParagraph = new Font(Font.FontFamily.HELVETICA, 12, 2);

            //var pagoimprimir = db.Pago.Include(x => x.Precio).Include(x => x.Alumno).FirstOrDefault(x => x.id_Precio == pago.id_pago);

            //int count = abono.Pago.id_pago.ToString().Length;

            //Paragraph folio = new Paragraph(count > 2 ? "Folio - 000" + abono.id_Pago : "Folio - 0000" + abono.id_Pago, fontParagraph);
            //folio.Alignment = Element.ALIGN_RIGHT;


            Paragraph alumno = new Paragraph("Nombre del Alumno: " + abono.Pago.GrupoAlumno.Alumno.Nombre + " " + abono.Pago.GrupoAlumno.Alumno.ApellidoP + " " + abono.Pago.GrupoAlumno.Alumno.ApellidoM, fontParagraph);
            Paragraph escolaridad = new Paragraph("Grupo y escolaridad: " + abono.Pago.GrupoAlumno.Grupo.Descripcion + " " + abono.Pago.GrupoAlumno.Grupo.Escolaridad.Descripcion, fontParagraph);
            Paragraph concepto = new Paragraph("Colegiatura: " + abono.Pago.DescripcionPago, fontParagraph);
            Paragraph formaPago = new Paragraph("Forma de Pago: " + abono.Pago.TipoPago, fontParagraph);

            Paragraph fecha = new Paragraph("       " + abono.Folio + "          " + "       " + DateTime.Now.ToString("dd/MM/yyyy"), fontParagraph);
            fecha.Alignment = Element.ALIGN_RIGHT;

            var path = Server.MapPath("~/Content/images/LogoCA.png");
            Image logo = Image.GetInstance(path);
            logo.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
            logo.IndentationLeft = 9f;
            logo.SpacingAfter = 9f;

            String montoletras = "";

            montoletras = enletras(abono.Importe.ToString());
            montoletras = ToUpperFirstLetter(montoletras.ToLower());


            documento.AddHeader("Recibo de Abono: ", abono.Pago.GrupoAlumno.Alumno.Nombre + " " + abono.Pago.GrupoAlumno.Alumno.ApellidoP + " " + abono.Pago.GrupoAlumno.Alumno.ApellidoM);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            //documento.Add(new Paragraph("Recibo de abono - Colegio Arce", fontTitle));
            //documento.Add(folio);
            documento.Add(fecha);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(alumno);
            documento.Add(escolaridad);
            documento.Add(concepto);
            documento.Add(formaPago);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);

            Paragraph importe = new Paragraph("Importe total del abono: " + abono.Importe.ToString("C"), fontParagraph);
            Paragraph enletras1 = new Paragraph("(SON: " + montoletras + " pesos 00/M.N.)", fontParagraph);
            importe.Alignment = Element.ALIGN_RIGHT;
            enletras1.Alignment = Element.ALIGN_RIGHT;
            documento.Add(importe);
            documento.Add(enletras1);


            documento.Add(Chunk.NEWLINE);

            documento.Close();
            writer.Close();

        }

        [HttpPost]
        public ActionResult EstadoCuenta(int? id)
        {
            if (id == null)
            {
                SetMessage("No se encontró al alumno.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }
            var pagos = db.Pago
                .Include(p => p.GrupoAlumno)
                .Include(p => p.GrupoAlumno.Grupo.Escolaridad)
                .Include(p => p.Precio)
                .Include(x => x.Abono)
                .Where(x => x.Activo == true && x.GrupoAlumno.id_Alumno == id)
                .OrderByDescending(x => x.Fecha).ToList();
            var ListPagos = new List<PagoViewModel>();
            List<Abono> Listabono = null;



            foreach (Pago pago in pagos)
            {
                if (pago.Abono != null)
                {
                    Listabono = db.Abono.Where(x => x.id_Pago == pago.id_pago).Where(x => x.Cancelado == false).ToList();
                    foreach (Abono abono in Listabono)
                    {
                        ListPagos.Add(new PagoViewModel
                        {
                            id_pago = abono.id_Pago,
                            Fecha = abono.Fecha.ToString("dd/MM/yyyy"),
                            Alumno = pago.GrupoAlumno.Alumno.Nombre + " " + pago.GrupoAlumno.Alumno.ApellidoP + " " + pago.GrupoAlumno.Alumno.ApellidoM,
                            //DescripcionEscolaridad = pago.Alumno.Grupo.Escolaridad.Descripcion,
                            DescripcionPrecio = "Abono de " + pago.Precio.Concepto,
                            Importe = abono.Importe
                        });
                    }
                }
                ListPagos.Add(new PagoViewModel
                {
                    id_pago = pago.id_pago,
                    Fecha = pago.Fecha.ToString("dd/MM/yyyy"),
                    Alumno = pago.GrupoAlumno.Alumno.Nombre + " " + pago.GrupoAlumno.Alumno.ApellidoP + " " + pago.GrupoAlumno.Alumno.ApellidoM,
                    //DescripcionEscolaridad = pago.Alumno.Grupo.Escolaridad.Descripcion,
                    DescripcionPrecio = pago.Precio.Concepto,
                    Importe = pago.Importe

                });
            }

            return View(ListPagos);
        }


        [HttpGet]
        public ActionResult EstadoCuenta(int id)
        {

            var Pagos = new List<Pago>();
            var Reportevm = new List<ReporteViewModel>();
            var ReporteAlumno = new ReporteAlumnoViewModel();

            //var alumnobusqueda = db.Alumno
            //    .Select(x => new { NombreCompleto = x.ApellidoP + " " + x.ApellidoM + " " + x.Nombre, id = x.id_Alumno })
            //    .Distinct().ToList();

            //var AlumnoPagos = alumnobusqueda.Where(x => x.NombreCompleto.Equals(Alumno)).Select(x => new { idAlumno = x.id }).FirstOrDefault();

            Pagos = db.Pago
                .Include(x => x.GrupoAlumno)
                .Include(x => x.GrupoAlumno.Grupo)
                .Include(x => x.GrupoAlumno.Alumno)
                .Include(x => x.Precio)
                .Include(x => x.Abono1)
                .Where(x => x.GrupoAlumno.id_Alumno == id && x.Activo == true)
                .ToList();
            if (Pagos == null || Pagos.Count == 0)
            {
                SetMessage("No se encontraron pagos para el Alumno.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Create");
            }

            List<Abono> Listabono = null;
            foreach (var Pagodb in Pagos)
            {
                if (Pagodb.Abono != false)
                {
                    Listabono = db.Abono.Where(x => x.id_Pago == Pagodb.id_pago && x.Cancelado == false).ToList();
                    foreach (Abono abono in Listabono)
                    {
                        Reportevm.Add(new ReporteViewModel
                        {
                            Fecha = Pagodb.Fecha.ToString("dd/MM/yyyy"),
                            Folio = abono.Folio.ToString(),
                            //DescripcionEscolaridad = pago.Alumno.Grupo.Escolaridad.Descripcion,
                            ConceptoPago = "Abono de " + Pagodb.Precio.Concepto,
                            Importe = abono.Importe
                        });
                    }
                }
                else
                {
                    Reportevm.Add(new ReporteViewModel
                    {
                        Fecha = Pagodb.Fecha.ToShortDateString(),
                        Folio = Pagodb.Folio.ToString(),
                        ConceptoPago = Pagodb.Precio.Concepto,
                        Importe = Pagodb.Importe
                    });
                }
            }
            var datos = Pagos
                 .Where(x => x.GrupoAlumno.id_Alumno == id)
                 .Select(x => new { Grupo = x.GrupoAlumno.Grupo.Descripcion, Escolaridad = x.GrupoAlumno.Grupo.Escolaridad.Descripcion, Nombre = x.GrupoAlumno.Alumno.ApellidoP + " " + x.GrupoAlumno.Alumno.ApellidoM + " " + x.GrupoAlumno.Alumno.Nombre }).FirstOrDefault();
            ReporteAlumno.Alumno = datos.Nombre;
            ReporteAlumno.Escolaridad = datos.Escolaridad;
            ReporteAlumno.Grupo = datos.Grupo;
            ReporteAlumno.NombreCompleto = datos.Nombre;
            ReporteAlumno.ListaPagos = Reportevm;


            return View(ReporteAlumno);


        }

        public string enletras(string num)
        {
            string res, dec = "";
            Int64 entero;
            int decimales;
            double nro;

            try

            {
                nro = Convert.ToDouble(num);
            }
            catch
            {
                return "";
            }

            entero = Convert.ToInt64(Math.Truncate(nro));
            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));
            if (decimales > 0)
            {
                dec = " CON " + decimales.ToString() + "/100";
            }

            res = toText(Convert.ToDouble(entero)) + dec;
            return res;
        }

        public string toText(double value)
        {
            string Num2Text = "";
            value = Math.Truncate(value);
            if (value == 0) Num2Text = "CERO";
            else if (value == 1) Num2Text = "UNO";
            else if (value == 2) Num2Text = "DOS";
            else if (value == 3) Num2Text = "TRES";
            else if (value == 4) Num2Text = "CUATRO";
            else if (value == 5) Num2Text = "CINCO";
            else if (value == 6) Num2Text = "SEIS";
            else if (value == 7) Num2Text = "SIETE";
            else if (value == 8) Num2Text = "OCHO";
            else if (value == 9) Num2Text = "NUEVE";
            else if (value == 10) Num2Text = "DIEZ";
            else if (value == 11) Num2Text = "ONCE";
            else if (value == 12) Num2Text = "DOCE";
            else if (value == 13) Num2Text = "TRECE";
            else if (value == 14) Num2Text = "CATORCE";
            else if (value == 15) Num2Text = "QUINCE";
            else if (value < 20) Num2Text = "DIECI" + toText(value - 10);
            else if (value == 20) Num2Text = "VEINTE";
            else if (value < 30) Num2Text = "VEINTI" + toText(value - 20);
            else if (value == 30) Num2Text = "TREINTA";
            else if (value == 40) Num2Text = "CUARENTA";
            else if (value == 50) Num2Text = "CINCUENTA";
            else if (value == 60) Num2Text = "SESENTA";
            else if (value == 70) Num2Text = "SETENTA";
            else if (value == 80) Num2Text = "OCHENTA";
            else if (value == 90) Num2Text = "NOVENTA";
            else if (value < 100) Num2Text = toText(Math.Truncate(value / 10) * 10) + " Y " + toText(value % 10);
            else if (value == 100) Num2Text = "CIEN";
            else if (value < 200) Num2Text = "CIENTO " + toText(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = toText(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) Num2Text = "QUINIENTOS";
            else if (value == 700) Num2Text = "SETECIENTOS";
            else if (value == 900) Num2Text = "NOVECIENTOS";
            else if (value < 1000) Num2Text = toText(Math.Truncate(value / 100) * 100) + " " + toText(value % 100);
            else if (value == 1000) Num2Text = "MIL";
            else if (value < 2000) Num2Text = "MIL " + toText(value % 1000);
            else if (value < 1000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0) Num2Text = Num2Text + " " + toText(value % 1000);
            }

            else if (value == 1000000) Num2Text = "UN MILLON";
            else if (value < 2000000) Num2Text = "UN MILLON " + toText(value % 1000000);
            else if (value < 1000000000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000) * 1000000);
            }

            else if (value == 1000000000000) Num2Text = "UN BILLON";
            else if (value < 2000000000000) Num2Text = "UN BILLON " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            else
            {
                Num2Text = toText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }
            return Num2Text;

        }

        public string ToUpperFirstLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
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
