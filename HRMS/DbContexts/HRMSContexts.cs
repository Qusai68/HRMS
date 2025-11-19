using HRMS.Mod411els;
using HRMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace HRMS.DbContexts
{
    public class HRMSContexts : DbContext
    {
        public HRMSContexts(DbContextOptions<HRMSContexts> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);// Call the Parent  method
                                               // Seeding Database ---
            modelBuilder.Entity<Lookup>().HasData(
                // Employee Posigints (MajorCode=0)
                new Lookup { Id = 1, MajorCode = 0, MinorCode = 0, Name = "Employee Positions" },
                new Lookup { Id = 2, MajorCode = 0, MinorCode = 1, Name = "Developer " },
                new Lookup { Id = 3, MajorCode = 0, MinorCode = 2, Name = "HR" },
                new Lookup { Id = 4, MajorCode = 0, MinorCode = 3, Name = "Manager" },
                // Departmernt Types (MajorCode=1)
                new Lookup { Id = 5, MajorCode = 1, MinorCode = 0, Name = "Department Types" },
                new Lookup { Id = 6, MajorCode = 1, MinorCode = 1, Name = "Finance" },
                new Lookup { Id = 7, MajorCode = 1, MinorCode = 2, Name = "Adminstrative" },
                new Lookup { Id = 8, MajorCode = 1, MinorCode = 3, Name = "Technical" }


                );
            //username is unique
            modelBuilder.Entity<User>().HasIndex(x => x.UserName).IsUnique();

            // UserId in Employee is unique
            modelBuilder.Entity<Employee>().HasIndex(x => x.UserId).IsUnique();

            //seeding admin user
            modelBuilder.Entity<User>().HasData(
                // BCrypt.net.BCrypt.HashPassword("Admin@123")="$2a$11$gDdfB1wrPrM4zSamIkXxYuamPfaRS4ou8gIiWZkMQ3nXMDMR0J4ja"
                new User { Id = 1, UserName = "Admin", IsAdmin = true, HashedPassword = "$2a$11$gDdfB1wrPrM4zSamIkXxYuamPfaRS4ou8gIiWZkMQ3nXMDMR0J4ja" }
                );
        }    

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
