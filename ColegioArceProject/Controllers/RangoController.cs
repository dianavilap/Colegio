using ColegioArceProject.AuthControllers;
using ColegioArceProject.enums;
using ColegioArceProject.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ColegioArceProject.Controllers
{
    [Authorize]
    public class RangoController : UserAuthController
    {
        private ColegioArce_Entities db = new ColegioArce_Entities();

        // GET: Rango
        public ActionResult Index()
        {
            return View(db.Rango.ToList());
        }

        // GET: Rango/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                SetMessage("No se encontró el rango, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }
            Rango rango = db.Rango.Find(id);
            if (rango == null)
            {
                SetMessage("No se encontró el rango, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }
            return View(rango);
        }



        // GET: Rango/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rango/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_Rango,Inicio,Fin,Descripcion")] Rango rango)
        {
            if (ModelState.IsValid)
            {
                db.Rango.Add(rango);
                db.SaveChanges();
                SetMessage("El rango de fecha fue creado.", BootstrapAlertTypes.Success);
                return RedirectToAction("Index");
            }

            return View(rango);
        }


        public ActionResult imprimir(int id)
        {

            var student = db.Alumno.SingleOrDefault(x => x.id_Alumno == id);

            var memoryStream = new MemoryStream();
            crearRecibo(memoryStream, student);
            return File(memoryStream.GetBuffer(), "application/pdf", "RegistroCSS.pdf");
        }

        public void crearRecibo(Stream output, Alumno student)
        {



            var documento = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(documento, output);
            documento.Open();
            var fontTitle = new Font(Font.FontFamily.HELVETICA, 26, 2);
            var fontParagraph = new Font(Font.FontFamily.HELVETICA, 12, 2);

            documento.AddHeader(student.Nombre, student.Nombre);
            documento.Add(Chunk.NEWLINE);
            documento.Add(Chunk.NEWLINE);
            documento.Add(new Paragraph("Recibo de pago - Colegio Arce", fontTitle));
            documento.Add(new Paragraph(DateTime.Now.ToString("dd/mm/yyyy"), fontParagraph));
            documento.Add(Chunk.NEWLINE);

            documento.Close();
            writer.Close();
        }


        // GET: Rango/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                SetMessage("No se encontró el rango, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }
            Rango rango = db.Rango.Find(id);
            if (rango == null)
            {
                SetMessage("No se encontró el rango, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }
            return View(rango);
        }

        // POST: Rango/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_Rango,Inicio,Fin,Descripcion")] Rango rango)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rango).State = EntityState.Modified;
                db.SaveChanges();
                SetMessage("Se editó el rango.", BootstrapAlertTypes.Success);
                return RedirectToAction("Index");
            }
            SetMessage("Ocurrió un error, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
            return View(rango);
        }

        // GET: Rango/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                SetMessage("No se encontró el rango, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }
            Rango rango = db.Rango.Find(id);
            if (rango == null)
            {
                SetMessage("No se encontró el rango, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }
            return View(rango);
        }

        // POST: Rango/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rango rango = db.Rango.Find(id);
            db.Rango.Remove(rango);
            db.SaveChanges();
            SetMessage("Se eliminó el rango.", BootstrapAlertTypes.Success);
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
