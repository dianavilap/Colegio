using System.Web.Mvc;
using System.Web.Routing;

namespace ColegioArceProject
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();


            routes.MapRoute(
                name: "VerificarPago",
                url: "Pago/VerificarPago/{idPago}",
                defaults: new { controller = "Pago", action = "VerificarPago", idPago = UrlParameter.Optional }
            );

  


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
