using ColegioArceProject.AuthControllers;
using ColegioArceProject.Classes;
using ColegioArceProject.enums;
using ColegioArceProject.Models;
using ColegioArceProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;

namespace ColegioArceProject.Controllers
{
    public class UsuarioController : UserAuthController
    {
        private ColegioArce_Entities db = new ColegioArce_Entities();

        // GET: Usuario
        public ActionResult Index()
        {
            var usuario = db.Usuario.Include(u => u.Rol);
            return View(usuario.ToList());
        }

        // GET: Usuario/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuario/Create
        public ActionResult Create()
        {
            ViewBag.id_Rol = new SelectList(db.Rol, "id_Rol", "Nombre");
            return View();
        }

        // POST: Usuario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CrearUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {

                ViewBag.id_Rol = new SelectList(db.Rol, "id_Rol", "Nombre", model.id_Rol);
                SetMessage("Ocurrió un error, por favor intente de nuevo.", BootstrapAlertTypes.Danger);
                return View(model);
            }

            var usuarioexiste = db.Usuario.SingleOrDefault(x => x.Correo.Equals(model.Email, System.StringComparison.OrdinalIgnoreCase));

            if (usuarioexiste != null)
            {
                SetMessage("Ya existe un usuario registrado con la dirección de correo proporcionada", BootstrapAlertTypes.Danger);
                return RedirectToAction("Index");
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                Correo = model.Email,
                Contra = model.Password,
                id_Rol = model.id_Rol
            };

            db.Usuario.Add(usuario);
            db.SaveChanges();
            SetMessage("El usuario fué creado.", BootstrapAlertTypes.Success);
            return RedirectToAction("Index");
        }

        // GET: Usuario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_Rol = new SelectList(db.Rol, "id_Rol", "Nombre", usuario.id_Rol);
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_Usuario,Nombre,Contra,Correo,id_Rol")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_Rol = new SelectList(db.Rol, "id_Rol", "Nombre", usuario.id_Rol);
            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuario.Find(id);
            db.Usuario.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetMessage("Datos inválidos.", BootstrapAlertTypes.Danger);
                return View(model);
            }

            //if (!WebSecurity.Login(model.Email, model.Password))
            //{
            //    return Redirect("/Usuario/Login");
            //}

            Usuario user = db.Usuario.Include(x => x.Rol).SingleOrDefault(x => x.Correo.Equals(model.Email, System.StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                SetMessage("No se encontró el usuario.", BootstrapAlertTypes.Danger);
                return Redirect("/Usuario/Login");
            }


            if (!model.Password.Equals(user.Contra, StringComparison.CurrentCulture))
            {
                SetMessage("La contraseña es incorrecta, intende de nuevo", BootstrapAlertTypes.Danger);
                return View(model);
            }


            SetAuthCookie(model.Email);

            SetEncryptedCookie(Configuration.UserCookie, new Dictionary<String, String>
            {
                { "Email", user.Correo },
                {"Name", user.Nombre},
                { "Code", "12365dasdasd"},
                { "Rol", user.Rol.Nombre}
            });

            //if (!String.IsNullOrEmpty(returnUrl))
            //{
            //    return Redirect(returnUrl);
            //}

            return Redirect("/");


        }

        public ActionResult LogOut()
        {
            if (Request.Cookies[Configuration.UserCookie] != null)
            {
                Request.Cookies[Configuration.UserCookie].Expires = DateTime.Now.AddDays(-1);
                Request.Cookies[Configuration.UserCookie].Value = string.Empty;
                Response.Cookies.Add(Request.Cookies[Configuration.UserCookie]);
                ViewBag.UserEmail = string.Empty;
                ViewBag.UserName = string.Empty;
                ViewBag.UserName = string.Empty;
                ViewBag.Rol = string.Empty;
            }
            Session.Abandon();

            FormsAuthentication.SignOut();
            return Redirect("/");
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
