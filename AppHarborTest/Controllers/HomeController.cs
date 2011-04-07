using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook.Web;
using Facebook.Web.Mvc;

namespace AppHarborTest.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [FacebookAuthorize(LoginUrl = "/Account/Login")]
        public ActionResult Profile()
        {
            var client = new FacebookWebClient();

            dynamic me = client.Get("me");
            ViewBag.Name = me.name;
            ViewBag.Id = me.id;

            return View();
        }
    }
}
