using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity2020.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        [Required]
        [StringLength(60,MinimumLength =3)]
        public string Name { get; set; }
        [DataType(DataType.Currency)]//client only
        [Column(TypeName ="money")]//sql server money database
        public decimal Buget { get; set; }
        [DataType(DataType.Date)]
        [Display(Name="Date Created")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Administrator")]
        public int InstructorID { get; set; } // a department May have an administrator(instructor)
                                               //and an administrator is always an instructor
        public virtual Instructor Administrator { get; set; }//1 course with many enrollment
        public virtual ICollection<Course> Courses { get; set; }//one department to many courses
    }
}