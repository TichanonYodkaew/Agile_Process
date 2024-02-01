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
using System.Linq.Expressions;

namespace AgileRap_Process2.Controllers
{
    public class WorksController : Controller
    {
        private readonly IEmailSender _emailSender;

        AgileRap_Process2Context db = new AgileRap_Process2Context();
        static List<SelectListItem> _User = new List<SelectListItem>();
        static List<SelectListItem> _Status = new List<SelectListItem>();
        static List<SelectListItem> _Provider = new List<SelectListItem>();

        public WorksController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        //-- หน้าแรกของ work --//
        public async Task<IActionResult> Index()
        {
            // *** ตรวจสอบการ Login *** //
            if (HttpContext.Session.GetString("UserEmailSession") == null)
            {
                return RedirectToAction("Login", "Logins");
            }
            // *************************** //

            ClearStatic();// <-- เคลีย static ทุกตัวที่มีค่าใน WorksController

            var work = await db.Work
                .Include(so => so.Provider.Where(lo => lo.IsDelete == false))
                .ToListAsync();

            InitDropdown(); // <-- สร้าง dropdown ที่ต้องใช้
            AllViewBag(); // <-- เรียก ViewBag ทุดชกอัน

            // ******* ตรวจสอบ work ที่มี due date ว่างถึง 2 วัน ********* //
            foreach (var item in work)
            {
                if (item.DueDate == null)
                {
                    DateTime currentDate = DateTime.Now;
                    TimeSpan timeDifference = (TimeSpan)(currentDate - item.CreateDate);

                    if (timeDifference.TotalDays > 2)
                    {
                        item.RedFlag = true;
                    }
                }
            }
            // ******* ***************************** ********* //

            return View(work);
        }

        // GET: Works/Create
        public async Task<IActionResult> Create()
        {
            // *** ตรวจสอบการ Login *** //
            if (HttpContext.Session.GetString("UserEmailSession") == null)
            {
                return RedirectToAction("Login", "Logins");
            }
            // *** *********** *** //

            ClearStatic();

            var work = await db.Work
                .Include(so => so.Provider)
                .ThenInclude(u => u.User)
                .ToListAsync(); // <-- เรียกmodel work

            // ******* ตรวจสอบ work ที่มี due date ว่างถึง 2 วัน ********* //
            foreach (var item in work)
            {
                if (item.DueDate == null)
                {
                    DateTime currentDate = DateTime.Now;
                    TimeSpan timeDifference = (TimeSpan)(currentDate - item.CreateDate);

                    if (timeDifference.TotalDays > 2)
                    {
                        item.RedFlag = true;
                    }
                }
            }
            // ************************************************* //

            // ***************** เพิ่มโมเดลเปล่าที่ใช้ในฟอร์มเข้าไป **********//
            work.Add(new Work()
            {
                StatusID = 1,
                CreateBy = GlobalVariable.GetUserID(),
                CreateDate = DateTime.Now,
                RedFlag = false
            });
            // ***************** ******************** **********//


            InitDropdown();
            AllViewBag();

            return View(work);
        }


        // POST: Works/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Create(Work work) // ชื่อต้องตรง กับ front ที่ทำการรับข้อมูล
        {
            var userdb = db.User.ToList();
            var switchcase = 0;

            if (work.DueDate == null)
            {
                work.StatusID = 1;
            }
            else
            {
                work.StatusID = 2;
            }

            work.Provider = new List<Provider>();
            work.WorkLogs = new List<WorkLog>();
            WorkLog workLog = new WorkLog();
            workLog.ProviderLog = new List<ProviderLog>();

            //workLog.UpdateDate = DateTime.Now;
            //workLog.CreateDate = DateTime.Now;
            //workLog.IsDelete = false;
            //workLog.Name = work.Name;
            //workLog.Project = work.Project;
            //workLog.DueDate = work.DueDate;
            //workLog.StatusID = work.StatusID;
            //workLog.Remark = work.Remark;
            //workLog.UpdateBy = work.UpdateBy;
            //workLog.CreateBy = work.CreateBy;
            workLog.No = 1;
            workLog.Insert(db, work);


            if (work.AllProviderSelected == true)
            {
                var userlist = db.User.ToList();
                foreach (var i in userlist)
                {
                    Provider provider = new Provider();
                    //provider.IsDelete = false;
                    //provider.UpdateDate = DateTime.Now;
                    //provider.CreateDate = DateTime.Now;
                    //provider.CreateBy = work.CreateBy;
                    //provider.UpdateBy = work.UpdateBy;
                    provider.UserID = i.ID;
                    provider.Insert(db);
                    work.Provider.Add(provider);

                    ProviderLog providerLog = new ProviderLog();
                    //providerLog.IsDelete = false;
                    //providerLog.UpdateDate = DateTime.Now;
                    //providerLog.CreateDate = DateTime.Now;
                    //providerLog.CreateBy = work.CreateBy;
                    //providerLog.UpdateBy = work.UpdateBy;
                    providerLog.UserID = i.ID;
                    providerLog.Insert(db);
                    workLog.ProviderLog.Add(providerLog);
                }
            }
            else
            {
                int[] listprovider = Array.ConvertAll(work.ProviderValue.Split(','), int.Parse);
                foreach (var i in listprovider)
                {
                    Provider provider = new Provider();
                    //provider.IsDelete = false;
                    //provider.UpdateDate = DateTime.Now;
                    //provider.CreateDate = DateTime.Now;
                    //provider.CreateBy = work.CreateBy;
                    //provider.UpdateBy = work.UpdateBy;
                    provider.UserID = i;
                    provider.Insert(db);
                    work.Provider.Add(provider);

                    ProviderLog providerLog = new ProviderLog();
                    //providerLog.IsDelete = false;
                    //providerLog.UpdateDate = DateTime.Now;
                    //providerLog.CreateDate = DateTime.Now;
                    //providerLog.CreateBy = work.CreateBy;
                    //providerLog.UpdateBy = work.UpdateBy;
                    providerLog.UserID = i;
                    providerLog.Insert(db);
                    workLog.ProviderLog.Add(providerLog);
                }
            }
            work.WorkLogs.Add(workLog);

            //db.Work.Add(work);
            work.Insert(db);
            db.SaveChanges();

            List<string> emailList = new List<string>();
            foreach (var p in work.Provider)
            {
                var data = userdb.Where(x => x.ID == p.UserID).FirstOrDefault();
                emailList.Add(data.Email);
            }
            SendEmailNotification(work, emailList, switchcase);

            return RedirectToAction("Index");

        }

        // GET: Works/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserEmailSession") == null)
            {
                return RedirectToAction("Login", "Logins");
            }
            ClearStatic();

            if (id == null)
            {
                return NotFound();
            }

            //var work = await db.Work.FindAsync(id);
            var work = await db.Work
                .Include(so => so.Provider).Include(log => log.WorkLogs)
                .ToListAsync();

            if (work == null)
            {
                return NotFound();
            }

            ViewBag.WorkEditID = id;

            foreach (var item in work)
            {
                if (item.DueDate == null)
                {
                    DateTime currentDate = DateTime.Now;
                    TimeSpan timeDifference = (TimeSpan)(currentDate - item.CreateDate);

                    if (timeDifference.TotalDays > 2)
                    {
                        item.RedFlag = true;
                    }
                }

                if (item.ID == id)
                {
                    item.RedFlag = false;

                    foreach (var i in db.User.ToList())
                    {
                        _Provider.Add(new SelectListItem
                        {
                            Value = i.ID.ToString(),
                            Text = i.Name,
                        });
                    }

                    foreach (var item3 in item.Provider.Where(i => i.IsDelete == false))
                    {
                        foreach (var i in _Provider)
                        {
                            if (item3.UserID == int.Parse(i.Value))
                            {
                                i.Selected = true;
                            }
                        }
                    }
                    //ViewBag.ProviderDropdownList = _Provider;
                }
            }

            InitDropdown();
            AllViewBag();
            return View(work);
        }


        // POST: Works/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Edit(Work work)
        {
            var userdbE = db.User.ToList();
            var switchcase = 1;
            //work.UpdateDate = DateTime.Now;
            //work.UpdateBy = int.Parse(HttpContext.Session.GetString("UserIDSession"));

            // **** ตรวจสอบการเปลี่ยนแปลง  **** //
            var checkWorkDB = db.Work.Where(m => m.ID == work.ID).Include(p => p.Provider).FirstOrDefault();

            bool flagProject = false;
            bool flagName = false;
            bool flagDuedate = false;
            bool flagStatus = false;
            bool flagRemark = false;
            bool flagSelectAllProvider = false;
            bool flagProvidervalue = false;


            if (work.Project != checkWorkDB.Project)
            {
                flagProject = true;
            }
            if (work.Name != checkWorkDB.Name)
            {
                flagName = true;
            }
            if (work.DueDate != checkWorkDB.DueDate)
            {
                flagDuedate = true;
            }
            if (work.StatusID != checkWorkDB.StatusID)
            {
                flagStatus = true;
            }
            if (work.Remark != checkWorkDB.Remark)
            {
                flagRemark = true;
            }
            if (work.AllProviderSelected == true)
            {
                flagSelectAllProvider = true;
            }
            if (work.ProviderValue != null)
            {
                flagProvidervalue = true;
            }

            db.ChangeTracker.Clear();
            // **** ตรวจสอบการเปลี่ยนแปลง  **** //


            //กรณีที่ไม่มีการเปลี่ยนแปลง
            if (!flagProject && !flagName && !flagDuedate && !flagStatus && !flagRemark && !flagSelectAllProvider && !flagProvidervalue)
            {
                //db.Work.Update(work);
                work.Update(db);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //กรณีที่มีการเปลี่ยนแปลง
            else
            {
                WorkLog workLog = new WorkLog();
                //workLog.UpdateDate = DateTime.Now;
                //workLog.CreateDate = DateTime.Now;
                //workLog.IsDelete = false;
                //workLog.Name = work.Name;
                //workLog.Project = work.Project;
                //workLog.DueDate = work.DueDate;
                //workLog.StatusID = work.StatusID;
                //workLog.Remark = work.Remark;
                //workLog.CreateBy = work.UpdateBy;
                //workLog.UpdateBy = work.UpdateBy;

                workLog.No = work.WorkLogs.Count + 1;
                workLog.Insert(db, work);

                workLog.ProviderLog = new List<ProviderLog>();


                if (work.Provider != null)
                {
                    if (work.AllProviderSelected == true)
                    {
                        var userlistdb = db.User.ToList();

                        foreach (var workprovider in work.Provider)
                        {
                            bool alreadyExist = false;
                            foreach (var userdb in userlistdb)
                            {
                                if (workprovider.UserID == userdb.ID)
                                {
                                    alreadyExist = true;
                                    if (workprovider.IsDelete == true)
                                    {
                                        //db.Entry(workprovider).State = EntityState.Modified;
                                        //workprovider.UpdateDate = DateTime.Now;
                                        //workprovider.IsDelete = false;
                                        workprovider.Restore(db);
                                    }
                                    userlistdb.Remove(userdb);
                                    break;
                                }
                            }
                            if (alreadyExist == false)
                            {
                                //db.Entry(workprovider).State = EntityState.Modified;
                                //workprovider.UpdateDate = DateTime.Now;
                                //workprovider.IsDelete = true;
                                workprovider.Delete(db);
                            }
                        }

                        if (userlistdb.Count > 0)
                        {
                            foreach (var i in userlistdb)
                            {
                                Provider provider = new Provider();
                                //provider.IsDelete = false;
                                //provider.UpdateDate = DateTime.Now;
                                //provider.CreateDate = DateTime.Now;
                                //provider.CreateBy = work.UpdateBy;
                                //provider.UpdateBy = work.UpdateBy;
                                provider.UserID = i.ID;
                                provider.WorkID = work.ID;
                                provider.Insert(db);
                                work.Provider.Add(provider);

                                ProviderLog providerLog = new ProviderLog();
                                //providerLog.IsDelete = false;
                                //providerLog.UpdateDate = DateTime.Now;
                                //providerLog.CreateDate = DateTime.Now;
                                //providerLog.CreateBy = work.UpdateBy;
                                //providerLog.UpdateBy = work.UpdateBy;
                                providerLog.UserID = i.ID;
                                providerLog.Insert(db);
                                workLog.ProviderLog.Add(providerLog);
                            }
                        }
                    }
                    else
                    {
                        if (work.ProviderValue != null)
                        {
                            List<int> listNewProvider = new List<int>(Array.ConvertAll(work.ProviderValue.Split(','), int.Parse));

                            foreach (var q in listNewProvider)
                            {

                                ProviderLog providerLog = new ProviderLog();
                                //providerLog.IsDelete = false;
                                //providerLog.UpdateDate = DateTime.Now;
                                //providerLog.CreateDate = DateTime.Now;
                                //providerLog.CreateBy = work.UpdateBy;
                                //providerLog.UpdateBy = work.UpdateBy;
                                providerLog.UserID = q;
                                providerLog.Insert(db);
                                workLog.ProviderLog.Add(providerLog);
                            }

                            foreach (var workprovider in work.Provider)
                            {
                                bool alreadyExist = false;

                                foreach (var newListProvider in listNewProvider)
                                {
                                    if (workprovider.UserID == newListProvider)
                                    {
                                        alreadyExist = true;

                                        if (workprovider.IsDelete == true)
                                        {
                                            //db.Entry(workprovider).State = EntityState.Modified;
                                            //workprovider.UpdateDate = DateTime.Now;
                                            //workprovider.IsDelete = false;
                                            workprovider.Restore(db);
                                        }
                                        listNewProvider.Remove(newListProvider);
                                        break;
                                    }
                                }
                                if (alreadyExist == false)
                                {
                                    //db.Entry(workprovider).State = EntityState.Modified;
                                    //workprovider.UpdateDate = DateTime.Now;
                                    //workprovider.IsDelete = true;
                                    workprovider.Delete(db);
                                }
                            }

                            if (listNewProvider.Count > 0)
                            {
                                foreach (var i in listNewProvider)
                                {
                                    Provider provider = new Provider();
                                    //provider.IsDelete = false;
                                    //provider.UpdateDate = DateTime.Now;
                                    //provider.CreateDate = DateTime.Now;
                                    //provider.CreateBy = work.UpdateBy;
                                    //provider.UpdateBy = work.UpdateBy;
                                    provider.UserID = i;
                                    provider.WorkID = work.ID;
                                    provider.Insert(db);
                                    work.Provider.Add(provider);
                                }
                            }
                        }
                        else
                        {
                            foreach (var i in work.Provider)
                            {
                                ProviderLog providerLog = new ProviderLog();
                                //providerLog.IsDelete = false;
                                //providerLog.UpdateDate = DateTime.Now;
                                //providerLog.CreateDate = DateTime.Now;
                                //providerLog.CreateBy = work.UpdateBy;
                                //providerLog.UpdateBy = work.UpdateBy;
                                providerLog.UserID = i.UserID;
                                providerLog.Insert(db);
                                workLog.ProviderLog.Add(providerLog);
                            }
                        }
                    }
                    work.WorkLogs.Add(workLog);
                }

                //db.Entry(work).State = EntityState.Modified;
                //db.Work.Update(work);
                work.Update(db);
                db.SaveChanges();

                List<string> emailList = new List<string>();
                foreach (var p in work.Provider)
                {
                    var data = userdbE.Where(x => x.ID == p.UserID).FirstOrDefault();
                    emailList.Add(data.Email);
                }
                SendEmailNotification(work, emailList, switchcase);

                return RedirectToAction("Index");
            }

        }

        public async Task<IActionResult> History(int? id)
        {
            if (HttpContext.Session.GetString("UserEmailSession") == null)
            {
                return RedirectToAction("Login", "Logins");
            }
            ClearStatic();

            if (id == null)
            {
                return NotFound();
            }

            //var work = await db.Work.FindAsync(id);
            var work = await db.Work
                .Include(s => s.Status)
                .Include(po => po.Provider)
                .Include(log => log.WorkLogs)
                .ToListAsync();

            if (work == null)
            {
                return NotFound();
            }

            ViewBag.WorkCount = work.Count;
            ViewBag.WorkHistoryID = id;

            foreach (var item in work)
            {
                item.HistoryPage = true;

                if (item.DueDate == null)
                {
                    DateTime currentDate = DateTime.Now;
                    TimeSpan timeDifference = (TimeSpan)(currentDate - item.CreateDate);

                    if (timeDifference.TotalDays > 2)
                    {
                        item.RedFlag = true;
                    }
                }

                if (item.ID == id)
                {
                    item.WorkLogs.Clear();

                    var workLog = db.WorkLog.Where(d => d.WorkID == id)
                        .Include(i => i.Status)
                        .Include(o => o.ProviderLog).ThenInclude(oi => oi.User)
                        .ToList();

                    if (workLog.Count > 1)
                    {
                        for (int i = 0; i < workLog.Count() - 1; i++)
                        {
                            //workLog[i].Description = "";
                            //if (workLog[i].Project != workLog[i + 1].Project) { workLog[i].Description += "Project : " + workLog[i].Project + "->" + workLog[i + 1].Project; }
                            //if (workLog[i].Name != workLog[i + 1].Name) { workLog[i].Description += "Name : " + workLog[i].Name + "->" + workLog[i + 1].Name; }
                            //if (workLog[i].DueDate != workLog[i + 1].DueDate) { workLog[i].Description += "DueDate : " + workLog[i].DueDate + "->" + workLog[i + 1].DueDate; }
                            //if (workLog[i].StatusID != workLog[i + 1].StatusID) { workLog[i].Description += "Status : " + workLog[i].Status.StatusName + "->" + workLog[i + 1].Status.StatusName;}
                            //if (workLog[i].Remark != workLog[i + 1].Remark) { workLog[i].Description += "Remark : " + workLog[i].Remark + "->" + workLog[i + 1].Remark; }

                            if (workLog[i] != workLog.LastOrDefault())
                            {

                                if (workLog[i].Project != workLog[i + 1].Project)
                                {
                                    workLog[i].Line1 = "";
                                    workLog[i].Line1 += "Project : " + workLog[i].Project + " -> " + workLog[i + 1].Project;
                                }
                                if (workLog[i].Name != workLog[i + 1].Name)
                                {
                                    workLog[i].Line2 = "";
                                    workLog[i].Line2 += "Name : " + workLog[i].Name + " -> " + workLog[i + 1].Name;
                                }
                                if (workLog[i].DueDate != workLog[i + 1].DueDate)
                                {
                                    workLog[i].Line3 = "";

                                    if (workLog[i].DueDate != null)
                                    {
                                        DateTime specifiedDate = (DateTime)workLog[i].DueDate;
                                        string formattedDate = specifiedDate.ToString("dd/MM/yyyy");
                                        if (workLog[i + 1].DueDate != null)
                                        {
                                            DateTime specifiedDate2 = (DateTime)workLog[i + 1].DueDate;
                                            string formattedDate2 = specifiedDate2.ToString("dd/MM/yyyy");
                                            workLog[i].Line3 += "DueDate : " + formattedDate + " -> " + formattedDate2;
                                        }
                                        else
                                        {
                                            string formattedDate2 = " N/A ";
                                            workLog[i].Line3 += "DueDate : " + formattedDate + " -> " + formattedDate2;
                                        }

                                    }
                                    else
                                    {
                                        string formattedDate = " N/A ";
                                        if (workLog[i + 1].DueDate != null)
                                        {
                                            DateTime specifiedDate2 = (DateTime)workLog[i + 1].DueDate;
                                            string formattedDate2 = specifiedDate2.ToString("dd/MM/yyyy");
                                            workLog[i].Line3 += "DueDate : " + formattedDate + " -> " + formattedDate2;
                                        }
                                        else
                                        {
                                            string formattedDate2 = " N/A ";
                                            workLog[i].Line3 += "DueDate : " + formattedDate + " -> " + formattedDate2;
                                        }

                                    }

                                }
                                if (workLog[i].StatusID != workLog[i + 1].StatusID)
                                {
                                    workLog[i].Line4 = "";
                                    workLog[i].Line4 += "Status : " + workLog[i].Status.StatusName + " -> " + workLog[i + 1].Status.StatusName;
                                }
                                if (workLog[i].Remark != workLog[i + 1].Remark)
                                {
                                    workLog[i].Line5 = "";
                                    workLog[i].Line5 += "Remark : " + workLog[i].Remark + " -> " + workLog[i + 1].Remark;
                                }

                                workLog[i].Line7 = "";
                                var Updater1 = db.User.Where(m => m.ID == workLog[i + 1].UpdateBy).FirstOrDefault();
                                workLog[i].Line7 += "Update by : " + Updater1.Name + ",  on " + workLog[i + 1].UpdateDate.Value.ToString("dd/MM/yyyy hh:mm tt");


                                if (workLog[i].ProviderLog != null)
                                {
                                    //if (!workLog[i].ProviderLog.SequenceEqual(workLog[i + 1].ProviderLog))

                                    int tempcounter = 0;
                                    int tempcounter2 = 0;

                                    int tempProviderCount = workLog[i].ProviderLog.Count();
                                    int tempProviderCount2 = workLog[i + 1].ProviderLog.Count();

                                    string temppro1 = "";
                                    string temppro2 = "";

                                    foreach (var tp1 in workLog[i].ProviderLog)
                                    {
                                        temppro1 += tp1.User.Name;

                                        tempcounter++;

                                        if (tempcounter < tempProviderCount)
                                        {
                                            temppro1 += ", ";
                                        }

                                    }

                                    foreach (var tp2 in workLog[i + 1].ProviderLog)
                                    {
                                        temppro2 += tp2.User.Name;

                                        tempcounter2++;

                                        if (tempcounter2 < tempProviderCount2)
                                        {
                                            temppro2 += ", ";
                                        }

                                    }


                                    //if (workLog[i].ProviderLog != workLog[i + 1].ProviderLog)
                                    if (temppro1 != temppro2)
                                    {
                                        workLog[i].Line6 = "";
                                        workLog[i].Line6 += "Provider : ";

                                        string pro1 = temppro1;
                                        string pro2 = temppro2;

                                        //int counter = 0;
                                        //int counter2 = 0;

                                        //int ProviderCount = workLog[i].ProviderLog.Count();
                                        //int ProviderCount2 = workLog[i + 1].ProviderLog.Count();

                                        //string pro1 = "";
                                        //string pro2 = "";

                                        //foreach (var p1 in workLog[i].ProviderLog)
                                        //{
                                        //    pro1 += p1.User.Name;

                                        //    counter++;

                                        //    if (counter < ProviderCount)
                                        //    {
                                        //        pro1 += ", ";
                                        //    }

                                        //}

                                        //foreach (var p2 in workLog[i + 1].ProviderLog)
                                        //{
                                        //    pro2 += p2.User.Name;

                                        //    counter2++;

                                        //    if (counter2 < ProviderCount2)
                                        //    {
                                        //        pro2 += ", ";
                                        //    }

                                        //}

                                        workLog[i].Line6 += pro1 + " -> " + pro2;

                                    }


                                }


                            }
                            //else
                            //{
                            //    workLog[i].Line1 = "";
                            //    workLog[i].Line2 = "";
                            //    workLog[i].Line3 = "";
                            //    workLog[i].Line4 = "";
                            //    workLog[i].Line5 = "";
                            //    workLog[i].Line7 = "";

                            //    workLog[i].Line1 += "Project : " + workLog[i].Project;
                            //    workLog[i].Line2 += "Name : " + workLog[i].Name;
                            //    //workLog[i].Line3 += "DueDate : " + workLog[i].DueDate;

                            //    if (workLog[i].DueDate != null)
                            //    {
                            //        DateTime specifiedDate = (DateTime)workLog[i].DueDate;
                            //        string formattedDate = specifiedDate.ToString("dd/MM/yyyy");
                            //        workLog[i].Line3 += "DueDate : " + formattedDate;
                            //    }
                            //    else
                            //    {
                            //        string formattedDate = " N/A ";
                            //        workLog[i].Line3 += "DueDate : " + formattedDate;
                            //    }

                            //    workLog[i].Line4 += "Status : " + workLog[i].Status.StatusName;

                            //    workLog[i].Line5 += "Remark : " + workLog[i].Remark;

                            //    var Updater = db.User.Where(m => m.ID == workLog[i].UpdateBy).FirstOrDefault();
                            //    workLog[i].Line7 += "UpdateBy : " + Updater.Name;

                            //    if (workLog[i].ProviderLog != null)
                            //    {
                            //        workLog[i].Line6 = "";
                            //        workLog[i].Line6 += "Provider : ";
                            //        int counter = 0;
                            //        int ProviderCount = workLog[i].ProviderLog.Count();
                            //        foreach (var p in workLog[i].ProviderLog)
                            //        {
                            //            workLog[i].Line6 += p.User.Name;

                            //            counter++;

                            //            if (counter < ProviderCount)
                            //            {
                            //                workLog[i].Line6 += ", ";
                            //            }

                            //        }
                            //    }
                            //}


                            item.WorkLogs.Add(workLog[i]);
                        }
                    }
                    else if (workLog.Count == 1)
                    {
                        for (int i = 0; i < workLog.Count; i++)
                        {
                            workLog[i].Description = " No change. ";
                            //workLog[i].Line1 = "";
                            //workLog[i].Line2 = "";
                            //workLog[i].Line3 = "";
                            //workLog[i].Line4 = "";
                            //workLog[i].Line5 = "";
                            //workLog[i].Line7 = "";

                            ////workLog[i].Description += "Project : " + workLog[i].Project;
                            ////workLog[i].Description += "Name : " + workLog[i].Name;
                            ////workLog[i].Description += "DueDate : " + workLog[i].DueDate;
                            ////workLog[i].Description += "Status : " + workLog[i].Status.StatusName;
                            ////workLog[i].Description += "Remark : " + workLog[i].Remark;
                            //workLog[i].Line1 += "Project : " + workLog[i].Project;
                            //workLog[i].Line2 += "Name : " + workLog[i].Name;
                            ////workLog[i].Line3 += "DueDate : " + workLog[i].DueDate;
                            //if (workLog[i].DueDate != null)
                            //{
                            //    DateTime specifiedDate = (DateTime)workLog[i].DueDate;
                            //    string formattedDate = specifiedDate.ToString("dd/MM/yyyy");
                            //    workLog[i].Line3 += "DueDate : " + formattedDate;
                            //}
                            //else
                            //{
                            //    string formattedDate = " N/A ";
                            //    workLog[i].Line3 += "DueDate : " + formattedDate;
                            //}
                            //workLog[i].Line4 += "Status : " + workLog[i].Status.StatusName;
                            //workLog[i].Line5 += "Remark : " + workLog[i].Remark;

                            //var Updater = db.User.Where(m => m.ID == workLog[i].UpdateBy).FirstOrDefault();
                            //workLog[i].Line7 += "UpdateBy : " + Updater.Name;

                            //if (workLog[i].ProviderLog != null)
                            //{
                            //    workLog[i].Line6 = "";
                            //    workLog[i].Line6 += "Provider : ";
                            //    int counter = 0;
                            //    int ProviderCount = workLog[i].ProviderLog.Count();
                            //    foreach (var p in workLog[i].ProviderLog)
                            //    {
                            //        workLog[i].Line6 += p.User.Name;

                            //        counter++;

                            //        if (counter < ProviderCount)
                            //        {
                            //            workLog[i].Line6 += ", ";
                            //        }

                            //    }
                            //}

                            item.WorkLogs.Add(workLog[i]);
                        }

                    }
                }
            }


            InitDropdown();
            AllViewBag();
            return View(work);
        }


        private bool WorkExists(int id)
        {
            return db.Work.Any(e => e.ID == id);
        }

        private void AllViewBag()
        {
            ViewBag.UserDropdown = _User;
            ViewBag.StatusDropdown = _Status;
            ViewBag.ProviderDropdownList = _Provider;
        }
        private void InitDropdown()
        {
            var UserList = db.User
                            .Select(u => new SelectListItem
                            {
                                Value = u.ID.ToString(),
                                Text = u.Name
                            }).ToList();
            _User = UserList;

            var StatusList = db.Status
                            .Select(s => new SelectListItem
                            {
                                Value = s.ID.ToString(),
                                Text = s.StatusName
                            }).ToList();
            _Status = StatusList;
        }

        private void ClearStatic()
        {
            _Provider.Clear();
            _Status.Clear();
            _User.Clear();
        }

        private void SendEmailNotification(Work work, List<string> emailList, int switchcase)
        {
            if (switchcase == 0)
            {
                var subject = "New AgileRap Work!!";

                var DuedateMailString = "";

                if (work.DueDate != null)
                {
                    DateTime specifiedDate = (DateTime)work.DueDate;
                    DuedateMailString = specifiedDate.ToString("dd/MMM/yyyy");
                }
                else
                {
                    DuedateMailString = "N/A";
                }

                var RemarkMailString = "";

                if (work.Remark != null)
                {
                    RemarkMailString = work.Remark;
                }
                else
                {
                    RemarkMailString = "N/A";
                }

                var ProviderMailString = "";

                if (work.Provider != null)
                {
                    int tempcounter = 0;
                    int tempProviderCount = work.Provider.Count;
                    foreach (var p in work.Provider)
                    {
                        ProviderMailString += p.User.Name;

                        tempcounter++;

                        if (tempcounter < tempProviderCount)
                        {
                            ProviderMailString += ", ";
                        }

                    }
                }
                else
                {
                    ProviderMailString = "N/A";
                }

                string emailBody = "<h3>Hello, this is a new job for you.</h3>" +
                    "<p><strong>Project</strong> : " + work.Project + "</p>" +
                    "<p><strong>Name</strong> : " + work.Name + "</p>" +
                    "<p><strong>Duedate</strong> : " + DuedateMailString + "</p>" +
                    "<p><strong>Provider</strong> : " + ProviderMailString + "</p>" +
                    "<p><strong>Remark</strong> : " + RemarkMailString + "</p>";

                _emailSender.SendEmail(emailList, subject, emailBody);

            }
            if (switchcase == 1)
            {
                var subjectU = "AgileRap Work has Update!!";
                //****************************************************************************************//
                var workLog = db.WorkLog.Where(d => d.WorkID == work.ID)
                        .Include(i => i.Status)
                        .Include(o => o.ProviderLog).ThenInclude(oi => oi.User)
                        .ToList();


                ICollection<WorkLog> items = workLog;

                IEnumerable<WorkLog> lastTwoItems = items.TakeLast(2);

                var LastTwoWorkLog = lastTwoItems.ToList();
                
                int i = 0;


                string emailBodyU = "<h3>This is a new update for your work.</h3>";

                var Updater1 = db.User.Where(m => m.ID == LastTwoWorkLog[i + 1].UpdateBy).FirstOrDefault();
                emailBodyU += "<p><strong>Update by</strong> : " + Updater1.Name + "  <strong>on</strong> " + LastTwoWorkLog[i + 1].UpdateDate.Value.ToString("dd/MM/yyyy hh:mm tt") + "</p><br />";

                if (LastTwoWorkLog[i].Project != LastTwoWorkLog[i + 1].Project)
                {
                    emailBodyU += "<p><strong>Project</strong> : " + LastTwoWorkLog[i].Project + " -> " + LastTwoWorkLog[i + 1].Project + "</p>";
                }
                if (LastTwoWorkLog[i].Name != LastTwoWorkLog[i + 1].Name)
                {
                    emailBodyU += "<p><strong>Name</strong> : " + LastTwoWorkLog[i].Name + " -> " + LastTwoWorkLog[i + 1].Name + "</p>";
                }
                if (LastTwoWorkLog[i].DueDate != LastTwoWorkLog[i + 1].DueDate)
                {
                    if (LastTwoWorkLog[i].DueDate != null)
                    {
                        DateTime specifiedDate = (DateTime)LastTwoWorkLog[i].DueDate;
                        string formattedDate = specifiedDate.ToString("dd/MMM/yyyy");
                        if (LastTwoWorkLog[i + 1].DueDate != null)
                        {
                            DateTime specifiedDate2 = (DateTime)LastTwoWorkLog[i + 1].DueDate;
                            string formattedDate2 = specifiedDate2.ToString("dd/MMM/yyyy");
                            emailBodyU += "<p><strong>Due date</strong> : " + formattedDate + " -> " + formattedDate2 + "</p>";
                        }
                        else
                        {
                            string formattedDate2 = " N/A ";
                            emailBodyU += "<p><strong>Due date</strong> : " + formattedDate + " -> " + formattedDate2 + "</p>";
                        }
                    }
                    else
                    {
                        string formattedDate = " N/A ";
                        if (LastTwoWorkLog[i + 1].DueDate != null)
                        {
                            DateTime specifiedDate2 = (DateTime)LastTwoWorkLog[i + 1].DueDate;
                            string formattedDate2 = specifiedDate2.ToString("dd/MM/yyyy");
                            emailBodyU += "<p><strong>Due date</strong> : " + formattedDate + " -> " + formattedDate2 + "</p>";
                        }
                        else
                        {
                            string formattedDate2 = " N/A ";
                            emailBodyU += "<p><strong>Due date</strong> : " + formattedDate + " -> " + formattedDate2 + "</p>";
                        }

                    }

                }
                if (LastTwoWorkLog[i].StatusID != LastTwoWorkLog[i + 1].StatusID)
                {
                    emailBodyU += "<p><strong>Status</strong> : " + LastTwoWorkLog[i].Status.StatusName + " -> " + LastTwoWorkLog[i + 1].Status.StatusName + "</p>";
                    //emailBodyU += "<p><strong>Status</strong> : " + statusdb.Where(s => s.ID == LastTwoWorkLog[i].StatusID).First().StatusName  + " -> " + statusdb.Where(s => s.ID == LastTwoWorkLog[i + 1].StatusID).First().StatusName + "</p><br />";
                }
                if (LastTwoWorkLog[i].Remark != LastTwoWorkLog[i + 1].Remark)
                {
                    emailBodyU += "<p><strong>Remark</strong> : " + LastTwoWorkLog[i].Remark + " -> " + LastTwoWorkLog[i + 1].Remark + "</p>";
                }

                int tempcounter = 0;
                int tempcounter2 = 0;

                int tempProviderCount = LastTwoWorkLog[i].ProviderLog.Count();
                int tempProviderCount2 = LastTwoWorkLog[i + 1].ProviderLog.Count();

                string temppro1 = "";
                string temppro2 = "";

                foreach (var tp1 in LastTwoWorkLog[i].ProviderLog)
                {
                    temppro1 += tp1.User.Name;

                    tempcounter++;

                    if (tempcounter < tempProviderCount)
                    {
                        temppro1 += ", ";
                    }

                }

                foreach (var tp2 in LastTwoWorkLog[i + 1].ProviderLog)
                {
                    temppro2 += tp2.User.Name;

                    tempcounter2++;

                    if (tempcounter2 < tempProviderCount2)
                    {
                        temppro2 += ", ";
                    }

                }

                if (temppro1 != temppro2)
                {
                    string pro1 = temppro1;
                    string pro2 = temppro2;
                    emailBodyU += "<p><strong>Provider</strong> : " + pro1 + " -> " + pro2 + "</p>";
                }
                //****************************************************************************************//

                _emailSender.SendEmail(emailList, subjectU, emailBodyU);

            }

        }

    }
}

