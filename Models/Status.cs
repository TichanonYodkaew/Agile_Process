namespace AgileRap_Process2.Models
{
    public partial class Status
    {
        public int ID { get; set; }
        public string? StatusName { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsDelete { get; set; }

        //----- Foreign key

        public ICollection<Work>? Work { get; set; }
        public ICollection<WorkLog>? WorkLog { get; set; }


    }
}
