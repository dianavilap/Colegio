using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ColegioArceProject.AuthControllers;
using ColegioArceProject.Models;
using ColegioArceProject.ViewModels;

namespace ColegioArceProject.Controllers
{
    [Authorize]
    public class GrupoController : UserAuthController
    {
        private ColegioArce_Entities db = new ColegioArce_Entities();

        // GET: Grupo
        public ActionResult Index()
        {
            var grupodb = db.Grupo
                .Where(x => x.Activo == true)
                .Include(g => g.Escolaridad);

            var grupolist = new List<GrupoViewModel>();

            foreach (var grupo in grupodb)
            {
                grupolist.Add(new GrupoViewModel {
                    id_grupo = grupo.id_Grupo,
                    Descripcion = grupo.Descripcion,
                    DescripcionEscolaridad = grupo.Escolaridad.Descripcion,
                    id_escolaridad = grupo.id_Escolaridad,
                    Activo = grupo.Activo,
                });
            }
        
            
                return View(grupolist);
        }

        // GET: Grupo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grupo grupobd = db.Grupo.Include(x => x.Escolaridad).SingleOrDefault(s => s.id_Grupo == id);
            if (grupobd == null)
            {
                return HttpNotFound();
            }
            var grupovm = new GrupoViewModel();

            grupovm.id_grupo = grupobd.id_Grupo;
            grupovm.Descripcion = grupobd.Descripcion;
            grupovm.DescripcionEscolaridad = grupobd.Escolaridad.Descripcion;
            grupovm.id_escolaridad = grupobd.id_Escolaridad;
            grupovm.Activo = grupobd.Activo;

            return View(grupovm);
        }

        //ViewBag.id_Alumno = new SelectList(db.Alumno, "id_Alumno", "Nombre");
        //ViewBag.id_Escolaridad = new SelectList(db.Escolaridad, "id_escolaridad", "Descripcion");

        // GET: Grupo/Create
        public ActionResult Create()
        {
            GrupoViewModel grupo = new GrupoViewModel();

            using (db = new ColegioArce_Entities())
            {
                grupo.EscolaridadList = db.Escolaridad.ToList();
            }
            return View(grupo);
        }

        // POST: Grupo/Create [Bind(Include = "id_Grupo,Descripcion,id_Escolaridad,id_Alumno")] Grupo grupo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GrupoViewModel grupo)
        {
            var GrupoBD = new Grupo();

            if (ModelState.IsValid)
            {
                GrupoBD.Descripcion = grupo.Descripcion;
                GrupoBD.id_Escolaridad = grupo.id_escolaridad;
                GrupoBD.Activo = true;
                
                db.Grupo.Add(GrupoBD);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            using (db = new ColegioArce_Entities())
            {
                grupo.EscolaridadList = db.Escolaridad.ToList();
            }

            //ViewBag.id_Alumno = new SelectList(db.Alumno, "id_Alumno", "Nombre", grupo.id_Alumno);
            //ViewBag.id_Escolaridad = new SelectList(db.Escolaridad, "id_escolaridad", "Descripcion", grupo.id_Escolaridad);
            return View(grupo);
        }

        // GET: Grupo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grupo grupobd = db.Grupo.Include(x => x.Escolaridad).SingleOrDefault(s => s.id_Grupo == id);
            if (grupobd == null)
            {
                return HttpNotFound();
            }
            var grupovm = new GrupoViewModel();
            grupovm.id_grupo = grupobd.id_Grupo;
            grupovm.Descripcion = grupobd.Descripcion;
            grupovm.DescripcionEscolaridad = grupobd.Escolaridad.Descripcion;
            grupovm.id_escolaridad = grupobd.id_Escolaridad;
            grupovm.Activo = grupobd.Activo;
            using (db = new ColegioArce_Entities())
            {
                grupovm.EscolaridadList = db.Escolaridad.ToList();
            }
            //ViewBag.id_Escolaridad = new SelectList(db.Escolaridad, "id_escolaridad", "Descripcion", grupo.id_Escolaridad);
            return View(grupovm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GrupoViewModel grupo)
        {
            if (ModelState.IsValid)
            {
                var grupobd = new Grupo();

                grupobd.id_Grupo = grupo.id_grupo;
                grupobd.Descripcion = grupo.Descripcion;
                grupobd.id_Escolaridad = grupo.id_escolaridad;
                grupobd.Activo = grupo.Activo;

                db.Entry(grupobd).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.id_Alumno = new SelectList(db.Alumno, "id_Alumno", "Nombre", grupo.id_Alumno);
            //ViewBag.id_Escolaridad = new SelectList(db.Escolaridad, "id_escolaridad", "Descripcion", grupo.id_Escolaridad);
            return View(grupo);
        }

        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grupo grupobd = db.Grupo.Include(x => x.Escolaridad).SingleOrDefault(s => s.id_Grupo == id);
            if (grupobd == null)
            {
                return HttpNotFound();
            }
            var grupovm = new GrupoViewModel();
            grupovm.id_grupo = grupobd.id_Grupo;
            grupovm.Descripcion = grupobd.Descripcion;
            grupovm.DescripcionEscolaridad = grupobd.Escolaridad.Descripcion;
            grupovm.id_escolaridad = grupobd.Escolaridad.id_escolaridad;
            grupovm.Activo = grupobd.Activo;
            return View(grupovm);
        }

        // POST: Grupo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Grupo grupo = db.Grupo.Find(id);
            

            grupo.Activo = false;
            db.Entry(grupo).State = EntityState.Modified;
            db.SaveChanges();

            var Alumnos = db.GrupoAlumno.Include(x => x.Alumno).Where(x => x.id_Grupo == id).Select(x => x.Alumno).ToList();
            foreach (Alumno Alum in Alumnos)
            {
                Alum.Activo = false;
                db.Entry(Alum).State = EntityState.Modified;
                db.SaveChanges();
            }
            
           
            //db.Grupo.Remove(grupo);
            //db.SaveChanges();
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
