using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity2020.Models
{
    public class Instructor : Person //instructor inherits from person
    {
        [DataType(DataType.Date)]//This will create a date picker in HTML
        [Display(Name = "EnrollmentDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }

        //=============================================Navigation prop
        //instructor can teach any number of courses so courses is defined as a collection of the courseassihn entity
        public virtual ICollection<CourseAssignment> Courses { get; set; }
        public virtual OfficeAssignment OfficeAssignment { get; set; }
    }
}
