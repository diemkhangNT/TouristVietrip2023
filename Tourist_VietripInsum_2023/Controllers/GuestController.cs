using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    public class GuestController : Controller
    {
        TouristEntities1 db = new TouristEntities1();
        // GET: Guest
        public ActionResult HomePageGuest()
        {
            return View();
        }

        public ActionResult Tourinfomation(string id)
        {
            return View(db.Tours.Where(s => s.MaLTour == id).FirstOrDefault());
        }
    }
}