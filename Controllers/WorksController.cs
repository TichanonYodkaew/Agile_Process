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
    public class WorksController : BaseController
    {
        private IEmailSender _emailSender;

        //AgileRap_Process2Context db = new AgileRap_Process2Context(); //dbContext
        static List<SelectListItem> _User = new List<SelectListItem>();
        static List<SelectListItem> _Status = new List<SelectListItem>();
        static List<SelectListItem> _Provider = new List<SelectListItem>();
        static List<SelectListItem> _Work = new List<SelectListItem>();
        static List<SelectListItem> _FilterProvider = new List<SelectListItem>();

        public WorksController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }


        public async Task<IActionResult> Index(string? RequseterFilter, string? ProviderFilterValue, string? ProjectFilter, string? StatusFilter, bool? IsChangePage, string? changeMode)
        {
            //! *** ตรวจสอบการ Login *** //
            if (HttpContext.Session.GetString("UserEmailSession") == null)
            {
                return RedirectToAction("Login", "Logins");
            }
            // *************************** //

            ClearStatic();//! <-- เคลีย static ทุกตัวที่มีค่าใน WorksController

            ChangeMode(changeMode);
            SelectProviderFilter(ProviderFilterValue);
            //var work = await db.Work
            //    .Include(so => so.Provider.Where(lo => lo.IsDelete == false))
            //    .ToListAsync();

            List<Work> work = await db.Work.Include(m => m.Status)
                .Include(b => b.Provider).ThenInclude(u => u.User)
                .Include(h => h.WorkLogs).ThenInclude(pl => pl.ProviderLog).ThenInclude(up => up.User)
                .ToListAsync();

            work = FilterListWork(work, RequseterFilter, ProviderFilterValue, ProjectFilter, StatusFilter, IsChangePage, null);

            InitDropdown(); //! <-- สร้าง dropdown ที่ต้องใช้
            AllViewBag(); //! <-- เรียกใช้ ViewBag ทุกอันที่มี

            //! ******* ตรวจสอบ work ที่มี due date ว่างถึง 2 วัน ********* //
            foreach (Work item in work)
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
        public async Task<IActionResult> Create(string? RequseterFilter, string? ProviderFilterValue, bool ProviderFilterAllSelected, string? ProjectFilter, string? StatusFilter, bool IsChangePage, string? changeMode)
        {
            //! *** ตรวจสอบการ Login *** //
            if (HttpContext.Session.GetString("UserEmailSession") == null)
            {
                return RedirectToAction("Login", "Logins");
            }
            // *** *********** *** //

            ClearStatic(); //!< --เคลีย static ทุกตัวที่มีค่าใน WorksController

            ChangeMode(changeMode);
            SelectProviderFilter(ProviderFilterValue);

            List<Work> work = await db.Work.Include(m => m.Status)
                .Include(b => b.Provider).ThenInclude(u => u.User)
                .Include(h => h.WorkLogs).ThenInclude(pl => pl.ProviderLog).ThenInclude(up => up.User)
                .ToListAsync();

            work = FilterListWork(work, RequseterFilter, ProviderFilterValue, ProjectFilter, StatusFilter, IsChangePage, null);
            //! ******* ตรวจสอบ work ที่มี due date ว่างถึง 2 วัน ********* //
            foreach (Work item in work)
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

            //! ************** เพิ่มโมเดลเปล่าที่ใช้ในฟอร์มเข้าไป **********//
            work.Add(new Work()
            {
                StatusID = 1,
                CreateBy = GlobalVariable.GetUserID(),
                CreateDate = DateTime.Now,
                RedFlag = false
            });
            // ***************** ******************** **********//

            InitDropdown(); //! <-- สร้าง dropdown ที่ต้องใช้
            AllViewBag(); //! <-- เรียกใช้ ViewBag ทุกอันที่มี

            return View(work);
        }


        // POST: Works/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Create(Work work) // ชื่อต้องตรง กับ front ที่ทำการรับข้อมูล
        {
            //var userdb = db.User.ToList();
            //var switchcase = 0; //! --swichcase แบบง่ายๆ กรณี Create

            //work.Provider = new List<Provider>();
            //work.WorkLogs = new List<WorkLog>();

            //WorkLog workLog = new WorkLog();
            //workLog.ProviderLog = new List<ProviderLog>();

            //workLog.No = 1;
            //workLog.Insert(db, work);

            ////TODO ฟังชันที่ต้องหาวิธียัดลง WorkMetadata

            //if (work.AllProviderSelected == true) //! กรณีเลือก Provider ทั้งหมด
            //{
            //    var userlist = db.User.ToList();

            //    // เพิ่ม User ทั้งหมด ลง Provider
            //    foreach (var i in userlist)
            //    {
            //        Provider provider = new Provider();
            //        provider.Insert(db,i.ID);
            //        work.Provider.Add(provider);

            //        ProviderLog providerLog = new ProviderLog();
            //        providerLog.Insert(db, i.ID);
            //        workLog.ProviderLog.Add(providerLog);
            //    }
            //}
            //else //! กรณีเลือก Provider บางส่วน
            //{
            //    int[] listprovider = Array.ConvertAll(work.ProviderValue.Split(','), int.Parse);
            //    // เพิ่ม User ตามที่เลือก ลง Provider
            //    foreach (var i in listprovider)
            //    {
            //        Provider provider = new Provider();
            //        provider.Insert(db,i);
            //        work.Provider.Add(provider);

            //        ProviderLog providerLog = new ProviderLog();
            //        providerLog.Insert(db, i);
            //        workLog.ProviderLog.Add(providerLog);
            //    }
            //}

            ////TODO สิ้นสุดฟังชันที่ต้องหาวิธียัดลง WorkMetadata
            //work.WorkLogs.Add(workLog);
            work.Insert(db); // บันทึกลง database
            db.SaveChanges();

            //TODO ฟํงก์ชันขั้นทดลอง

            // ส่วนในการรับ อีเมลของคนที่ต้องส่งอีเมลให้
            //List<string> emailList = new List<string>();
            //foreach (var p in work.Provider)
            //{
            //    var data = userdb.Where(x => x.ID == p.UserID).FirstOrDefault();
            //    emailList.Add(data.Email);
            //}

            //SendEmailNotification(work, emailList, switchcase); // ส่วนในการส่งอีเมล

            //TODO สิ้นสุดฟํงก์ชันขั้นทดลอง

            return RedirectToAction("Index", "Works", new
            {
                RequseterFilter = HttpContext.Session.GetString("RequseterFilter"),
                ProviderFilterValue = HttpContext.Session.GetString("ProviderFilterValue"),
                ProjectFilter = HttpContext.Session.GetString("ProjectFilter"),
                StatusFilter = HttpContext.Session.GetString("StatusFilter"),
                IsChangePage = true
            });
        }

        // GET: Works/Edit/5
        public async Task<IActionResult> Edit(int EditID, string? RequseterFilter, string? ProviderFilterValue, bool ProviderFilterAllSelected, string? ProjectFilter, string? StatusFilter, bool IsChangePage, string? changeMode)
        {
            //! *** ตรวจสอบการ Login *** //
            if (HttpContext.Session.GetString("UserEmailSession") == null)
            {
                return RedirectToAction("Login", "Logins");
            }
            // ************* //


            ClearStatic();//! <-- เคลีย static ทุกตัวที่มีค่าใน WorksController

            ChangeMode(changeMode);
            SelectProviderFilter(ProviderFilterValue);
            //var work = await db.Work
            //    .Include(so => so.Provider).Include(log => log.WorkLogs)
            //    .ToListAsync();

            List<Work> work = await db.Work.Include(m => m.Status)
                .Include(b => b.Provider).ThenInclude(u => u.User)
                .Include(h => h.WorkLogs).ThenInclude(pl => pl.ProviderLog).ThenInclude(up => up.User)
                .ToListAsync();

            work = FilterListWork(work, RequseterFilter, ProviderFilterValue, ProjectFilter, StatusFilter, IsChangePage, EditID);


            ViewBag.WorkEditID = EditID;

            foreach (Work item in work)
            {
                if (item.DueDate == null) //! ******* ตรวจสอบ work ที่มี due date ว่างถึง 2 วัน ********* //
                {
                    DateTime currentDate = DateTime.Now;
                    TimeSpan timeDifference = (TimeSpan)(currentDate - item.CreateDate);

                    if (timeDifference.TotalDays > 2)
                    {
                        item.RedFlag = true;
                    }
                }

                if (item.ID == EditID) //! ตรวจหา Work ที่ต้องแก้ไข
                {
                    item.RedFlag = false; // กรณีที่ Work ที่ต้องแก้ไข แล้วมี Due date ว่างถึงหรือเกิน 2 วัน ไม่ให้ไฮไลท์สีแดง

                    // สร้าง SelectListItem ของ Provider
                    foreach (User i in db.User.ToList())
                    {
                        _Provider.Add(new SelectListItem
                        {
                            Value = i.ID.ToString(),
                            Text = i.Name,
                        });
                    }
                    // ทำให้ SelectListItem ของ Provider ทำการเลือกตัวที่มีใน Provider ของ work
                    foreach (Provider item3 in item.Provider.Where(i => i.IsDelete == false))
                    {
                        foreach (SelectListItem i in _Provider)
                        {
                            if (item3.UserID == int.Parse(i.Value))
                            {
                                i.Selected = true;
                            }
                        }
                    }
                }
            }

            InitDropdown(); //! <-- สร้าง dropdown ที่ต้องใช้
            AllViewBag(); //! <-- เรียกใช้ ViewBag ทุกอันที่มี
            return View(work);
        }


        // POST: Works/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Edit(Work work)
        {
            //var userdbE = db.User.ToList();
            //var switchcase = 1;

            //! **** เริ่มการตรวจสอบการเปลี่ยนแปลง  **** //
            Work checkWorkDB = db.Work.Where(m => m.ID == work.ID).Include(p => p.Provider).FirstOrDefault();

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
            // **** สิ้นสุดการตรวจสอบการเปลี่ยนแปลง  **** //


            //!กรณีที่ไม่มีการเปลี่ยนแปลง
            if (!flagProject && !flagName && !flagDuedate && !flagStatus && !flagRemark && !flagSelectAllProvider && !flagProvidervalue)
            {
                //return RedirectToAction("Index");
                return RedirectToAction("Index", "Works", new
                {
                    RequseterFilter = HttpContext.Session.GetString("RequseterFilter"),
                    ProviderFilterValue = HttpContext.Session.GetString("ProviderFilterValue"),
                    ProjectFilter = HttpContext.Session.GetString("ProjectFilter"),
                    StatusFilter = HttpContext.Session.GetString("StatusFilter"),
                    IsChangePage = true
                });
            }

            //!กรณีที่มีการเปลี่ยนแปลง
            else
            {
                work.Update(db);
                db.SaveChanges();

                //TODO ฟํงก์ชันขั้นทดลอง
                //List<string> emailList = new List<string>();
                //foreach (var p in work.Provider)
                //{
                //    var data = userdbE.Where(x => x.ID == p.UserID).FirstOrDefault();
                //    emailList.Add(data.Email);
                //}
                //SendEmailNotification(work, emailList, switchcase);
                //TODO สิ้นสุดฟํงก์ชันขั้นทดลอง

                //return RedirectToAction("Index");
                return RedirectToAction("Index", "Works", new
                {
                    RequseterFilter = HttpContext.Session.GetString("RequseterFilter"),
                    ProviderFilterValue = HttpContext.Session.GetString("ProviderFilterValue"),
                    ProjectFilter = HttpContext.Session.GetString("ProjectFilter"),
                    StatusFilter = HttpContext.Session.GetString("StatusFilter"),
                    IsChangePage = true
                });
            }
        }

        public async Task<IActionResult> History(int HistoryID, string? RequseterFilter, string? ProviderFilterValue, bool ProviderFilterAllSelected, string? ProjectFilter, string? StatusFilter, bool IsChangePage, string? changeMode)
        {
            //! *** ตรวจสอบการ Login *** //
            if (HttpContext.Session.GetString("UserEmailSession") == null)
            {
                return RedirectToAction("Login", "Logins");
            }
            // ************************ //

            ClearStatic();//! <-- เคลีย static ทุกตัวที่มีค่าใน WorksController


            //var work = await db.Work
            //    .Include(s => s.Status)
            //    .Include(po => po.Provider)
            //    .Include(log => log.WorkLogs)
            //    .ToListAsync();

            ChangeMode(changeMode);
            SelectProviderFilter(ProviderFilterValue);

            List<Work> work = await db.Work.Include(m => m.Status)
                .Include(b => b.Provider).ThenInclude(u => u.User)
                .Include(h => h.WorkLogs).ThenInclude(pl => pl.ProviderLog).ThenInclude(up => up.User)
                .ToListAsync();

            work = FilterListWork(work, RequseterFilter, ProviderFilterValue, ProjectFilter, StatusFilter, IsChangePage, HistoryID);

            ViewBag.WorkCount = work.Count;
            ViewBag.WorkHistoryID = HistoryID;

            foreach (Work item in work)
            {
                item.HistoryPage = true; //เพื่อเซ็ตให้ทุก work แสดงสำหรับหน้า History (ไม่ให้แสดงส่วน manage)

                if (item.DueDate == null) //! ******* ตรวจสอบ work ที่มี due date ว่างถึง 2 วัน ********* //
                {
                    DateTime currentDate = DateTime.Now;
                    TimeSpan timeDifference = (TimeSpan)(currentDate - item.CreateDate);

                    if (timeDifference.TotalDays > 2)
                    {
                        item.RedFlag = true;
                    }
                }

                if (item.ID == HistoryID) //กรณีที่ ID ตรงกัน
                {
                    item.WorkLogs.Clear();

                    List<WorkLog> workLog = db.WorkLog.Where(d => d.WorkID == HistoryID)
                        .Include(i => i.Status)
                        .Include(o => o.ProviderLog).ThenInclude(oi => oi.User)
                        .ToList();

                    if (workLog.Count > 1)// กรณีที่ workLog มีมากกว่า 1
                    {
                        for (int i = 0; i < workLog.Count() - 1; i++)//เทียบ workLog ตัวปัจจุบันกับ workLog ตัวต่อไป
                        {
                            if (workLog[i] != workLog.LastOrDefault())// กรณีที่ workLog[i] ไม่ใช่ตัวสุดท้าย
                            {

                                if (workLog[i].Project != workLog[i + 1].Project) // กรณีที่ Project ไม่ตรงกัน
                                {
                                    workLog[i].Line1 = "";
                                    workLog[i].Line1 += "Project : " + workLog[i].Project + " -> " + workLog[i + 1].Project;
                                }
                                if (workLog[i].Name != workLog[i + 1].Name)// กรณีที่ Name ไม่ตรงกัน
                                {
                                    workLog[i].Line2 = "";
                                    workLog[i].Line2 += "Name : " + workLog[i].Name + " -> " + workLog[i + 1].Name;
                                }
                                if (workLog[i].DueDate != workLog[i + 1].DueDate)// กรณีที่ DueDate ไม่ตรงกัน
                                {
                                    workLog[i].Line3 = "";

                                    if (workLog[i].DueDate != null) // กรณี workLog ตัวปัจจุบัน มีค่า
                                    {
                                        DateTime specifiedDate = (DateTime)workLog[i].DueDate;
                                        string formattedDate = specifiedDate.ToString("dd/MM/yyyy");
                                        if (workLog[i + 1].DueDate != null) // กรณี workLog ตัวต่อไป มีค่า
                                        {
                                            DateTime specifiedDate2 = (DateTime)workLog[i + 1].DueDate;
                                            string formattedDate2 = specifiedDate2.ToString("dd/MM/yyyy");
                                            workLog[i].Line3 += "DueDate : " + formattedDate + " -> " + formattedDate2;
                                        }
                                        else //กรณี workLog ตัวต่อไป ไม่มีค่า
                                        {
                                            string formattedDate2 = " N/A ";
                                            workLog[i].Line3 += "DueDate : " + formattedDate + " -> " + formattedDate2;
                                        }

                                    }
                                    else// กรณี workLog ตัวปัจจุบัน ไม่มีค่า
                                    {
                                        string formattedDate = " N/A ";
                                        if (workLog[i + 1].DueDate != null) //กรณี workLog ตัวต่อไป มีค่า
                                        {
                                            DateTime specifiedDate2 = (DateTime)workLog[i + 1].DueDate;
                                            string formattedDate2 = specifiedDate2.ToString("dd/MM/yyyy");
                                            workLog[i].Line3 += "DueDate : " + formattedDate + " -> " + formattedDate2;
                                        }
                                        else //กรณี workLog ตัวต่อไป ไม่มีค่า(ป้องกันระเบิด)
                                        {
                                            string formattedDate2 = " N/A ";
                                            workLog[i].Line3 += "DueDate : " + formattedDate + " -> " + formattedDate2;
                                        }

                                    }

                                }
                                if (workLog[i].StatusID != workLog[i + 1].StatusID) // กรณีที่ StatusID ไม่ตรงกัน
                                {
                                    workLog[i].Line4 = "";
                                    workLog[i].Line4 += "Status : " + workLog[i].Status.StatusName + " -> " + workLog[i + 1].Status.StatusName;
                                }
                                if (workLog[i].Remark != workLog[i + 1].Remark) // กรณีที่ Remark ไม่ตรงกัน
                                {
                                    workLog[i].Line5 = "";
                                    workLog[i].Line5 += "Remark : " + workLog[i].Remark + " -> " + workLog[i + 1].Remark;
                                }

                                // แสดงชื่อผู้แก้ไข พร้อมเวลาที่แก้ไข
                                workLog[i].Line7 = "";
                                User Updater1 = db.User.Where(m => m.ID == workLog[i + 1].UpdateBy).FirstOrDefault();
                                workLog[i].Line7 += "Update by : " + Updater1.Name + ",  on " + workLog[i + 1].UpdateDate.Value.ToString("dd/MM/yyyy hh:mm tt");

                                //***** เทียบ ProviderLog ***** ///
                                if (workLog[i].ProviderLog != null)
                                {
                                    int tempcounter = 0;
                                    int tempcounter2 = 0;

                                    int tempProviderCount = workLog[i].ProviderLog.Count();
                                    int tempProviderCount2 = workLog[i + 1].ProviderLog.Count();

                                    string temppro1 = "";
                                    string temppro2 = "";

                                    foreach (ProviderLog tp1 in workLog[i].ProviderLog)
                                    {
                                        temppro1 += tp1.User.Name;

                                        tempcounter++;

                                        if (tempcounter < tempProviderCount)
                                        {
                                            temppro1 += ", ";
                                        }

                                    }

                                    foreach (ProviderLog tp2 in workLog[i + 1].ProviderLog)
                                    {
                                        temppro2 += tp2.User.Name;

                                        tempcounter2++;

                                        if (tempcounter2 < tempProviderCount2)
                                        {
                                            temppro2 += ", ";
                                        }

                                    }

                                    if (temppro1 != temppro2) // กรณีที่ Provider ไม่ตรงกัน (เทียบ String)
                                    {
                                        workLog[i].Line6 = "";
                                        workLog[i].Line6 += "Provider : ";

                                        string pro1 = temppro1;
                                        string pro2 = temppro2;

                                        workLog[i].Line6 += pro1 + " -> " + pro2;

                                    }
                                }
                                //***** สิ้นสุดการเทียบ ProviderLog ***** ///
                            }
                            item.WorkLogs.Add(workLog[i]);
                        }
                    }
                    else if (workLog.Count == 1) // กรณีที่ workLog มีเพียง 1 ตัว
                    {
                        for (int i = 0; i < workLog.Count; i++)
                        {
                            //แสดงประโยคว่า "ยังไม่มีการเปลี่ยนแปลงเกิดขึ้น"
                            workLog[i].Description = " No change. ";
                            item.WorkLogs.Add(workLog[i]);
                        }

                    }
                }
            }

            InitDropdown(); //! <-- สร้าง dropdown ที่ต้องใช้
            AllViewBag(); //! <-- เรียกใช้ ViewBag ทุกอันที่มี
            return View(work);
        }


        private bool WorkExists(int id) // <-- Gen ขึ้นมาเอง ไม่กล้าลบ
        {
            return db.Work.Any(e => e.ID == id);
        }

        private void AllViewBag() //! <-- สำหรับเรียกใช้ ViewBag ทุกอันที่มี
        {
            ViewBag.UserDropdown = _User;
            ViewBag.StatusDropdown = _Status;
            ViewBag.ProviderDropdownList = _Provider;
            ViewBag.ProjectDropdownList = _Work;
            ViewBag.FilterProviderDropdownList = _FilterProvider;
        }
        private void InitDropdown() //! <-- สำหรับสร้าง dropdown ที่ต้องใช้
        {
            //สร้าง dropdown User
            List<SelectListItem> UserList = db.User
                            .Select(u => new SelectListItem
                            {
                                Value = u.ID.ToString(),
                                Text = u.Name
                            }).ToList();
            _User = UserList;

            //สร้าง dropdown Status
            List<SelectListItem> StatusList = db.Status
                            .Select(s => new SelectListItem
                            {
                                Value = s.ID.ToString(),
                                Text = s.StatusName
                            }).ToList();
            _Status = StatusList;

            //var WorkList = db.Work
            //                .Select(u => new SelectListItem
            //                {
            //                    Value = u.Project,
            //                    Text = u.Project
            //                }).ToList();
            List<SelectListItem> WorkList = db.Work
                            .Select(u => u.Project)
                            .Distinct()
                            .Select(project => new SelectListItem
                            {
                                Value = project,
                                Text = project
                            }).ToList();
            _Work = WorkList;
        }

        private void ClearStatic() //! <-- สำหรับเคลีย static ทุกตัวที่มีค่าใน WorksController
        {
            _Provider.Clear();
            _Status.Clear();
            _User.Clear();
            _FilterProvider.Clear();
            _Work.Clear();
        }

        private void SendEmailNotification(Work work, List<string> emailList, int switchcase) //! <--ฟังก์ชันในการส่งอีเมล
        {
            if (switchcase == 0) //กรณีที่ Create Work
            {
                string subject = "New AgileRap Work!!"; //หัวข้ออีเมล

                string DuedateMailString = ""; //สำหรับแสดง Due date

                if (work.DueDate != null) //กรณี work.DueDate มีค่า
                {
                    DateTime specifiedDate = (DateTime)work.DueDate;
                    DuedateMailString = specifiedDate.ToString("dd/MMM/yyyy");
                }
                else //กรณี work.DueDate ไม่มีค่า
                {
                    DuedateMailString = "N/A";
                }

                string RemarkMailString = ""; //สำหรับแสดง Remark

                if (work.Remark != null) //กรณี work.Remark มีค่า
                {
                    RemarkMailString = work.Remark;
                }
                else //กรณี work.Remark ไม่มีค่า
                {
                    RemarkMailString = "N/A";
                }

                string ProviderMailString = ""; //สำหรับแสดง Provider

                if (work.Provider != null) //กรณี work.Provider มีค่า
                {
                    int tempcounter = 0;
                    int tempProviderCount = work.Provider.Count;
                    foreach (Provider p in work.Provider)
                    {
                        ProviderMailString += p.User.Name;

                        tempcounter++;

                        if (tempcounter < tempProviderCount)
                        {
                            ProviderMailString += ", ";
                        }

                    }
                }
                else //กรณี work.Provider ไม่มีค่า
                {
                    ProviderMailString = "N/A";
                }

                User finduser = db.User.Where(u => u.ID == work.CreateBy).FirstOrDefault();

                // เนื้อหาอีเมล
                string emailBody = "<h3>Hello, this is a new job for you.</h3>" +
                    "<p><strong>Assign by</strong> :  " + finduser.Name + " </p>" +
                    "<p><strong>Project</strong> : " + work.Project + "</p>" +
                    "<p><strong>Name</strong> : " + work.Name + "</p>" +
                    "<p><strong>Duedate</strong> : " + DuedateMailString + "</p>" +
                    "<p><strong>Provider</strong> : " + ProviderMailString + "</p>" +
                    "<p><strong>Remark</strong> : " + RemarkMailString + "</p>";

                // เรียกใช้ฟังชันส่งเมล
                _emailSender.SendEmail(emailList, subject, emailBody);

            }
            if (switchcase == 1) //กรณีที่ Edit Work
            {
                string subjectU = "AgileRap Work has Update!!"; //หัวข้ออีเมล

                //!------ เทียบประวัติการแก้ไข -------//
                List<WorkLog> workLog = db.WorkLog.Where(d => d.WorkID == work.ID)
                        .Include(i => i.Status)
                        .Include(o => o.ProviderLog).ThenInclude(oi => oi.User)
                        .ToList();

                // -- ดึงประวัติการแก้ไข 2 ตัวล่าสุดออกมา
                ICollection<WorkLog> items = workLog;

                IEnumerable<WorkLog> lastTwoItems = items.TakeLast(2);

                List<WorkLog> LastTwoWorkLog = lastTwoItems.ToList();
                // -- สิ้นสุดดึงประวัติการแก้ไข 2 ตัวล่าสุดออกมา

                int i = 0;

                // เนื้อหาอีเมล
                string emailBodyU = "<h3>This is a new update for your work.</h3>";

                // แสดงชื่อผู้แก้ไข พร้อมเวลาที่แก้ไข
                User Updater1 = db.User.Where(m => m.ID == LastTwoWorkLog[i + 1].UpdateBy).FirstOrDefault();
                emailBodyU += "<p><strong>Update by</strong> : " + Updater1.Name + "  <strong>on</strong> " + LastTwoWorkLog[i + 1].UpdateDate.Value.ToString("dd/MM/yyyy hh:mm tt") + "</p><br />";

                if (LastTwoWorkLog[i].Project != LastTwoWorkLog[i + 1].Project)// กรณีที่ Project ไม่ตรงกัน
                {
                    emailBodyU += "<p><strong>Project</strong> : " + LastTwoWorkLog[i].Project + " -> " + LastTwoWorkLog[i + 1].Project + "</p>";
                }
                if (LastTwoWorkLog[i].Name != LastTwoWorkLog[i + 1].Name)// กรณีที่ Name ไม่ตรงกัน
                {
                    emailBodyU += "<p><strong>Name</strong> : " + LastTwoWorkLog[i].Name + " -> " + LastTwoWorkLog[i + 1].Name + "</p>";
                }
                if (LastTwoWorkLog[i].DueDate != LastTwoWorkLog[i + 1].DueDate)// กรณีที่ DueDate ไม่ตรงกัน
                {
                    if (LastTwoWorkLog[i].DueDate != null) //กรณี workLog ตัวแรก มีค่า
                    {
                        DateTime specifiedDate = (DateTime)LastTwoWorkLog[i].DueDate;
                        string formattedDate = specifiedDate.ToString("dd/MMM/yyyy");

                        if (LastTwoWorkLog[i + 1].DueDate != null)//กรณี workLog ตัวสุดท้าย มีค่า
                        {
                            DateTime specifiedDate2 = (DateTime)LastTwoWorkLog[i + 1].DueDate;
                            string formattedDate2 = specifiedDate2.ToString("dd/MMM/yyyy");
                            emailBodyU += "<p><strong>Due date</strong> : " + formattedDate + " -> " + formattedDate2 + "</p>";
                        }
                        else//กรณี workLog ตัวสุดท้าย ไม่มีค่า
                        {
                            string formattedDate2 = " N/A ";
                            emailBodyU += "<p><strong>Due date</strong> : " + formattedDate + " -> " + formattedDate2 + "</p>";
                        }
                    }
                    else //กรณี workLog ตัวแรก ไม่มีค่า
                    {
                        string formattedDate = " N/A ";
                        if (LastTwoWorkLog[i + 1].DueDate != null)//กรณี workLog ตัวสุดท้าย มีค่า
                        {
                            DateTime specifiedDate2 = (DateTime)LastTwoWorkLog[i + 1].DueDate;
                            string formattedDate2 = specifiedDate2.ToString("dd/MM/yyyy");
                            emailBodyU += "<p><strong>Due date</strong> : " + formattedDate + " -> " + formattedDate2 + "</p>";
                        }
                        else//กรณี workLog ตัวสุดท้าย ไม่มีค่า(ป้องกันระเบิด)
                        {
                            string formattedDate2 = " N/A ";
                            emailBodyU += "<p><strong>Due date</strong> : " + formattedDate + " -> " + formattedDate2 + "</p>";
                        }

                    }

                }
                if (LastTwoWorkLog[i].StatusID != LastTwoWorkLog[i + 1].StatusID) // กรณีที่ StatusID ไม่ตรงกัน
                {
                    emailBodyU += "<p><strong>Status</strong> : " + LastTwoWorkLog[i].Status.StatusName + " -> " + LastTwoWorkLog[i + 1].Status.StatusName + "</p>";
                }
                if (LastTwoWorkLog[i].Remark != LastTwoWorkLog[i + 1].Remark) // กรณีที่ Remark ไม่ตรงกัน
                {
                    emailBodyU += "<p><strong>Remark</strong> : " + LastTwoWorkLog[i].Remark + " -> " + LastTwoWorkLog[i + 1].Remark + "</p>";
                }

                int tempcounter = 0;
                int tempcounter2 = 0;

                int tempProviderCount = LastTwoWorkLog[i].ProviderLog.Count();
                int tempProviderCount2 = LastTwoWorkLog[i + 1].ProviderLog.Count();

                string temppro1 = "";
                string temppro2 = "";

                foreach (ProviderLog tp1 in LastTwoWorkLog[i].ProviderLog)
                {
                    temppro1 += tp1.User.Name;

                    tempcounter++;

                    if (tempcounter < tempProviderCount)
                    {
                        temppro1 += ", ";
                    }

                }

                foreach (ProviderLog tp2 in LastTwoWorkLog[i + 1].ProviderLog)
                {
                    temppro2 += tp2.User.Name;

                    tempcounter2++;

                    if (tempcounter2 < tempProviderCount2)
                    {
                        temppro2 += ", ";
                    }

                }

                if (temppro1 != temppro2) // กรณีที่ Provider ไม่ตรงกัน
                {
                    string pro1 = temppro1;
                    string pro2 = temppro2;
                    emailBodyU += "<p><strong>Provider</strong> : " + pro1 + " -> " + pro2 + "</p>";
                }

                //!------ สิ้นสุดการเทียบประวัติการแก้ไข -------//

                // เรียกใช้ฟังชันส่งเมล
                _emailSender.SendEmail(emailList, subjectU, emailBodyU);

            }
        }

        private List<Work> FilterListWork(List<Work> works, string? RequseterFilter, string? ProviderFilterValue, string? ProjectFilter, string? StatusFilter, bool? IsChangePage, int? id)
        {
            if (IsChangePage == null && RequseterFilter == null) { HttpContext.Session.Remove("RequseterFilter"); }
            if (IsChangePage == null && ProviderFilterValue == null) { HttpContext.Session.Remove("ProviderFilterValue"); }
            if (IsChangePage == null && ProjectFilter == null) { HttpContext.Session.Remove("ProjectFilter"); }
            if (IsChangePage == null && StatusFilter == null) { HttpContext.Session.Remove("StatusFilter"); }

            if (RequseterFilter != null)
            {
                //works = works.Where(m => m.CreateBy == int.Parse(RequseterFilter)).ToList();
                works = works.Where(m => m.ID == id || m.CreateBy == int.Parse(RequseterFilter)).ToList();
                HttpContext.Session.SetString("RequseterFilter", RequseterFilter);
            }
            if (ProjectFilter != null)
            {
                //works = works.Where(m => m.Project == ProjectFilter).ToList();
                works = works.Where(m => m.ID == id || m.Project == ProjectFilter).ToList();
                HttpContext.Session.SetString("ProjectFilter", ProjectFilter);
            }
            if (StatusFilter != null)
            {
                //works = works.Where(m => m.StatusID == int.Parse(StatusFilter)).ToList();
                works = works.Where(m => m.ID == id || m.StatusID == int.Parse(StatusFilter)).ToList();
                HttpContext.Session.SetString("StatusFilter", StatusFilter);
            }
            if (ProviderFilterValue != null)
            {
                string[] ListStringsProvider = ProviderFilterValue.Split(',');
                foreach (Work work in works)
                {
                    if (work.Provider.Where(p => ListStringsProvider.Contains(p.UserID.ToString()) == true && p.IsDelete != true).Count() > 0)
                    {
                        work.ProvidersFilterIsFound = true;
                    }
                }
                //works = works.Where(m => m.ProvidersFilterIsFound == true).ToList();
                works = works.Where(m => m.ID == id || m.ProvidersFilterIsFound == true).ToList();
                HttpContext.Session.SetString("ProviderFilterValue", ProviderFilterValue);
            }

            return works;
        }

        private void ChangeMode(string? changeMode)
        {
            if (changeMode == "Operator")
            {
                HttpContext.Session.SetString("Default", "Operator");
                HttpContext.Session.Remove("RequseterFilter");
                HttpContext.Session.Remove("ProjectFilter");
                HttpContext.Session.Remove("StatusFilter");
            }
            else if (changeMode == "Controller")
            {
                HttpContext.Session.SetString("Default", "Controller");
                HttpContext.Session.Remove("ProviderFilterValue");
                HttpContext.Session.Remove("ProjectFilter");
                HttpContext.Session.Remove("StatusFilter");
            }
        }

        private void SelectProviderFilter(string? ProviderFilterValue)
        {
            if (ProviderFilterValue != null)
            {
                string[] ListStringsProvider = ProviderFilterValue.Split(',');
                List<SelectListItem> UserList = db.User
                                                .Select(u => new SelectListItem
                                                {
                                                    Value = u.ID.ToString(),
                                                    Text = u.Name
                                                }).ToList();
                //_FilterProvider = UserList;
                foreach (string i in ListStringsProvider)
                {
                    foreach (SelectListItem j in UserList)
                    {
                        if (i == j.Value)
                        {
                            j.Selected = true;
                            break;
                        }
                    }
                }
                _FilterProvider = UserList;
            }
            else
            {
                List<SelectListItem> UserList = db.User.Select(u => new SelectListItem { Value = u.ID.ToString(), Text = u.Name }).ToList();
                _FilterProvider = UserList;
            }
        }

    }
}

