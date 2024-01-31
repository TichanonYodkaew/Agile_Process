using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AgileRap_Process2.Models;

namespace AgileRap_Process2.Data
{
    public class AgileRap_Process2Context : DbContext
    {
        public AgileRap_Process2Context()
        {
        }

        public AgileRap_Process2Context(DbContextOptions<AgileRap_Process2Context> options)
    : base(options)
        { }
        public DbSet<Work> Work { get; set; }
        public DbSet<Provider> Provider { get; set; }
        public DbSet<ProviderLog> ProviderLog { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<WorkLog> WorkLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=LAPTOP-98EBUKHT\\SQLEXPRESS;Initial Catalog=DB_AgileRap_Process; Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
    }
}
