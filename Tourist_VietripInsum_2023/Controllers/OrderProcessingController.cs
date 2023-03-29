using System;
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
    [AdminAuthorize(idPos = "OP")]
    public class OrderProcessingController : Controller
    {
        TouristEntities1 db = new TouristEntities1();

        public ActionResult HomePageOP()
        {
            var donhang = db.BookTours.Count();
            TempData["TongDonDat"] = donhang;
            var ph = db.PhanHois.Count();
            TempData["phanhoi"] = ph;
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

        public ActionResult TourList()
        {
            List<Tour> listTour = db.Tours.Include(t => t.LoaiTour).ToList();
            return View(listTour);
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

            var checkMail = db.KhachHangs.FirstOrDefault(s => s.Email == khachHang.Email);
           
            if (checkMail != null)
            {
                ModelState.AddModelError(string.Empty, "Đã có email trong hệ thống !!!");
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(khachHang.HoTenKH) == true ||
                    khachHang.NgaySinh == null ||
                    string.IsNullOrEmpty(khachHang.DiaChi) == true ||
                    string.IsNullOrEmpty(khachHang.Email) == true)
                {
                    TempData["trong"] = "Vui lòng nhập đầy đủ thông tin";
                    return View();
                } 
                else
                {
                    khachHang.MaKH = idCus;
                    khachHang.MaLoaiKH = null;
                    khachHang.Username = null;
                    khachHang.UserPassword = null;
                    khachHang.HinhDaiDien = null;
                    db.KhachHangs.Add(khachHang);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("CreateOrder");
        }

        public JsonResult CheckEmailAvailability(string mail)
        {
            System.Threading.Thread.Sleep(200);

            var mailCus = db.KhachHangs.Where(x => x.Email == mail).SingleOrDefault();
            if (mailCus != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
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
            var user = (Tourist_VietripInsum_2023.Models.NhanVien)HttpContext.Session["user"];
            if (ModelState.IsValid)
            {
                if (donHang.SoCho < 1 || donHang.SoCho == null)
                {
                    TempData["ErrorSoCho"] = "Số chỗ phải lớn hơn 1!!!";
                    return RedirectToAction("CreateOrder");
                }
                var tour = db.Tours.Find(donHang.MaTour);
                if (donHang.SoCho > tour.SoChoNull)
                {
                    TempData["ErrorSoChoMax"] = "Số chỗ vượt quá số chỗ còn trống";
                    return RedirectToAction("CreateOrder");
                }
                else
                {
                    donHang.MaDH = maDH;
                    Session["MaDH"] = donHang.MaDH;
                    donHang.MaNVLap = user.MaNV;
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
            var maDH = Session["MaDH"].ToString();

            //Tìm kiếm tour
            var booking = db.BookTours.FirstOrDefault(s => s.MaDH == maDH);
            var tour = db.Tours.FirstOrDefault(s => s.MaTour == booking.MaTour);

            Random random = new Random();
            int socho = (int)Session["SoCho"];

            double tongtien = 0;
            if (ModelState.IsValid)
            {
                //Tạo vé theo số lượng khách đặt
                for (int i = 0; i < socho; i++)
                {
                    if (string.IsNullOrEmpty(Request["HoTenKH_" + i]) == true ||
                        string.IsNullOrEmpty(Request["GioiTinh_" + i]) == true ||
                        string.IsNullOrEmpty(Request["NgaySinh_" + i]) == true)
                    {
                        TempData["errorTrong" + i] = "Vui lòng nhập đầy đủ thông tin";
                        return View();
                    }
                    else
                    {

                        ve = new Ve();
                        var maVe = "V" + random.Next(1, 1000);
                        ve.MaVe = maVe;
                        ve.MaDH = maDH;
                        ve.Hoten_KH = Request["HoTenKH_" + i];
                        ve.MaLVe = Request["MaLVe_" + i];
                        ve.GioiTinh = Request["GioiTinh_" + i];
                        ve.NgaySinh = Convert.ToDateTime(Request["NgaySinh_" + i]);
                        ve.LuuY = Request["LuuY" + i];
                        db.Ves.Add(ve);

                        if (ve.MaLVe == "TICKET01")
                        {
                            tongtien = tongtien + (int)tour.GiaNguoiLon;
                        }
                        else if (ve.MaLVe == "TICKET02")
                        {
                            tongtien = tongtien + (int)tour.GiaTreEm;
                        }
                        db.SaveChanges();
                    }
                }
                //Update tổng tiền cho đơn đặt tour
                var updateBT = db.BookTours.Find(ve.MaDH);
                updateBT.TotalPrice = (decimal)tongtien;
                //Update số chỗ trong tour
                tour.SoChoNull -= booking.SoCho;
                db.SaveChanges();
                return RedirectToAction("OrderingInfo");
            }
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
                var tour = db.Tours.Find(donHang.MaTour);
                if (donHang.SoCho > tour.SoChoNull)
                {
                    TempData["ErrorSoChoMax"] = "Số chỗ vượt quá số chỗ còn trống";
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

            var donHang = db.BookTours.Find(id);
            var tour = db.Tours.Find(donHang.MaTour);
            tour.SoChoNull += donHang.SoCho;
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
            TempData["MaBT"] = bookTour.MaDH;

            if (id == null)
            {
                return HttpNotFound();
            }
            return View(bookTour);
        }
        [HttpPost]
        public ActionResult BookTourDetail(BookTour bookTour)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookTour).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Cập nhật trạng thái thành công";
                return RedirectToAction("BookTourDetail");
            }
            return View();
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
                return RedirectToAction("BookTourDetail/" + Session["MaBT"]); 
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
            var tour = db.Tours.Find(donHang.MaTour);
            tour.SoChoNull += donHang.SoCho;
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

        public ActionResult DeleteFeedBack(string id)
        {
            PhanHoi phanHoi = db.PhanHois.Find(id);
            db.PhanHois.Remove(phanHoi);
            db.SaveChanges();
            return RedirectToAction("FeedBack");
        }

        public ActionResult FeedBackEdit(string id, PhanHoi phanHoi)
        {
            TempData["maFB"] = id;
            //PhanHoi phanHoi = db.PhanHois.Find(id);
            if (ModelState.IsValid)
            {
                db.Entry(phanHoi).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Cập nhật phản hồi thành công";
                return RedirectToAction("FeedBack");
            }
            return RedirectToAction("FeedBack");
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