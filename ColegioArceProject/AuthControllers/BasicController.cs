using ColegioArceProject.Classes;
using ColegioArceProject.enums;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ColegioArceProject.AuthControllers
{
    public class BasicController : Controller
    {
        public BasicController() { }


        protected void SetMessage(String message, BootstrapAlertTypes type)
        {
            TempData[Configuration.AlertMagicString] = GetClassString(type);
            TempData[Configuration.MessageMagicString] = message;
        }
        protected void SetMessage(Dictionary<String, String> tempData)
        {
            TempData[Configuration.AlertMagicString] = tempData[Configuration.AlertMagicString];
            TempData[Configuration.MessageMagicString] = tempData[Configuration.MessageMagicString];
        }

      


        private String GetClassString(BootstrapAlertTypes type)
        {
            switch (type)
            {
                default:
                case BootstrapAlertTypes.Success: return "alert-success";
                case BootstrapAlertTypes.Info: return "alert-info";
                case BootstrapAlertTypes.Warning: return "alert-warning";
                case BootstrapAlertTypes.Danger: return "alert-danger";
            }

        }

        protected void SetAuthCookie(String email)
        {
            var authTicket =
            new FormsAuthenticationTicket(
                1,
                email,
                DateTime.Now,
                DateTime.Now.AddMonths(Configuration.CookieLifeTime),
                true,
                String.Empty
                );

            var encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
        }

        protected void SetCookie(String name, Object value, Boolean isPersistantCookie = true)
        {
            var cookie = new HttpCookie(name, value.ToString());
            if (isPersistantCookie) { cookie.Expires = DateTime.Now.AddMonths(Configuration.CookieLifeTime); }

            cookie.HttpOnly = true;
            cookie.Secure = HttpContext.Request.IsSecureConnection;

            Response.Cookies.Add(cookie);
        }

        protected void SetEncryptedCookie(String cookieName, Dictionary<String, String> values)
        {
            var cookie = new HttpCookie(cookieName);

            foreach (var item in values) { cookie.Values[item.Key] = EncryptionService.Encrypt(item.Value); }
            cookie.Expires = DateTime.Now.AddMonths(Configuration.CookieLifeTime);

            cookie.HttpOnly = true;
            cookie.Secure = HttpContext.Request.IsSecureConnection;

            Response.SetCookie(cookie);
        }
        protected void SetEncryptedCookie(String cookieName, String value)
        {
            var cookie = new HttpCookie(cookieName);

            cookie.HttpOnly = true;
            cookie.Secure = HttpContext.Request.IsSecureConnection;
            cookie.Value = EncryptionService.Encrypt(value);
            cookie.Expires = DateTime.Now.AddMonths(1);

            Response.SetCookie(cookie);
        }

        protected void RemoveCookie(String name)
        {
            var cookie = new HttpCookie(name, null);

            //cookie.Domain = HttpContext.Request.Url.Host;
            cookie.HttpOnly = true;
            cookie.Path = HttpContext.Request.ApplicationPath;
            cookie.Secure = HttpContext.Request.IsSecureConnection;

            cookie.Expires = new DateTime(2001, 09, 11);

            Response.Cookies.Add(cookie);
        }
        protected override ITempDataProvider CreateTempDataProvider()
        {
            //Cookies
            return new CookieTempDataProvider();

            //Session
            //return base.CreateTempDataProvider();
        }

    }
}