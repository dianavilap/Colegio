using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ColegioArceProject.AuthControllers;
using ColegioArceProject.Models;
using ColegioArceProject.ViewModels;

namespace ColegioArceProject.Controllers
{
    [Authorize]
    public class EscolaridadController : UserAuthController
    {
        private ColegioArce_Entities db = new ColegioArce_Entities();

        // GET: Escolaridad
        public ActionResult Index()
        {
            return View(db.Escolaridad.ToList());
        }

        // GET: Escolaridad/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Escolaridad escolaridad = db.Escolaridad.Find(id);
            if (escolaridad == null)
            {
                return HttpNotFound();
            }
            return View(escolaridad);
        }

        // GET: Escolaridad/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Escolaridad/Create [Bind(Include = "id_escolaridad,Descripcion")] Escolaridad escolaridad
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EscolaridadViewModel Escolaridad)
        {
            var EscolaridadDB = new Escolaridad();

            if (ModelState.IsValid)
            {
                EscolaridadDB.Descripcion = Escolaridad.Descripcion;
                EscolaridadDB.No_Peridos = Escolaridad.No_periodos;
            
                db.Escolaridad.Add(EscolaridadDB);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(EscolaridadDB);
        }

        // GET: Escolaridad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Escolaridad escolaridad = db.Escolaridad.Find(id);
            if (escolaridad == null)
            {
                return HttpNotFound();
            }
            return View(escolaridad);
        }

        // POST: Escolaridad/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Escolaridad escolaridad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(escolaridad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(escolaridad);
        }

        // GET: Escolaridad/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Escolaridad escolaridad = db.Escolaridad.Find(id);
            if (escolaridad == null)
            {
                return HttpNotFound();
            }
            return View(escolaridad);
        }

        // POST: Escolaridad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Escolaridad escolaridad = db.Escolaridad.Find(id);
            db.Escolaridad.Remove(escolaridad);
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
