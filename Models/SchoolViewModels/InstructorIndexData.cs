using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity2020.Models.SchoolViewModels
{
    public class InstructorIndexData
    {
        //part 8 : creating view models 
        //1. Instructor related records
        // - Instructor , Course , Enrollment

        //The instructor Index Viwe will show data from three different tables
        //(models|entities) so for this reson we are creating this viewmodel
        //tthis will include this following
        public IEnumerable <Instructor> Instructors { get; set; }
        public IEnumerable <Course> Courses { get; set; }
        public IEnumerable <Enrollment> Enrollments { get; set; }
    }
}
