using ContosoUniversity2020.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity2020.Data
{
    //mwilliams:  Part 5: Seeding the database (SchoolContext)
    public class DbInitializer
    {
        //1.  Create the DbInitializer Class

        //2.  Create the Initialize Method
        public static void Initialize(SchoolContext context)
        {
            //Look for any Enrollment records 
            if (context.Enrollments.Any()) { 
                return; //DB has been seeded already (stop processing this script)
            }

            //1.====================== STUDENTS ======================//
            var students = new Student[]
            {
                new Student
                { 
                    FirstName="Carson",
                    LastName = "Alexander",
                    Email="calexander@contoso.com",
                    EnrollmentDate = DateTime.Parse("2020-01-20"),
                    Address = "4 Flanders Court",
                    City = "Moncton",
                    Province = "NB",
                    PostalCode = "E1C 0K6"
                },
                new Student
                {
                    FirstName="Meridith",
                    LastName = "Alonso",
                    Email="malonso@contoso.com",
                    EnrollmentDate = DateTime.Parse("2020-01-20"),
                    Address = "25 Garden Hill Ave.",
                    City = "Moncton",
                    Province = "NB",
                    PostalCode = "E1C 3E2"
                },
                new Student
                {
                    FirstName="Arturo",
                    LastName = "Anand",
                    Email="aanand@contoso.com",
                    EnrollmentDate = DateTime.Parse("2020-01-20"),
                    Address = "205 Argyle Street",
                    City = "Moncton",
                    Province = "NB",
                    PostalCode = "E1C 8V2"
                }
            };

            foreach (Student s in students)
            {
                context.Students.Add(s); //Add each student to Students collection
            }
            //Save changes to db context
            context.SaveChanges();


            //2.==================== INSTRUCTORS ====================//
            var instructors = new Instructor[]
            {
                new Instructor
                {
                    FirstName="Marc",
                    LastName = "Williams",
                    Email="mwilliams@faculty.contoso.com",
                    HireDate = DateTime.Parse("1996-08-01"),
                    Address = "4 Flanders Court",
                    City = "Moncton",
                    Province = "NB",
                    PostalCode = "E1C 0K6"
                },
                new Instructor
                {
                    FirstName="John",
                    LastName = "Smith",
                    Email="jsmith@faculty.contoso.com",
                    HireDate = DateTime.Parse("2000-09-01"),
                    Address = "4 Flanders Court",
                    City = "Moncton",
                    Province = "NB",
                    PostalCode = "E1C 0K6"
                },
                new Instructor
                {
                    FirstName="Frank",
                    LastName = "Bekkering",
                    Email="fbekkering@faculty.contoso.com",
                    HireDate = DateTime.Parse("1995-09-01"),
                    Address = "4 Flanders Court",
                    City = "Moncton",
                    Province = "NB",
                    PostalCode = "E1C 0K6"
                }
            };
            foreach (Instructor i in instructors)
            {
                context.Instructors.Add(i); //Add each instructor to Instructors collection
            }
            //Save changes to db context
            context.SaveChanges();

            //3.==================== DEPARTMENTS ====================//
            var departments = new Department[]
            {
                new Department
                {
                    Name = "English",
                    Buget = 350000,
                    CreateDate = DateTime.Parse("1996-01-01"),
                    InstructorID = instructors.Single(i=>i.LastName =="Smith").ID //Assign Smith to this department                   
                },
                 new Department
                {
                    Name = "Mathematics",
                    Buget = 550000,
                    CreateDate = DateTime.Parse("1996-01-01"),
                    InstructorID = instructors.Single(i=>i.LastName =="Bekkering").ID //Assign Bekkering to this department                   
                }
            };
            foreach (Department d in departments)
            {
                context.Departments.Add(d); //Add each department to Departments collection
            }
            //Save changes to db context
            context.SaveChanges();

            //4.====================== COURSES ======================//
            var courses = new Course[]
            {
                new Course
                {
                    CourseID = 1050,
                    Title = "Composition",
                    Credits = 3,
                    DepartmentID= departments.Single(d=>d.Name=="English").DepartmentID
                },
                new Course
                {
                    CourseID = 1200,
                    Title = "Calculus",
                    Credits = 3,
                    DepartmentID= departments.Single(d=>d.Name=="Mathematics").DepartmentID
                }
            };
            foreach (Course c in courses)
            {
                context.Courses.Add(c); //Add each department to Departments collection
            }
            //Save changes to db context
            context.SaveChanges();

            //5.================ OFFICE ASSIGNMENTS =================//
            var officeAssignments = new OfficeAssignment[]
            {
                new OfficeAssignment
                {
                    InstructorID = instructors.Single(i=>i.LastName=="Williams").ID,
                    Location =  "Dragen's Den"
                },

            };
            foreach (OfficeAssignment o in officeAssignments)
            {
                context.OfficeAssignments.Add(o); //Add each department to Departments collection
            }
            //Save changes to db context
            context.SaveChanges();

            //6.================ COURSE ASSIGNMENTS =================//
            var courseAssignments = new CourseAssignment[]
            {
                new CourseAssignment
                {
                    CourseID = courses.Single(c=>c.Title=="Composition").CourseID,
                    InstructorID = instructors.Single(i=>i.LastName=="Smith").ID
                },
                 new CourseAssignment
                {
                    CourseID = courses.Single(c=>c.Title=="Calculus").CourseID,
                    InstructorID = instructors.Single(i=>i.LastName=="Bekkering").ID
                }
            }; 
            foreach (CourseAssignment c in courseAssignments)
            {
                context.CourseAssignments.Add(c); //Add each department to Departments collection
            }
            //Save changes to db context
            context.SaveChanges();

            //7.==================== ENROLLMENT =====================//
            var enrollments = new Enrollment[]
            {
                new Enrollment
                {
                    StudentID = students.Single(s=>s.LastName=="Alexander").ID,
                    CourseID = courses.Single(c=>c.Title =="Composition").CourseID,
                    Grade = Grade.A
                },
                new Enrollment
                {
                    StudentID = students.Single(s=>s.LastName=="Alonso").ID,
                    CourseID = courses.Single(c=>c.Title =="Calculus").CourseID     
                },
                new Enrollment
                {
                    StudentID = students.Single(s=>s.LastName=="Anand").ID,
                    CourseID = courses.Single(c=>c.Title =="Calculus").CourseID
                }
            };
            foreach (Enrollment e in enrollments)
            {
                context.Enrollments.Add(e); //Add each department to Departments collection
            }
            //Save changes to db context
            context.SaveChanges();

        }//End Initialize Method
    }//End Class
}//End Namespace
