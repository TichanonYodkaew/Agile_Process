using System.ComponentModel.DataAnnotations;

namespace AgileRap_Process2.Models
{
    public partial class Work
    {
        public int ID { get; set; }
        public int? HeadID { get; set; }
        public string? Project { get; set; }
        public string? Name { get; set;}
        public DateTime? DueDate { get; set; }
        public int? StatusID { get; set; }
        public string? Remark { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set;}
        public bool? IsDelete { get; set; }

        //--- Parent

        public virtual Status Status { get; set; }

        ////---- Foreign key 
        public ICollection<WorkLog>? WorkLogs { get; set; }

        public ICollection<Provider>? Provider { get; set; }
    }
}
