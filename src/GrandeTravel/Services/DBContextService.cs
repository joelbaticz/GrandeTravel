using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using GrandeTravel.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GrandeTravel.Services
{
    public class DbContextService: IdentityDbContext<ApplicationUser>
    {
        /*
        public DbContextService(DbContextOptions<DBContextService> options) : base(options)
        {
        }
        */

        protected override void OnConfiguring(DbContextOptionsBuilder option)
        {
            option.UseSqlServer(@"Server=(localdb)\mssqllocaldb ;Database= GrandeTravel; Trusted_Connection=True");
        }

        public DbSet<Provider> tblProvider { get; set; }
        public DbSet<Package> tblPackage { get; set; }
        public DbSet<Customer> tblCustomer { get; set; }
        public DbSet<Feedback> tblFeedback { get; set; }
        public DbSet<Booking> tblBooking { get; set; }

    }
}
