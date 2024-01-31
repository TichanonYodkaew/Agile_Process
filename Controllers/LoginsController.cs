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

namespace AgileRap_Process2.Controllers
{
    public class LoginsController : Controller
    {
        AgileRap_Process2Context db = new AgileRap_Process2Context();


        // GET: LoginsController
        public ActionResult Login()
        {
            User user = new User();
            if (TempData["shortMessage"] != null)
            {
                ViewBag.Alert = TempData["shortMessage"].ToString();
            }

            return View(user);
        }

        // POST: LoginsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            if (user.Email == null || user.Password == null)
            {
                ViewBag.Message = "กรอกข้อมูลไม่ครบ! กรุณากรอกข้อมูลให้ครบ";
            }
            else
            {
                var checkDB = db.User.Where(m => m.Email == user.Email && m.Password == user.Password).FirstOrDefault();

                if (checkDB != null)
                {
                    HttpContext.Session.SetString("UserEmailSession", checkDB.Email);
                    HttpContext.Session.SetString("UserNameSession", checkDB.Name);
                    HttpContext.Session.SetString("UserIDSession", checkDB.ID.ToString());
                    GlobalVariable.SetUserID(checkDB.ID);
                    TempData["AlertMessage"] = "ลงชื่อเข้าใช้สำเร็จ! ยินดีต้อนรับ  คุณ " + checkDB.Name;
                    return RedirectToAction("Index", "Works");
                }
                else
                {
                    ViewBag.Message = "การเข้าสู่ระบบล้มเหลว ชื่อผู้ใช้หรือรหัสผ่านของคุณอาจไม่ถูกต้อง";
                }
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            var tempSession = HttpContext.Session.GetString("UserEmailSession");

            if (tempSession != null)
            {
                HttpContext.Session.Remove("UserEmailSession");
                HttpContext.Session.Remove("UserNameSession");
                HttpContext.Session.Remove("UserIDSession");
                GlobalVariable.ClearGlobalVariable();
                //HttpContext.Session.Clear();
                TempData["AlertMessage"] = "ออกจากระบบสำเร็จ!";
            }

            return View("Login");
        }

        public ActionResult Register()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (user.Email == null || user.Password == null || user.Name == null || user.ConfirmPassword == null)
            {
                ViewBag.InputWarning = "กรอกข้อมูลไม่ครบ! กรุณากรอกข้อมูลให้ครบ";
                return View(user);
            }

            var checkDB = db.User.Where(m => m.Email == user.Email).FirstOrDefault();
            if (checkDB != null)
            {
                ViewBag.EmailWarning = "อีเมลนี้ลงทะเบียนแล้ว!";
                if (user.Password != user.ConfirmPassword)
                {
                    ViewBag.PasswordWarning = "รหัสผ่านกับยืนยันรหัสผ่านไม่ตรงกัน!";
                }
            }
            else
            {
                if (user.Password != user.ConfirmPassword)
                {
                    ViewBag.PasswordWarning = "รหัสผ่านกับยืนยันรหัสผ่านไม่ตรงกัน!";
                }
                else
                {
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
