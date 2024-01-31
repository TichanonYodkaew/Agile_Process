using AgileRap_Process2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgileRap_Process2.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        private readonly IEmailSender _emailSender;

        public HomeController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            //string emailBody = "<p>Hello, this is a <strong>JOB888</strong> come and fun!</p>";
            ////var email = "tichanon.yo.63@ubu.ac.th";
            //var subject = "Job888!!";
            ////var body = "Wake up!! we have a job to do!!";
            //List<string> emailList = new List<string>
            //{
            //"tichanon.yo.63@ubu.ac.th",
            //"tichanon4658@gmail.com",
            //// เพิ่มรายการอีเมลต่อไปตามต้องการ
            //};
            //_emailSender.SendEmail(emailList, subject, emailBody);
            return View();
        }

        public IActionResult Privacy()
        {
            //return View();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
