using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Buoi6_TestScriptPortal
{
    public class TestScript
    {
        private ChromeDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(300);
        }

        [TearDown]
        public void TearDown()
        {
            Thread.Sleep(1000);
            driver.Quit();
        }

        //test đăng nhập
        [Test]
        [TestCase("20dh112027", "Dk1912@2002!!", "Cổng thông tin đào tạo")] //Đúng
        [TestCase("20dh112027", "dfsdfsdf", "Đăng nhập")] //Sai
        public void DangNhap(string user, string pass, string expected)
        {
            driver.Navigate().GoToUrl("https://portal.huflit.edu.vn/Login");
            driver.FindElement(By.XPath("//input[@name='txtTaiKhoan']")).SendKeys(user);
            driver.FindElement(By.XPath("//input[@name='txtMatKhau']")).SendKeys(pass);
            driver.FindElement(By.XPath("//input[@value='Đăng nhập']")).Click();
            Assert.That(driver.Title, Is.EqualTo(expected));
        }

        //test đăng xuất
        [Test]
        public void DangXuat()
        {
            DangNhap("20dh112027", "Dk1912@2002!!", "Cổng thông tin đào tạo");
            driver.FindElement(By.XPath("//span[contains(text(),'20dh112027 | Nguyễn Thị Diễm Khang')]")).Click(); //hoặc lấy //span[@class='caret']
            driver.FindElement(By.XPath("//a[normalize-space()='Thoát']")).Click();
            Assert.That(driver.Title, Is.EqualTo("Đăng nhập"));
        }

        //test tìm kiếm môn học
        [Test]
        [TestCase("1250023")] //Đúng mã môn (Tiếng anh chuyên ngành 2)
        public void TraCuuThoiKhoaBieu_Success(string content)
        {
            driver.Navigate().GoToUrl("https://portal.huflit.edu.vn/public/tracuuthoikhoabieu");
            driver.FindElement(By.XPath("//input[@id='ddCurriculumId']")).SendKeys(content);
            driver.FindElement(By.XPath("//button[contains(text(),'Lọc')]")).Click();
            Thread.Sleep(2000);
            bool flag = driver.FindElement(By.CssSelector("div[class='container-fluid'] div span")).Displayed;
            // tìm thấy khi hiển thị span ngày giờ này
            Assert.That(flag, Is.EqualTo(true));
        }

        //test tìm kiếm mon học
        [Test]
        [TestCase("1250023223")] //Nhập sai mã môn
        [TestCase("")] //Không nhập mã môn (để trống)
        public void TraCuuThoiKhoaBieu_NotFound(string content)
        {
            driver.Navigate().GoToUrl("https://portal.huflit.edu.vn/public/tracuuthoikhoabieu");
            driver.FindElement(By.XPath("//input[@id='ddCurriculumId']")).SendKeys(content);
            driver.FindElement(By.XPath("//button[contains(text(),'Lọc')]")).Click();
            Thread.Sleep(2000);
            bool flag = driver.FindElement(By.XPath("//div[@class='alert alert-info']")).Displayed;
            // không tìm thấy thì sẽ hiển thị span có class "alert-info" này 
            Assert.That(flag, Is.EqualTo(true));
        }

        //test kích thước logo
        [Test]
        [TestCase("//a[contains(text(),'Chương trình đào tạo')]")]
        [TestCase("//a[contains(text(),'Lịch học')]")]
        [TestCase("//a[contains(text(),'Kết quả rèn luyện')]")]
        [TestCase("//a[contains(text(),'Đánh giá điểm rèn luyện')]")]
        [TestCase("//a[normalize-space()='Thông báo']")]
        [TestCase("//a[contains(text(),'Thông tin cá nhân')]")]
        [TestCase("//a[contains(text(),'Hướng dẫn sử dụng')]")] // trường hợp fail
        [TestCase("//a[normalize-space()='Tài chính sinh viên']")]
        public void CheckImage_LogoHead(string xPath) // vừa mới đăng nhập thành công và bấm vào menu chương trình đào tạo
        {
            DangNhap("20dh112027", "Dk1912@2002!!", "Cổng thông tin đào tạo");
            IWebElement element1 = driver.FindElement(By.XPath("//img[@src='/Content/logo/Logo-Portal.png']"));
            int hei1 = element1.Size.Height;
            int wid1 = element1.Size.Width;
            //
            driver.FindElement(By.XPath(xPath)).Click();
            IWebElement element2 = driver.FindElement(By.XPath("//img[@src='/Content/logo/Logo-Portal.png']"));
            int hei2 = element2.Size.Height;
            int wid2 = element2.Size.Width;
            //
            Assert.That(hei1 * wid1, Is.EqualTo(hei2 * wid2));
        }

        //test nội dung header
        [Test]
        [TestCase("//a[contains(text(),'Chương trình đào tạo')]")]
        [TestCase("//a[contains(text(),'Lịch học')]")]
        [TestCase("//a[contains(text(),'Kết quả rèn luyện')]")]
        [TestCase("//a[contains(text(),'Đánh giá điểm rèn luyện')]")]
        [TestCase("//a[normalize-space()='Thông báo']")]
        [TestCase("//a[contains(text(),'Thông tin cá nhân')]")]
        [TestCase("//a[normalize-space()='Tài chính sinh viên']")]
        public void Test_ContentHeader(string xPath)
        {
            DangNhap("20dh112027", "Dk1912@2002!!", "Cổng thông tin đào tạo");
            IWebElement element1 = driver.FindElement(By.XPath("//header[@id='header']"));
            string content1 = element1.Text;
            //
            driver.FindElement(By.XPath(xPath)).Click();
            IWebElement element2 = driver.FindElement(By.XPath("//header[@id='header']"));
            string content2 = element2.Text;
            //
            Assert.That(content1.Equals(content2));
        }

        //test nội dung menu nằm bên trái
        [Test]
        [TestCase("//a[contains(text(),'Chương trình đào tạo')]")]
        [TestCase("//a[contains(text(),'Lịch học')]")]
        [TestCase("//a[contains(text(),'Kết quả rèn luyện')]")]
        [TestCase("//a[contains(text(),'Đánh giá điểm rèn luyện')]")]
        [TestCase("//a[normalize-space()='Thông báo']")]
        [TestCase("//a[contains(text(),'Thông tin cá nhân')]")]
        [TestCase("//a[normalize-space()='Tài chính sinh viên']")]
        public void Test_ContentMenuLeftSide(string xPath)
        {
            DangNhap("20dh112027", "Dk1912@2002!!", "Cổng thông tin đào tạo");
            IWebElement element1 = driver.FindElement(By.XPath("//div[@class='col-md-3']"));
            string content1 = element1.Text;
            //
            driver.FindElement(By.XPath(xPath)).Click();
            IWebElement element2 = driver.FindElement(By.XPath("//div[@class='col-md-3']"));
            string content2 = element2.Text;
            //
            Assert.That(content1.Equals(content2));
        }

        //test nội dung footer
        [Test]
        [TestCase("//a[contains(text(),'Chương trình đào tạo')]")]
        [TestCase("//a[contains(text(),'Lịch học')]")]
        [TestCase("//a[contains(text(),'Kết quả rèn luyện')]")]
        [TestCase("//a[contains(text(),'Đánh giá điểm rèn luyện')]")]
        [TestCase("//a[normalize-space()='Thông báo']")]
        [TestCase("//a[contains(text(),'Thông tin cá nhân')]")]
        [TestCase("//a[contains(text(),'Hướng dẫn sử dụng')]")]
        [TestCase("//a[normalize-space()='Tài chính sinh viên']")]
        public void Test_ContentFooter(string xPath)
        {
            DangNhap("20dh112027", "Dk1912@2002!!", "Cổng thông tin đào tạo");
            IWebElement element1 = driver.FindElement(By.XPath("//footer[@class='footer bgfooterr']"));
            string content1 = element1.Text;
            //
            driver.FindElement(By.XPath(xPath)).Click();
            IWebElement element2 = driver.FindElement(By.XPath("//footer[@class='footer bgfooterr']"));
            string content2 = element2.Text;
            //
            Assert.That(content1.Equals(content2));
        }
    }
}
