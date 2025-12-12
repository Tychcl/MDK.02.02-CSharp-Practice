using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather2pr8.Classes
{
    public class Context: DbContext
    {
        public DbSet<DBPeriodSummary> Requests { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost;user=root;password=strong_password_123;database=pr8;", new MySqlServerVersion(new Version(8, 0, 30)));
        }
        public Context()
        {
            Database.EnsureCreated();
        }
    }
}
