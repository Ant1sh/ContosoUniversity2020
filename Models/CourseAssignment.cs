namespace ContosoUniversity2020.Models
{
    public class CourseAssignment
    {
        public int InstructorID { get; set; }//Composite PK (with CourseID), FK to Insrtuctor entity
        public int CourseID { get; set; }//composite pk (with InstructorID) , FK to Course Entity


        /*
         * note:
         * The only way to identity a composite pk when  using ef(entity framework) is by using the 
         * fluent API whithin the Database Context Class (which will be called SchoolContwxt in this case
         */

        /*
         * nav properties
         * many-many (this is the junction table between course and instructor)
         * many instructors teaches many courses
         *
        */
        public virtual Instructor Instructor { get; set; }
        public virtual Course Course { get; set; }
    }
}