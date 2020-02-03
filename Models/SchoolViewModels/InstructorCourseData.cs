using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity2020.Models.SchoolViewModels
{
    public class InstructorCourseData
    {
        public int ID { get; set; }// for Instructor (ID)
        public string Email { get; set; }//for Instructor Email
        public string FullName { get; set; } //for Instructor FirstName + " " + LastName
        public int CourseID { get; set; } //for Course ID (that instructor is teaching)
        public string Title { get; set; } //for Course Title (that instructor is teaching)
    }
}
