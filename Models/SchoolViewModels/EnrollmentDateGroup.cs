using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity2020.Models.SchoolViewModels
{
    public class EnrollmentDateGroup
    {
        //part 7 : creating view models 
        // student statistics
        //this class is considered to be a ViewModel. A viewmodel allow allows you to share multiple entities
        //into a single object optimized for consumption and rendering by the view.
        //the porpouse of the ViewModel is for the view to have a single object to render.

        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate { get; set; }
        //data annptations datatype property above us will be used for formatting within view
        //without it date look like this 1/21/2020 12:00:00AM
        //with it it will look like this 1/21/2020
        public int StudentCount { get; set; }

    }
}
