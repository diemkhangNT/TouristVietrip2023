//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Tourist_VietripInsum_2023.Models;
//using PagedList;


//namespace TrySearching.Controllers
//{
//    public class HomeController : Controller
//    {
//        StudentsDbContext db = new StudentsDbContext();
//        public ActionResult Index(int? pageNumber)
//        {
//            if (pageNumber == null) pageNumber = 1;
//            var StudentList = (from l in db.Students
//                               select l).OrderBy(x => x.Id);
//            int pageSize = 5;
//            //pass the StudentList list object to the view.  
//            return View(StudentList.ToPagedList((int)pageNumber, pageSize));
//        }



//        //the first parameter is the option that we choose and the second parameter will use the textbox value  
//        [HttpPost]
//        public ActionResult Index(string option, string search, int? pageNumber)
//        {

//            //if a user choose the radio button option as Subject  
//            if (option == "Subjects")
//            {
//                //Index action method will return a view with a student records based on what a user specify the value in textbox  
//                return View(db.Students.Where(x => x.Subjects.StartsWith(search) || x.Subjects.EndsWith(search) || search == null).ToList().ToPagedList(pageNumber ?? 1, 5));
//            }
//            else if (option == "Gender")
//            {
//                return View(db.Students.Where(x => x.Gender.StartsWith(search) || search == null).ToList().ToPagedList(pageNumber ?? 1, 5));
//            }
//            else
//            {
//                return View(db.Students.Where(x => x.Name.StartsWith(search) || search == null).ToList().ToPagedList(pageNumber ?? 1, 5));
//            }
//        }

//        public ActionResult Details(int id)
//        {
//            return View();
//        }





//        public ActionResult About()
//        {
//            ViewBag.Message = "Your application description page.";

//            return View();
//        }

//        public ActionResult Contact()
//        {
//            ViewBag.Message = "Your contact page.";

//            return View();
//        }
//    }
//}