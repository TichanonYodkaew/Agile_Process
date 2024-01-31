namespace AgileRap_Process2.Models
{
    public partial class User
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? LineID { get; set; }
        public string? Role { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsDelete { get; set; }

        ////---- Foreign key 
        public ICollection<Provider>? Provider { get; set; }
        public ICollection<ProviderLog>? ProviderLog { get; set; }

    }
}
