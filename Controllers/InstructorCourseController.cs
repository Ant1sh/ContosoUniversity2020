using System;
using System.Collections.Generic;
using System.Data.Common; //need for ADO.NET
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity2020.Data; //need school contect
using ContosoUniversity2020.Models;
using ContosoUniversity2020.Models.SchoolViewModels; //need school view models
using Microsoft.AspNetCore.Authorization; //need authorization
using Microsoft.AspNetCore.Identity; //need identity
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; //need EF

namespace ContosoUniversity2020.Controllers
{
    [Authorize(Roles = "Instructor")] //1.  mwilliams - Only instructors
    public class InstructorCourseController : Controller
    {
        //2.  Private members (need school context and Identity user)
        private readonly SchoolContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        //3. mwilliams - Constructor
        public InstructorCourseController(SchoolContext context,
                                          UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //4.  mwilliams:  function to return currently logged in user
        private Task<IdentityUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        //5.  mwilliams:  Index Action
        public async Task<IActionResult> Index(int? id) //convert to async task and need id param (which course)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
                //To do :  return some kind of Error View instead
                //return View ("Error")
            }

            var instructor = await _context.Instructors
                            //.Include(i => i.Courses) //using EF needed for this      
                .SingleOrDefaultAsync(m => m.Email == user.Email);  //associate identity user -> student (using email property)

            //Get the Instructor Name as well for use in view
            ViewData["InstructorName"] = instructor.FullNameAlt;

            //Populate the InstructorCourseData View Model 
            //using ADO.NET code (using RAW SQL query that returns other types)
            List<InstructorCourseData> data = new List<InstructorCourseData>();

            //===================== Now for the ADO.NET stuff ==========================//

            //1.  Get the database connection
            var conn = _context.Database.GetDbConnection();

            try
            {
                //2.  Open the db connection
                await conn.OpenAsync();

                //3.  Create a command object
                //    command object holds the query and 
                //    executes (does the work)
                using (var command = conn.CreateCommand())
                {
                    string query = @"SELECT ID, Email, FirstName + ' '  + LastName as FullName, Course.CourseID, Title
                                 FROM Person JOIN CourseAssignment ON ID = InstructorID
                                             JOIN Course ON Course.CourseID = CourseAssignment.CourseID
                                 WHERE Email = @Email";

                    //4. Set up command object
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = query;

                    //5.  Create the parameter object
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@Email";
                    parameter.Value = user.Email;
                    command.Parameters.Add(parameter);

                    //6.  Create Data Reader object (and get the data)
                    DbDataReader reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);

                    //7. First check if any rows in reader
                    if (reader.HasRows)
                    {
                        //8.  Read:  Loop all the rows 
                        while (await reader.ReadAsync())
                        {
                            //create instance of InstructorCourseData 
                            //and populate it
                            var row = new InstructorCourseData
                            {
                                ID = reader.GetInt32(0),
                                Email = reader.GetString(1),
                                FullName = reader.GetString(2),
                                CourseID = reader.GetInt32(3),
                                Title = reader.GetString(4)
                            };
                            //each each row to list of data
                            data.Add(row);
                        }
                    }
                    //clean up 
                    reader.Close();

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //finally
            //{
            //    //close the database connection
            //    conn.Close();
            //}

            //Check for incoming parameter (id :  for course id)
            if (id != null)
            {
                //get selected course
                var selectedCourse = _context.Courses
                    .Where(x => x.CourseID == id)
                    .SingleOrDefault();

                if (selectedCourse == null)
                {
                    return NotFound();
                }

                //get all enrolled students within the selected course
                var enrollments = _context.Enrollments
                    .Include(e => e.Student)
                    .Where(e => e.CourseID == selectedCourse.CourseID);


                //return both items above in view data
                ViewData["Enrolled"] = enrollments.ToList();
                ViewData["Course"] = selectedCourse.Title;
                //Return the course id (id) back to the view for highlighting the selected row
                ViewData["CourseID"] = id.Value;
            }

            //return the view with data attached
            return View(data.ToList());
        }

        //GET:  InstructorCourse/Edit/1
        public async Task<IActionResult> Edit(int? id, string mode)
        {
            //check for incoming parameter
            if (id == null)
            {
                return NotFound();
            }

            //get that enrollment 
            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .SingleOrDefaultAsync(e => e.EnrollmentID == id);

            //check if we have an enrollment 
            if (enrollment == null)
            {
                return NotFound();
            }

            //otherwise return the view and attach the enrollment
            ViewData["mode"] = mode;
            return View(enrollment);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            //check for incoming parameter
            if (id == null)
            {
                return NotFound();
            }

            //Find the enrollment record to be updated
            var enrollmentToUpdate = await _context.Enrollments
                .SingleOrDefaultAsync(e => e.EnrollmentID == id);

            if (await TryUpdateModelAsync<Enrollment>(
                    enrollmentToUpdate, "", e => e.EnrollmentID,
                   /* e=>e.CourseID,*/ e => e.Grade/*,e=>e.StudentID*/))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { id = enrollmentToUpdate.CourseID });
                }
                catch (DbUpdateException)
                {

                    ModelState.AddModelError("", "Error saving changes!");
                }
            }
            return View(enrollmentToUpdate);
        }
    }
}