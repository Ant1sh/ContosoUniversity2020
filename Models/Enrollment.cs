using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity2020.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; } //PK

        [Display(Name ="Course")]
        public int CourseID { get; set; } //FK for Course Entitiy

        [Display(Name = "Student")]
        public int StudentID { get; set; }//FK for studentid entity
        [DisplayFormat(NullDisplayText ="No Grade Yet")]//Display "no Grade yet when grade is null
        public Grade? Grade { get; set; }//? means nullable,we dont start with a grade upon registration

        //navigation properties
        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }
    public enum Grade
    {
        A, B, C, D, F
    }
    
}