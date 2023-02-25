using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.App_Start
{
    public class Myauthentication : AuthorizeAttribute 
    {
        public string idStaff { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //check session: da dang nhap=> chay filter
            // neu khong dang nhap => quay lai login
            Staff staffSession = (Staff)HttpContext.Current.Session["user"];
            if(staffSession!=null)
            {
                //dang nhap=>check quyen
                TouristEntities1 db = new TouristEntities1();
                var count = db.Staffs.Count(s => s.IdStaff == staffSession.IdStaff & s.IdPos == staffSession.IdPos);
                if (count != 0)
                {
                    //co quyen
                    return;

                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "LoginStaff",
                                action = "Login"

                            }
                            ));
                }
                return;
                //thoat khoi ham
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "LoginStaff",
                            action = "Login"

                        }
                        ));
            }
        
           
        }
    }
}