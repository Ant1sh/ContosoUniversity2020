using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity2020.Models
{
    public class OfficeAssignment
    {
        [Key]
        public int InstructoiID { get; set; }//PK
        [Display(Name ="Office Loaction")]
        [StringLength(60)]
        public string Location { get; set; }

        //navigation properties
        public virtual Instructor Instructor { get; set; }//1:1 (One Instructor assigned to one office

    }
}