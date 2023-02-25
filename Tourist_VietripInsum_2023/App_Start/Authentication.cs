using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.App_Start
{
    public class Authentication : AuthorizeAttribute
    {
        public string IdPos { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //1.Check session  : Đã đăng nhập vào hệ thống = > cho thực hiện filterContext
            //Ngược lại thì cho trở lại trang đăng nhập 
            Staff nvSession = (Staff)HttpContext.Current.Session["user"];
            if (nvSession == null)
            {

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "LoginStaff", action = "Login" }));
            }
            else
            {
                #region 2. Check quyền : Có quyền thì cho thực hiện filterContext
                //Ngược lại thì cho trở lại trang đăng nhập  = > Trang từ chối truy cập
                TouristEntities1 db = new TouristEntities1();
                var count = db.Staffs.Count(m => m.IdPos == nvSession.IdPos && m.IdPos == IdPos);
                if (count != 0)
                {
                    return;
                }
                else
                {
                    var returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(
                        new
                        {
                            Controller = "Home",
                            action = "Index",
                            returnUrl = returnUrl.ToString()
                        }));
                }
                #endregion

            }
            return;

        }
    }
}
