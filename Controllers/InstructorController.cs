using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity2020.Data;
using ContosoUniversity2020.Models;
using ContosoUniversity2020.Models.SchoolViewModels;
using Microsoft.AspNetCore.Authorization;

namespace ContosoUniversity2020.Controllers
{
    //mwilliams:  Part 12:  Authorization (securing admin controllers)
    [Authorize(Roles = "Admin")]
    public class InstructorController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Instructor

        //mwilliams:  Part 8:  Creating View Models
        //1.  Instructor related records (instructors, courses, enrollments)
        //    which is in the ViewModel (InstructorIndexData)
        //    Replace the old Index method with this one
        //public async Task<IActionResult> Index()
        public async Task<IActionResult> Index(int? id, int? courseID)
        //Part 4:  Which instructor was selected (added the id param)
        {     //Part 5:  Which course was selected (added the courseID param)
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment) //1.  Part 1:  Get Instructors including office assignments
                .Include(i => i.Courses)          //2.  Part 2.  Get the Courses
                    .ThenInclude(i => i.Course)       //    Have to get the Course Entity out of the Courses JOIN Entity  
                    .ThenInclude(i => i.Department)   //3.  Part 3.  Get the Department (to show the Department Name)
                .ToListAsync();

            //=========================== INSTRUCTOR SELECTED ===================================
            if (id != null)
            {//if the id instructor param is passed in
                //Get the instructor data
                Instructor instructor = viewModel.Instructors.Where(
                    i => i.ID == id.Value).SingleOrDefault();  //Get a single instructor that matches id param

                // to do:  check if we have a valid instructor
                if (instructor == null)
                {
                    return NotFound(); //send to 404 page
                }

                //Now get the instructor courses (course assignments)
                viewModel.Courses = instructor.Courses.Select(s => s.Course);

                //Get the instructor name for display within view (using ViewData)
                ViewData["InstructorName"] = instructor.FullName;

                //Return the instructor id (id) back to the view for highlighting the selected row
                ViewData["InstructorID"] = id.Value;
            }
            //========================= END INSTRUCTOR SELECTED =================================

            //============================= COURSE SELECTED =====================================
            if (courseID != null)
            {//if the courseID course param is passed in
                //Get the enrollment data
                _context.Enrollments.Include(i => i.Student)
                    .Where(c => c.CourseID == courseID.Value).Load();//Explicit Loading
                //Only enrollments for a single selected course (courseID = 1050)
                //We do not want all enrollments in this case, for example:
                //viewModel.Enrollments = _context.Enrollments

                var enrollments = viewModel.Courses
                    .Where(x => x.CourseID == courseID).SingleOrDefault();

                if (enrollments == null)
                {
                    return NotFound(); //404 page
                }
                //Populate the view model with enrollments (to pass back to View)
                viewModel.Enrollments = enrollments.Enrollments;

                //Pass back the course id to the view using ViewData for selected row CSS
                ViewData["CourseID"] = courseID.Value; //this is the URL parameter called courseID

            }
            //=========================== END COURSE SELECTED ===================================

            //return view with instructor index data
            return View(viewModel);
        }
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Instructors.ToListAsync());
        //}

        // GET: Instructor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructor/Create
        //mwilliams Part 9: Update related records (instructor)
        public IActionResult Create()
        { 
            //new code 
            //Get course assignments (for this instructor - the one we are currently editing)
            //Create the instructor object
            var instructor = new Instructor();
            //create list of courses
            instructor.Courses = new List<CourseAssignment>();
           
            PopulateAssignedCourseData(instructor); //using ViewBag object to use within create view
      
            //end new 

            return View();
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            //1.Get all courses
            var allCourses = _context.Courses;

            //2. Create a hash set of instructor courses (Hashset of integers with course id)
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));

            //3. Instantiate and populate the AssignedCourseData View Model Class
            var viewModel = new List<AssignedCourseData>(); //instantiate

            //populate it
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }
            //Save the view model within the ViewData object for use within view
            ViewData["Courses"] = viewModel;

        }//End PopulateAssignedCourseData

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HireDate,OfficeAssignment,FirstName,LastName,Email,Address,City,Province,PostalCode")] Instructor instructor,
                                               string[]selectedCourse /*mwilliams:  added course assignments*/ )
        {

            //mwilliams Part 9: Update related records (course assignments)
            //check for selectedCourse
            if(selectedCourse!= null)
            {
                //some checkboxes has been checked - create a list of CourseAssignment
                instructor.Courses = new List<CourseAssignment>();

                //Loop through the array of selectedCourse
                foreach(var course in selectedCourse)
                {
                    //Populate the CourseAssignment object
                    var courseToAdd = new CourseAssignment
                    {
                        InstructorID = instructor.ID,
                        CourseID = int.Parse(course)
                    };
                    instructor.Courses.Add(courseToAdd);  //Add to collection
                }
            }
            //end course assignments
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //mwilliams Part 9: Update related records (instructor)
            //var instructor = await _context.Instructors.FindAsync(id);
            var instructor = await _context.Instructors
                             .Include(i => i.OfficeAssignment)//include office assignments (part 1)
                             .Include(i=>i.Courses)//include course assignments (part 2)
                             .SingleOrDefaultAsync(i => i.ID == id.Value);//get a single instructor
            if (instructor == null)
            {
                return NotFound();
            }

            //part 2:  Get course assignments (for this instructor - the one we are currently editing)
            PopulateAssignedCourseData(instructor);
            //end part 2

            return View(instructor);
        }

        //part 2:  Get course assignments
       

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //mwilliams Part 9: Update related records (overposting check)
        //public async Task<IActionResult> Edit(int id, [Bind("HireDate,ID,FirstName,LastName,Email,Address,City,Province,PostalCode")] Instructor instructor)
        public async Task<IActionResult> Edit(int? id,string[] selectedCourse)
        {
            if (id==null)
            {
                return NotFound();
            }

            //overposting check 
            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)//include office assignment -part 1
                .Include(i=>i.Courses) //include courses (for course assignment) - part2
                .ThenInclude(i=>i.Course)//include course for update - part 2
                .SingleOrDefaultAsync(i => i.ID == id.Value); //for only one instructor

            //try to update the instructor
            if(await TryUpdateModelAsync<Instructor>(
                    instructorToUpdate,"",
                    i=>i.FirstName,i=>i.LastName, i=>i.Address,
                    i => i.City, i => i.Province, i => i.PostalCode,
                    i => i.Email, i => i.HireDate,
                    i => i.OfficeAssignment)
                )
            {
                //Check for empty office location 
                if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;//completely  remove record
                }

                //Part 2:  Course Assignment
                UpdateInstructorCourses(selectedCourse, instructorToUpdate);

            }

            //save changes back to database
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes!");
                }
                //Success - return to index view of Instructor
                return RedirectToAction(nameof(Index));
            }
            //Failed validation - return to the Edit View of instructor
            return View(instructorToUpdate);
        }

        //Part 2:  Update Related Data (Instructor Assigned Courses)
        private void UpdateInstructorCourses(string[] selectedCourse, Instructor instructorToUpdate)
        {
            if (selectedCourse == null)
            {
                //If no checkboxes were selected, initialize the Courses navigation property
                //with an empty collection and return
                instructorToUpdate.Courses = new List<CourseAssignment>();
                return;
            }

            //To facilitate efficient lookups, 2 collections will be stored in HashSet objects
            //: selectedCourseHS ->  selected course (hashset of checkboxe selections)
            //: instructorCourses -> instructor courses (hashset of courses assigned to instructor)
            var selectedCourseHS = new HashSet<string>(selectedCourse);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.Courses.Select(c => c.Course.CourseID));

            //Loop through all courses in the database and check each course against the ones
            //currently assigned to the instructor versus the ones that were selected in the
            //view
            foreach (var course in _context.Courses)//Loop all courses
            {
                //CONDITION 1:
                //If the checkbox for a course was selected but the course isn't in the 
                //Instructor.Courses navigation property, the course is added to the collection
                //in the navigation property
                if (selectedCourseHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(new CourseAssignment
                        {
                            InstructorID = instructorToUpdate.ID,
                            CourseID = course.CourseID
                        });
                    }
                }
                //CONDITION 2:
                //If the check box for a course wasn't selected, but the course is in the 
                //Instructor.Courses navigation property, the course is removed 
                //from the navigation property.
                else
                {
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove =
                            instructorToUpdate.Courses
                            .SingleOrDefault(i => i.CourseID == course.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }

            }//end foreach
        }//end UpdateInstructorCourses Method

        // GET: Instructor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            try
            {
                _context.Instructors.Remove(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
      
                if( ((Microsoft.Data.SqlClient.SqlException)ex.InnerException).Number==547   )
                {
                    //FK constraint error (cascade restrict)
                    ModelState.AddModelError("", "Unable to delete instructor due to related records!");
                }
                else
                {
                    //Some other error 
                    ModelState.AddModelError("", "Unable to delete instructor due to a system error!");
                }
                
            }
            //failed to update return back to view attaching the instructor object
            return View(instructor);
            
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }
    }
}
