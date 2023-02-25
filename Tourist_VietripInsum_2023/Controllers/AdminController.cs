using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        TouristEntities1 database = new TouristEntities1();
        public ActionResult HomePage()
        {
            return View(database.Staffs.ToList());
        }
    }
}