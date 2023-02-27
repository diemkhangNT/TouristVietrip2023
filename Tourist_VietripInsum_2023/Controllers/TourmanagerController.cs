using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Net;
using System.IO;
using Tourist_VietripInsum_2023.App_Start;

namespace Tourist_VietripInsum_2023.Controllers
{
    [AdminAuthorize (idPos = "TM")]
    public class TourmanagerController : Controller
    {
        // GET: Tourmanager
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult HomePageTM()
        {
            return View();
        }
    }
}