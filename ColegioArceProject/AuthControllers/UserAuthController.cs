using ColegioArceProject.Classes;
using System;
using System.Web.Mvc;

namespace ColegioArceProject.AuthControllers
{
    public class UserAuthController : BasicController
    {
        public UserAuthController() { }
        protected String UserEmail
        {
            get
            {
                var httpCookie = Request.Cookies[Configuration.UserCookie];
                if (httpCookie != null)
                {
                    return EncryptionService.Decrypt(Request.Cookies[Configuration.UserCookie]["Email"]);
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        protected String UserName
        {
            get
            {
                var httpCookie = Request.Cookies[Configuration.UserCookie];
                if (httpCookie != null)
                {
                    return EncryptionService.Decrypt(Request.Cookies[Configuration.UserCookie]["Name"]);
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        protected String Code
        {
            get
            {
                var httpCookie = Request.Cookies[Configuration.UserCookie];
                if (httpCookie != null)
                {
                    return EncryptionService.Decrypt(Request.Cookies[Configuration.UserCookie]["Code"]);
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        protected String Rol
        {
            get
            {
                var httpCookie = Request.Cookies[Configuration.UserCookie];
                if (httpCookie != null)
                {
                    return EncryptionService.Decrypt(Request.Cookies[Configuration.UserCookie]["Rol"]);
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.UserEmail = UserEmail;
            ViewBag.UserName = UserName;
            ViewBag.UserName = Code;
            ViewBag.Rol = Rol;

            base.OnActionExecuting(filterContext);
        }
    }
}