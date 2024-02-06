using AgileRap_Process2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgileRap_Process2.Models
{
    public class WorkLogMetadata
    {
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DueDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreateDate { get; set; }
    }

    [ModelMetadataType(typeof(WorkLogMetadata))]
    public partial class WorkLog
    {
        [NotMapped]
        public string? Description { get; set; }

        [NotMapped]
        public string? Line1 { get; set; }

        [NotMapped]
        public string? Line2 { get; set; }

        [NotMapped]
        public string? Line3 { get; set; }

        [NotMapped]
        public string? Line4 { get; set; }

        [NotMapped]
        public string? Line5 { get; set; }

        [NotMapped]
        public string? Line6 { get; set; }

        [NotMapped]
        public string? Line7 { get; set; }

        public void Insert(AgileRap_Process2Context dbContext)
        {
            this.CreateBy = GlobalVariable.GetUserID();
            this.UpdateBy = GlobalVariable.GetUserID();
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsDelete = false;

            dbContext.WorkLog.Add(this);
        }

        public void Update(AgileRap_Process2Context dbContext)
        {
            this.UpdateBy = GlobalVariable.GetUserID();
            this.UpdateDate = DateTime.Now;
            var existingEntity = dbContext.WorkLog.Find(this.ID);
            dbContext.Entry(existingEntity).CurrentValues.SetValues(this);
        }

        public void Delete(AgileRap_Process2Context dbContext)
        {
            var data = dbContext.WorkLog.Find(this.ID);
            data.UpdateBy = GlobalVariable.GetUserID();
            data.UpdateDate = DateTime.Now;
            data.IsDelete = true;
            dbContext.Entry(data).State = EntityState.Modified;
        }
    }

}
