using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InspectionsApp.Models;

namespace InspectionsApp.Controllers
{
    public class HomeController : Controller
    {
        private db_a9ffb8_murphysourceEntities db = new db_a9ffb8_murphysourceEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Users()
        {
            List<Users> users = db.Users.ToList();

            return View(users);
        }
    }
}