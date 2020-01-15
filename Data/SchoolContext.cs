using ContosoUniversity2020.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity2020.Data
{
    public class SchoolContext : DbContext
    {
        //anton: part3 : create the data base

        //default constructor
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {

        }
        //Specify Entiset Sets - corresponding to the database 
        //be created upon database migration.Each single entity corresponds
        //to a row in a table
        public DbSet<Person> People { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }

        /*
         * Plural table names are thr defaul:
         * When the database is created (from migration process)
         */
         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //plural table names -> singular table names
            //note: since student and instructor are inheriting from person
            //EF will only create the person table
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");

            //2. composite PK on CourseAssignment (CourseID, InstructorID)
            modelBuilder.Entity<CourseAssignment>()
                .HasKey(c => new { c.CourseID, c.InstructorID });

        }


    }
}
