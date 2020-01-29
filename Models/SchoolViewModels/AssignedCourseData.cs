using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity2020.Models.SchoolViewModels
{
    public class AssignedCourseData
    {
        /* This ViewModel Class will be used to add course 
         * assignments to the Insructor (Edit|Create) pages.
         * It will be provide a list of course checkboxes with
         * CourseID and CourseTitle as well as an indicator
         * that the instuctor is assigned or not assigned to a prticular course
         * 
         * The view will genetate checkboxes for each course
         * <input type="checkbox" name="selectedCourses
         * id="1200" value="1200" checked>
         * Calculus
         * So we will need to create two new properties 
         * -one foe the course title
         * -a second for the course id
         * -a third for checked|not checked flag
         */
        public int CourseID { get; set; } //for the course id
        public string Title { get; set; } //for course title
        public bool Assigned { get; set; } //for checked| not checked (is instructor assigned)
    }
}
