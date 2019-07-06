using System.Linq;
using System.Net;
using ColegioArceProject.AuthControllers;

namespace ColegioArceProject.Controllers
{
    public class PrecioController : UserAuthController
    {
        private ColegioArce_Entities db = new ColegioArce_Entities();

        // GET: Precio
        public ActionResult Index()
        {
            var precio = db.Precio.Include(p => p.Rango);
            return View(precio.ToList());
        }

        // GET: Precio/Details/5
        public ActionResult Details(int? id)
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
            return View(precio);
        }

        // GET: Precio/Create
        public ActionResult Create()
        {
            ViewBag.id_Rango = new SelectList(db.Rango, "id_Rango", "Descripcion");
            return View();
        }

        // POST: Precio/Create
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
                return RedirectToAction("Index");
            }

            ViewBag.id_Rango = new SelectList(db.Rango, "id_Rango", "Descripcion", precio.id_Rango);
            return View(precio);
        }

        // GET: Precio/Edit/5
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
            return View(precio);
        }

        // POST: Precio/Edit/5
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
                return RedirectToAction("Index");
            }
            ViewBag.id_Rango = new SelectList(db.Rango, "id_Rango", "Descripcion", precio.id_Rango);
            return View(precio);
        }

        // GET: Precio/Delete/5
        public ActionResult Delete(int? id)
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
            return View(precio);
        }

        // POST: Precio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Precio precio = db.Precio.Find(id);
            db.Precio.Remove(precio);
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
