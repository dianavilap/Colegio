using ColegioArceProject.AuthControllers;
using ColegioArceProject.enums;
using ColegioArceProject.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ColegioArceProject.Controllers
{
    [Authorize]
    public class PreciosController : UserAuthController
    {
        private ColegioArce_Entities db = new ColegioArce_Entities();

        // GET: Precios
        public ActionResult Index()
        {
            var precio = db.Precio.Include(p => p.Rango).Include(p => p.Escolaridad).ToList();
            return View(precio);
        }

        // GET: Precios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                SetMessage("No se encontró el precio, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }
            Precio precio = db.Precio.Find(id);
            if (precio == null)
            {
                SetMessage("No se encontró el precio, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }
            return View(precio);
        }

        // GET: Precios/Create
        public ActionResult Create()
        {
            ViewBag.id_Rango = new SelectList(db.Rango, "id_Rango", "Descripcion");
            ViewBag.id_Escolaridad = new SelectList(db.Escolaridad, "id_escolaridad", "Descripcion");
            return View();
        }

        // POST: Precios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_Precio,Concepto,Importe,id_Rango,id_Escolaridad")] Precio precio)
        {
            if (ModelState.IsValid)
            {
                db.Precio.Add(precio);
                db.SaveChanges();
                SetMessage("El precio de colegiatura fue creado.", BootstrapAlertTypes.Success);
                return RedirectToAction("Index");
            }


            ViewBag.id_Rango = new SelectList(db.Rango, "id_Rango", "Descripcion", precio.id_Rango);
            ViewBag.id_Escolaridad = new SelectList(db.Escolaridad, "id_escolaridad", "Descripcion", precio.id_Escolaridad);
            SetMessage("Ocurrio un error, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
            return View(precio);
        }

        // GET: Precios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Precio precio = db.Precio.Find(id);
            if (precio == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_Rango = new SelectList(db.Rango, "id_Rango", "Descripcion", precio.id_Rango);
            ViewBag.id_Escolaridad = new SelectList(db.Escolaridad, "id_escolaridad", "Descripcion", precio.id_Escolaridad);
            return View(precio);
        }

        // POST: Precios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_Precio,Concepto,Importe,id_Rango,id_Escolaridad")] Precio precio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(precio).State = EntityState.Modified;
                db.SaveChanges();
                SetMessage("El precio fue actualizado con exito.", BootstrapAlertTypes.Success);
                return RedirectToAction("Index");
            }

            SetMessage("Ocurrio un error, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
            ViewBag.id_Rango = new SelectList(db.Rango, "id_Rango", "Descripcion", precio.id_Rango);
            ViewBag.id_Escolaridad = new SelectList(db.Escolaridad, "id_escolaridad", "Descripcion", precio.id_Escolaridad);
            return View(precio);
        }

        // GET: Precios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Precio precio = db.Precio.Find(id);
            if (precio == null)
            {
                SetMessage("Ocurrio un error, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }
            return View(precio);
        }

        // POST: Precios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Precio precio = db.Precio.Find(id);
            db.Precio.Remove(precio);
            db.SaveChanges();
            SetMessage("El precio fue eliminado con exito.", BootstrapAlertTypes.Success);
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
