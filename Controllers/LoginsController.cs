using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AgileRap_Process2.Data;
using AgileRap_Process2.Models;
using System.Text.Json;
using NuGet.Packaging;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Http;
using BCrypt;

namespace AgileRap_Process2.Controllers
{
    public class LoginsController : BaseController
    {
        //AgileRap_Process2Context db = new AgileRap_Process2Context(); //dbContext
 
        // GET: LoginsController
        public ActionResult Login() //แสดงหน้า Login
        {

            if (HttpContext.Session.GetString("UserEmailSession") != null)
            {
                return RedirectToAction("Index", "Works");
            }
            User user = new User();
            return View(user);
        }

        // POST: LoginsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user) 
        {

            if (user.Email == null || user.Password == null) //กรณีกรอกข้อมูล Email และ Password ไม่ครบ
            {
                ViewBag.Message = "กรอกข้อมูลไม่ครบ!";
            }
            else //กรณีกรอกข้อมูล Email และ Password ไครบ
            {
                string hashedPassword = HashingHelper.HashPassword(user.Password); // HashPassword ที่กรอกเข้ามา

                User checkDB = db.User.Where(m => m.Email == user.Email && m.Password == hashedPassword).FirstOrDefault();//ตรวจหา Email และ Password ที่ตรงกัน

                //var checkDB = db.User.Where(m => m.Email == user.Email && m.Password == user.Password).FirstOrDefault();

                if (checkDB != null) //กรณีที่มีค่า(หมายความว่า Email และ Password ตรง)
                {
                    HttpContext.Session.SetString("UserEmailSession", checkDB.Email);
                    HttpContext.Session.SetString("UserNameSession", checkDB.Name);
                    HttpContext.Session.SetString("Default", "Operator");
                    HttpContext.Session.SetString("UserIDSession", checkDB.ID.ToString());
                    GlobalVariable.SetUserID(checkDB.ID);
                    TempData["AlertMessage"] = "ลงชื่อเข้าใช้สำเร็จ! ยินดีต้อนรับ  คุณ " + checkDB.Name;
                    return RedirectToAction("Index", "Works" , new { ProviderFilterValue = GlobalVariable.GetUserID().ToString()});
                }
                else //กรณีที่มีค่า(หมายความว่า Email และ Password ไม่ตรง)
                {
                    ViewBag.Message = "การเข้าสู่ระบบล้มเหลว ชื่อผู้ใช้หรือรหัสผ่านของคุณอาจไม่ถูกต้อง";
                }
            }
            return View(user);
        }

        public ActionResult Logout() //ฟังชันสำหรับออกจากระบบ
        {
            var tempSession = HttpContext.Session.GetString("UserEmailSession");

            if (tempSession != null) // ตรวจสอบค่า UserEmailSession
            {
                HttpContext.Session.Remove("UserEmailSession");
                HttpContext.Session.Remove("UserNameSession");
                HttpContext.Session.Remove("Default");
                HttpContext.Session.Remove("UserIDSession");
                GlobalVariable.ClearGlobalVariable();
                //HttpContext.Session.Clear();
                TempData["AlertMessage"] = "ออกจากระบบสำเร็จ!";
            }

            return View("Login");
        }

        public ActionResult Register() //สำหรับเข้าหน้าลงทะเบียน
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (user.Email == null || user.Password == null || user.Name == null || user.ConfirmPassword == null)//กรณีกรอกข้อมูลไม่ครบ
            {
                ViewBag.InputWarning = "กรอกข้อมูลไม่ครบ! กรุณากรอกข้อมูลให้ครบ";
                return View(user);
            }

            var checkDB = db.User.Where(m => m.Email == user.Email).FirstOrDefault();

            if (checkDB != null) //กรณีตรวจหาอีเมลซํ้าได้
            {
                ViewBag.EmailWarning = "อีเมลนี้ลงทะเบียนแล้ว!";
                if (user.Password != user.ConfirmPassword) //กรณี Password และ ConfirmPassword ไม่ตรงกัน
                {
                    ViewBag.PasswordWarning = "รหัสผ่านกับยืนยันรหัสผ่านไม่ตรงกัน!";
                }
            }
            else //กรณีที่อีเมลไม่ซํ้า
            {
                if (user.Password != user.ConfirmPassword) //กรณี Password และ ConfirmPassword ไม่ตรงกัน
                {
                    ViewBag.PasswordWarning = "รหัสผ่านกับยืนยันรหัสผ่านไม่ตรงกัน!";
                }
                else //กรณี Password และ ConfirmPassword ตรงกัน
                {
                    //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    //user.Password = hashedPassword;
                    //user.Password = HashingHelper.HashPassword(user.Password);
                    //user.IsDelete = false;
                    //user.CreateDate = DateTime.Now;
                    //user.UpdateDate = DateTime.Now;
                    //db.User.Add(user);
                    user.Insert(db);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "ลงทะเบียนสำเร็จ!";
                    return RedirectToAction("Login");
                }
            }
            return View(user);
        }

        public ActionResult RegisterSuccess()
        {
            return View();
        }

    }
}
