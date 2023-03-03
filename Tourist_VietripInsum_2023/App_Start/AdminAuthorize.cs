using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.App_Start
{
    public class AdminAuthorize: AuthorizeAttribute
    {
        public string idPos { set; get; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //ktra sesstion dang nhap=> thuc hien dang nhap
            //nguoc lai => trang dang nhap
            Staff nvSession = (Staff)HttpContext.Current.Session["user"];

            if(nvSession!=null)
            {
                return;
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult
                    (new RouteValueDictionary
                    (new { controller = "LoginStaff", action = "Login" }));
            }

            //check quyen

            TouristEntities1 db = new TouristEntities1();
            var count = db.Staffs.Count(m => m.IdStaff == nvSession.IdStaff & m.IdPos == idPos);

            if(count !=0)
            {
                return;
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }

            base.OnAuthorization(filterContext);
        }
    }
}