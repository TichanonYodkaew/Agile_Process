using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using AgileRap_Process2.Data;
using Microsoft.EntityFrameworkCore;

namespace AgileRap_Process2.Models
{
    public class WorkMetadata
    {
        [Required]
        public string? Project { get; set; }
        [Required]
        public string? Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DueDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreateDate { get; set; }
    }

    [ModelMetadataType(typeof(WorkMetadata))]

    public partial class Work
    {
        [NotMapped]
        public string? ProviderValue { get; set; }

        [NotMapped]
        public bool AllProviderSelected { get; set; }

        [NotMapped]
        public List<int>? ProvidersSelected { get; set; }

        [NotMapped]
        public bool RedFlag { get; set; }

        [NotMapped]
        public bool HistoryPage { get; set; }

        [NotMapped]
        public bool ProvidersFilterIsFound { get; set; }

        public void Insert(AgileRap_Process2Context dbContext)
        {
            this.CreateBy = GlobalVariable.GetUserID();
            this.UpdateBy = GlobalVariable.GetUserID();
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsDelete = false;

            //! -- ตรวจสอบ Due date (ป้องกันอีกชั้น) -- //
            if (this.DueDate == null)
            {
                this.StatusID = 1;
            }
            else
            {
                this.StatusID = 2;
            }
            //-- ********* -- //

            //TODO ฟังชันที่ต้องหาวิธียัดลง WorkMetadata

            this.Provider = new List<Provider>();
            this.WorkLogs = new List<WorkLog>();

            WorkLog workLog = new WorkLog();
            workLog.ProviderLog = new List<ProviderLog>();

            if (this.ID == 0)
            {
                workLog.No = 1;
            }
            else
            {
                workLog.No = this.WorkLogs.LastOrDefault().No + 1;
            }

            workLog.Name = this.Name;
            workLog.Project = this.Project;
            workLog.DueDate = this.DueDate;
            workLog.StatusID = this.StatusID;
            workLog.Remark = this.Remark;
            workLog.Insert(dbContext);

            if (this.AllProviderSelected == true) //! กรณีเลือก Provider ทั้งหมด
            {
                var userlist = dbContext.User.ToList();

                // เพิ่ม User ทั้งหมด ลง Provider
                foreach (var i in userlist)
                {
                    Provider provider = new Provider();
                    provider.UserID = i.ID;
                    provider.Insert(dbContext);
                    this.Provider.Add(provider);

                    ProviderLog providerLog = new ProviderLog();
                    providerLog.UserID = i.ID;
                    providerLog.Insert(dbContext);
                    workLog.ProviderLog.Add(providerLog);
                }
            }
            else //! กรณีเลือก Provider บางส่วน
            {
                if (this.ProviderValue != null)
                {
                    int[] listprovider = Array.ConvertAll(this.ProviderValue.Split(','), int.Parse);
                    // เพิ่ม User ตามที่เลือก ลง Provider
                    foreach (var i in listprovider)
                    {
                        Provider provider = new Provider();
                        provider.UserID = i;
                        provider.Insert(dbContext);
                        this.Provider.Add(provider);

                        ProviderLog providerLog = new ProviderLog();
                        providerLog.UserID = i;
                        providerLog.Insert(dbContext);
                        workLog.ProviderLog.Add(providerLog);
                    }

                }
            }
            this.WorkLogs.Add(workLog);
            //TODO สิ้นสุดฟังชันที่ต้องหาวิธียัดลง WorkMetadata

            dbContext.Work.Add(this);
        }

        public void Update(AgileRap_Process2Context dbContext)
        {
            this.UpdateBy = GlobalVariable.GetUserID();
            this.UpdateDate = DateTime.Now;

            WorkLog workLog = new WorkLog();

            workLog.No = this.WorkLogs.LastOrDefault().No + 1;

            workLog.WorkID = this.ID;
            workLog.Name = this.Name;
            workLog.Project = this.Project;
            workLog.DueDate = this.DueDate;
            workLog.StatusID = this.StatusID;
            workLog.Remark = this.Remark;
            workLog.Insert(dbContext);

            workLog.ProviderLog = new List<ProviderLog>();

            if (this.AllProviderSelected == true)//! กรณีเลือก Provider ทั้งหมด
            {
                var userlistdb = dbContext.User.ToList();
                var userIDs = this.Provider.Select(wp => wp.UserID).ToList();

                var existingProviders = dbContext.User
                    .Where(u => userIDs.Contains(u.ID))
                    .ToList();

                foreach (var workProvider in this.Provider)
                {
                    var userdb = existingProviders.FirstOrDefault(u => u.ID == workProvider.UserID);

                    if (userdb != null)
                    {
                        if (workProvider.IsDelete)
                        {
                            workProvider.Restore(dbContext);
                            ProviderLog providerLog = new ProviderLog();
                            providerLog.UserID = workProvider.UserID;
                            providerLog.Insert(dbContext);
                            workLog.ProviderLog.Add(providerLog);
                        }
                        existingProviders.Remove(userdb);
                    }
                    else if (!workProvider.IsDelete)
                    {
                        workProvider.Delete(dbContext);
                    }
                }

                foreach (var newUser in existingProviders)
                {
                    Provider provider = new Provider();
                    provider.UserID = newUser.ID;
                    provider.Insert(dbContext);
                    this.Provider.Add(provider);

                    ProviderLog providerLog = new ProviderLog();
                    providerLog.UserID = newUser.ID;
                    providerLog.Insert(dbContext);
                    workLog.ProviderLog.Add(providerLog);
                }
            }
            else //! กรณีเลือก Provider บางส่วน
            {
                if (this.ProviderValue != null) //กรณีที่ work.ProviderValue มีค่า
                {
                    List<int> listNewProvider = new List<int>(Array.ConvertAll(this.ProviderValue.Split(','), int.Parse));

                    listNewProvider.ForEach(i =>
                    {
                        ProviderLog providerLog = new ProviderLog();
                        providerLog.UserID = i;
                        providerLog.Insert(dbContext);
                        workLog.ProviderLog.Add(providerLog);
                    });

                    // Remove existing providers that are not in the new list
                    this.Provider
                        .Where(wp => listNewProvider.Contains(wp.UserID) && wp.IsDelete)
                        .ToList()
                        .ForEach(wp => wp.Restore(dbContext));

                    // Delete providers that are in the existing list but not in the new list
                    this.Provider
                        .Where(wp => !listNewProvider.Contains(wp.UserID) && !wp.IsDelete)
                        .ToList()
                        .ForEach(wp => wp.Delete(dbContext));

                    // Add new providers to the database
                    listNewProvider.Except(this.Provider.Select(wp => wp.UserID))
                        .ToList()
                        .ForEach(i =>
                        {
                            Provider provider = new Provider();
                            provider.WorkID = this.ID;
                            provider.UserID = i;
                            provider.Insert(dbContext);
                            this.Provider.Add(provider);
                        });
                }
                else //! กรณีที่ work.ProviderValue ไม่มีค่า
                {
                    //เพื่อป้องกัน worklog.ProviderLog มีค่าว่าง จึงเพิ่มจาก ProviderLog จาก Provider ที่มีอยู่
                    foreach (var i in this.Provider)
                    {
                        ProviderLog providerLog = new ProviderLog();
                        providerLog.UserID = i.UserID;
                        providerLog.Insert(dbContext);
                        workLog.ProviderLog.Add(providerLog);
                    }
                }
                this.WorkLogs.Add(workLog);
            }
            var existingEntity = dbContext.Work.Find(this.ID);
            dbContext.Entry(existingEntity).CurrentValues.SetValues(this);
        }

        public void Delete(AgileRap_Process2Context dbContext)
        {
            var data = dbContext.Work.Find(this.ID);
            data.UpdateBy = GlobalVariable.GetUserID();
            data.UpdateDate = DateTime.Now;
            data.IsDelete = true;
            dbContext.Entry(data).State = EntityState.Modified;
        }
    }
}
