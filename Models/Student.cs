using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity2020.Models
{
    //Create Data models
    //Create the student class (inheriting from person)
    public class Student : Person
    {
        [DataType(DataType.Date)]//This will create a date picker in HTML
        [Display(Name = "EnrollmentDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EnrollmentDate { get; set; }

       
        //========================================Navigation Property
        /* The enrollments property is a navigation property. Navigation properties hold other 
         * entities that are related to this entity.  In this case, the Enrollments property of 
         * a Student entity will hold all of the Enrollments that are related to that Student.
         * In other words, if a given student row in the database has two related enrollment rows
         * (rows that containt that student's primary key value in their studentid foreign key 
         * column), that student entity's enrollment navigation property will contain those two 
         * enrollment entities.
         * 
         * Navigation properties are typically defined as virtual so that they can take advantage
         * of certain Entity Framework functionality such as as lazy loading.
         * Note:  Lazy Loading is not yet available in EF Core.
         */

        public virtual ICollection<Enrollment> Enrollments { get; set; } //1 student with many enrollments
    }
}
