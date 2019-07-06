using ColegioArceProject.AuthControllers;
using System.Web.Mvc;

namespace ColegioArceProject.Controllers
{
    public class HomeController : UserAuthController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}