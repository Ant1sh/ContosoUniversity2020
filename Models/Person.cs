using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity2020.Models
{
    public abstract class Person
    {
        //The ID property name will become the pk column of the databse
        //by default the entity framework(EF) interprets a property named "ID" or
        //"ClassnameID"
        public int ID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(65, ErrorMessage = "Last name cannot be longer than 65 characters")]
        [Display(Name = "LastName")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)] //nvarchar(max)
        [StringLength(85, ErrorMessage = "Email cannot be longer than 85 characters")]
        public string Email { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Address cannot be longer than 150 characters")]
        public string Address { get; set; }
        [Required]
        [StringLength(60)]
        public string City { get; set; }
        [Required]
        [StringLength(2)]
        [Column(TypeName = "nchar(2)")]
        public string Province { get; set; }
        [Required]
        [StringLength(7)]
        [Column(TypeName = "nchar(7)")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "PostalCode")]
        public string PostalCode { get; set; }

        //some read only properties

        [Display(Name ="Name")]
        public string FullName { 
        get{
            return LastName + ", " + FirstName;
                //Chartovich Anton
            }
        }

        [Display(Name = "Name")]
        public string FullNameAlt
        {
            get
            {
                return FirstName + " " + LastName;
                //Anton Chartovich
            }
        }

        public string IdFullName
        {
            get
            {
                return "(" + ID + ") " + FullName;
                //(1)Anton Chartovich
            }
        }

        public string FullAddress
        {
            get
            {
                return Address + " " + City + " " + Province + " " + PostalCode + " ";
              
            }
        }









    }
}
