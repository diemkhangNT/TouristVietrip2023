using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tourist_VietripInsum_2023.App_Start;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    [AdminAuthorize(idPos = "TM")]
    public class OrderProcessingController : Controller
    {
        TouristEntities1 db = new TouristEntities1();

        public ActionResult HomePageOP()
        {
            var donhang = db.BookTours.Count();
            TempData["TongDonDat"] = donhang;

            var dsDonHangMoi = db.BookTours.OrderByDescending(dhang => dhang.NgayLap).Take(5).ToList();
            return View(dsDonHangMoi);
        }
        
        public ActionResult ManageTourOrders(string search)
        {
            if (search == null)
            {
                return View(db.BookTours.ToList());
            }
            else
            {
                ViewBag.SearchInfo = search;
                return View(db.BookTours.Where(s => s.MaDH.ToLower().Contains(search) == true || s.SdtKH.ToLower().Contains(search) == true).ToList());
            }
        }

        public ActionResult ListOfCustomers(string search)
        {
            if (search == null)
            {
                return View(db.KhachHangs.ToList());
            }
            else
            {
                return View(db.KhachHangs.Where(s => s.MaKH.ToLower().Contains(search) == true || s.SDT.ToLower().Contains(search) == true || s.HoTenKH.ToLower().Contains(search) == true).ToList());
            }
        }

        public ActionResult CheckCusInfo(string search)
        {
            var info = db.KhachHangs.FirstOrDefault(s => s.SDT == search);
            if (search == null)
            {
                return View();
            }
            else
            {
                if (info == null)
                {
                    ViewBag.Info = info;
                    ViewBag.SDT = search;
                    Session["SDT"] = search;
                    return View();
                }
                else
                {
                    ViewBag.Info = info;
                    ViewBag.SDT = search;
                    Session["SDT"] = search;
                    return View(info);
                }
            }
        }
        [HttpPost, ActionName("CheckCusInfo")]
        public ActionResult CreateCustomer([Bind(Include = "MaKH,SDT,MaLoaiKH,Username,UserPassword,HoTenKH,DiaChi,Email,NgaySinh,GioiTinh,HinhDaiDien")] KhachHang khachHang)
        {
            Random random = new Random();
            var idCus = "GS" + random.Next(1, 1000);
            
            if (ModelState.IsValid)
            {
                khachHang.MaKH = idCus;
                khachHang.MaLoaiKH = null;
                khachHang.Username = null;
                khachHang.UserPassword = null;
                khachHang.HinhDaiDien = null;
                db.KhachHangs.Add(khachHang);
                db.SaveChanges();
            }
            return RedirectToAction("CreateOrder");
        }

        [HttpGet]
        public ActionResult CreateOrder()
        {
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "TenTour");
            return View();
        }
        [HttpPost]
        public ActionResult CreateOrder(BookTour donHang)
        {
            Random random = new Random();
            var maDH = "DH" + random.Next(1, 1000);
            if (ModelState.IsValid)
            {
                if (donHang.SoCho < 1)
                {
                    TempData["ErrorSoCho"] = "Số chỗ phải lớn hơn 1!!!";
                    return RedirectToAction("CreateOrder");
                }
                else
                {
                    donHang.MaDH = maDH;
                    Session["MaDH"] = donHang.MaDH;
                    donHang.MaNVLap = "NV178";
                    donHang.NgayLap = System.DateTime.Now;
                    donHang.TrangThaiTT = true;
                    donHang.TotalPrice = null;

                    var sdt = Session["SDT"].ToString();
                    var info = db.KhachHangs.FirstOrDefault(s => s.SDT == sdt);
                    
                    donHang.SdtKH = sdt;
                    donHang.MaKH = info.MaKH;
                    db.BookTours.Add(donHang);
                    db.SaveChanges();
                    Session["SoCho"] = donHang.SoCho;
                }
            }
            return RedirectToAction("CreateTickets");
        }

        [HttpGet]
        public ActionResult CreateTickets()
        {
            ViewBag.MaLVe = new SelectList(db.LoaiVes, "MaLoaiVe", "TenLVe");
            return View();
        }
        [HttpPost]
        public ActionResult CreateTickets(Ve ve)
        {
            var infoCus = Session["SDT"].ToString();

            var booking = db.BookTours.FirstOrDefault(s => s.SdtKH == infoCus);
            var tour = db.Tours.FirstOrDefault(s => s.MaTour == booking.MaTour);

            Random random = new Random();
            int socho = (int)Session["SoCho"];

            double tongtien = 0;
            if (ModelState.IsValid)
            {
                for (int i = 0; i < socho; i++)
                {
                    ve = new Ve();
                    var maVe = "V" + random.Next(1, 1000);
                    ve.MaVe = maVe;
                    ve.MaDH = Session["MaDH"].ToString();
                    ve.Hoten_KH = Request["HoTenKH_" + i];
                    ve.MaLVe = Request["MaLVe_" + i];
                    //ve.MaLVe = "TICKET01";
                    ve.GioiTinh = Request["GioiTinh_" + i];
                    ve.NgaySinh = Convert.ToDateTime(Request["NgaySinh_" + i]);
                    ve.LuuY = Request["LuuY" + i];
                    db.Ves.Add(ve);

                    if (ve.MaLVe == "TICKET01")
                    {
                        tongtien = tongtien + (double)tour.GiaTreEm;
                    }
                    else if (ve.MaLVe == "TICKET02")
                    {
                        tongtien = tongtien + (double)tour.GiaNguoiLon;
                    }    
                    db.SaveChanges();
                }
                var updateBT = db.BookTours.Find(ve.MaDH);
                updateBT.TotalPrice = (decimal)tongtien;
                db.SaveChanges();
            }
            TempData["Success"] = "Book tour thành công";
            return RedirectToAction("OrderingInfo");
        }

        public ActionResult OrderingInfo()
        {
            BookTour infoOrder = db.BookTours.Find(Session["MaDH"]);
            //var infoOrder = db.BookTours.FirstOrDefault(s => s.MaDH == Session["MaDH"].ToString());
            return View(infoOrder);
        }

        [HttpGet]
        public ActionResult EditOrdering(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookTour bookTour = db.BookTours.Find(id);
            if (bookTour == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "TenTour", bookTour.MaTour);
            return View(bookTour);
        }
        [HttpPost]
        public ActionResult EditOrdering(BookTour donHang)
        {
            if (ModelState.IsValid)
            {
                if (donHang.SoCho < 1)
                {
                    TempData["ErrorSoCho"] = "Số chỗ phải lớn hơn 1!!!";
                    return RedirectToAction("EditOrdering");
                }
                else
                {
                    db.Entry(donHang).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("CreateTickets");
                }
            }
            return View();
        }

        public ActionResult DeleteOrdering(string id)
        {
            var ve = db.Ves.Where(s => s.MaDH == id).ToList();
            db.Ves.RemoveRange(ve);
            db.SaveChanges();

            BookTour donHang = db.BookTours.Find(id);
            db.BookTours.Remove(donHang);
            db.SaveChanges();
            return RedirectToAction("CheckCusInfo");
        }

        public ActionResult BookTourDetail(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BookTour bookTour = db.BookTours.Find(id);
            Session["MaBT"] = bookTour.MaDH;

            if (id == null)
            {
                return HttpNotFound();
            }
            return View(bookTour);
        }

        [HttpGet]
        public ActionResult TicketEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ve ve = db.Ves.Find(id);
            if (ve == null)
            {
                return HttpNotFound();
            }
            return View(ve);          
        }
        [HttpPost]
        public ActionResult TicketEdit(Ve ve)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ve).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Sửa vé thành công";
                return RedirectToAction("TicketEdit"); 
            }
            return View();
        }

        public ActionResult DeleteOrder(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookTour donHang = db.BookTours.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }
        [HttpPost, ActionName("DeleteOrder")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var ve = db.Ves.Where(s => s.MaDH == id).ToList();
            db.Ves.RemoveRange(ve);
            db.SaveChanges();

            BookTour donHang = db.BookTours.Find(id);
            db.BookTours.Remove(donHang);
            db.SaveChanges();
            TempData["deletesuccess"] = "Xóa đơn đặt thành công";
            return RedirectToAction("ManageTourOrders");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult FeedBack()
        {
            return View(db.PhanHois.ToList());
        }

        public ActionResult Profile(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien staff = db.NhanViens.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }
    }
}