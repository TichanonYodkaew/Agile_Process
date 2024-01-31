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
        public bool RedFlag {  get; set; }

        [NotMapped]
        public bool HistoryPage { get; set;}

        public void Insert(AgileRap_Process2Context dbContext)
        {
            this.CreateBy = GlobalVariable.GetUserID();
            this.UpdateBy = GlobalVariable.GetUserID();
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsDelete = false;
            dbContext.Work.Add(this);
        }

        public void Update(AgileRap_Process2Context dbContext)
        {
            this.UpdateBy = GlobalVariable.GetUserID();
            this.UpdateDate = DateTime.Now;
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
