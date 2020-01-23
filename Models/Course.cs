using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity2020.Models

{
    public class Course
    {
        //remove the default identity Property(autonumber feature)
        //choises are - Computed, Identity, or None
        //Computerd: Database generates a value when a row is inserted or updated
        //Identity: Database generates a value when row is inserted
        //None: Database does not generate a valued

        //we want the user to enter the course id manually
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name="Course Number")]
        [Required]
        public int CourseID { get; set; }//PK
        [Required]
        [StringLength(50,MinimumLength =3)]
        public string Title { get; set; }
        [Range(3,6)]
        public int Credits { get; set; }
        [Display(Name ="Department")]
        public int DepartmentID { get; set; } //FK to department entity

        //read only property: return the course id and title
        public string CourseIdTite
        {
            get
            {
                return CourseID + ": " + Title;
                //1021: Intro to c# programming
            }
        }

        //navigation Property
        public virtual ICollection<Enrollment> Enrollments { get; set; }

        public virtual Department Department { get; set; }// A course can only belong to at most one department
    }
}