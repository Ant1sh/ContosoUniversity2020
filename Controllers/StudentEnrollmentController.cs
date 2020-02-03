using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity2020.Data;
using ContosoUniversity2020.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ContosoUniversity2020.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentEnrollmentController : Controller
    {
        private readonly SchoolContext _context;
        private readonly UserManager<IdentityUser> _userManager;//1. need identity user

        public StudentEnrollmentController(SchoolContext context,
                                           UserManager<IdentityUser> userManager)//2. need identity user
        {
            _context = context;
            _userManager = userManager;//3. need identity user
        }
        //4. method to return currently logged in user
        private Task<IdentityUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        // GET: StudentEnrollment
        public async Task<IActionResult> Index()
        {
            //5. retreve currently logged in user
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                //not logged in
                return NotFound();//note: could return some kind of error view here
            }
            //6. locate the logged in user (student) whithin the Student entity
            var student = await _context.Students//need enrollmenta for that student
                .Include(s => s.Enrollments)//need enrollments for this student
                .ThenInclude(s => s.Course)//need course info too
                .AsNoTracking()//remove caching of database object context (it will not be changed)
                .SingleOrDefaultAsync(s => s.Email == user.Email);//only for the logged in student
            //7. Courses enrolled (courses that current student is enrolled in)
            var studentEnrollments = _context.Enrollments
                .Include(c => c.Course)//need course in 
                .Where(c => c.StudentID == student.ID)//only for given student
                .AsNoTracking();//no caching
            //Get the student naME FOR DISPLAY IN VIEW USING VIEW DATA
            ViewData["StudentName"] = student.FullName;
            //8. courses avaliable (courses that current stuent is not enrolled in)
            //build a RAW sql query using LINQ for this demo
            string query = @"Select CourseID, Credits, Title, DepartmentID
                           From Course
                           Where CourseID NOT IN(SELECT DISTINCT CourseID
                           From Enrollment
                           Where StudentID = {0})";

            var courses = _context.Courses.FromSqlRaw(query, student.ID).AsNoTracking().ToList();
            //send course back to view using ViewBag
            ViewData["Courses"] = courses;
            //9. return th view with enrollment data



            //var schoolContext = _context.Enrollments.Include(e => e.Course).Include(e => e.Student);
            return View(await studentEnrollments.ToListAsync());
        }

        public async Task<IActionResult> Enroll(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Get currently logged in student user
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                //user not found -404
                return NotFound();
            }
            //if we get this far, we have a logged in user - locate the corrsponding student for this user 
            var student = await _context.Students
              .Include(s => s.Enrollments)
              .AsNoTracking()
              .SingleOrDefaultAsync(s => s.Email == user.Email);
            //send data back to view (view data) for form hidden field(so we know who they are)
            ViewData["StudentID"] = student.ID;
            //Retrive this student`s current enrollment
            //(for comparison with the course they want enroll in)
            //Student cannot enroll twice in same course 
            var studentEnrollments = new HashSet<int>(_context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentID == student.ID)
                .Select(e => e.CourseID));
            //check for method parameter
            int currentCourceID;
            if (id.HasValue)//id here is the method parameter (course id)
            {
                currentCourceID = (int)id;
            }
            else
            {
                currentCourceID = 0;
            }
            //handle situation where student tries to enroll in same course
            if (studentEnrollments.Contains(currentCourceID))
            {
                //same cource - send back eroor to view as ModelState error
                ModelState.AddModelError("AlreadyEnrolled", "You are already enrolled in this course!");
            }
            //first find the course
            var course = await
                _context.Courses.SingleOrDefaultAsync(c => c.CourseID == id.Value);
            //handle situation where student tries to enroll non-existing course
            //if course was not instansiated because no course id was found based on param 
            //for example StudentEnrollment/Enroll/5000
            //5000 is not a valid course
            //return not found
            if (course == null)
            {
                return NotFound();
            }
            //return view - attach the cource model to it
            return View(course);


        }//end of get enroll

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll([Bind("CourseID, StudentID")] Enrollment enrollment)
        {
         //add new enrollment object to database contest
         _context.Add(enrollment);
         //Save it to database
         await _context.SaveChangesAsync();
         //return to index view
         return RedirectToAction("Index");
        }//End post enroll

        //get UnEnroll
        public async Task<IActionResult> UnEnroll(int? id, bool? saveChangesError=false)
        {
            

            if (id == null)
            {
                return NotFound();
            }
            //check ifa valid enrollment was found based on id param passed - othervise 404
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }
            //check if enrollment has grade - do not delete it if grade is recorded
            if(enrollment.Grade != null)
            {
                //there was a grade -return error message as ModelState error
                ModelState.AddModelError("HasGrade", "You cannot remove this class because you have a grade!");
                //check if there was an error with post (UnEnroll)
                if (saveChangesError.GetValueOrDefault())
                {
                    //Error was found - return view data with reeoe message
                    ViewData["ErrorMessage"] =
                        "UnEnroll failed. Try again, and if the propblem presists" + "see you system administrator.";
                }
            }

            return View(enrollment);
        }

        //post unenroll
        [HttpPost, ActionName("UnEnroll")]
        public async Task<IActionResult> UnEnrollConfirmed(int EnrollmentID)
        {
            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.EnrollmentID == EnrollmentID);
            if(enrollment == null)
            {
                //Noting was found
                return RedirectToAction(nameof(Index));//send user back to index page
            }
            if(enrollment.Grade != null)
            {
                //Student has grade for this enrollment - cannot delete
                //return model state error back to view
                ModelState.AddModelError("HasGrade", "You cannot remove this because you have a grade!");
                return View(enrollment);
            }
            //if we get this far delete it
            try
            { 
                //remove the enrollment object from database context
                _context.Enrollments.Remove(enrollment);
                //save it back to database
                await _context.SaveChangesAsync();
                //redirect user back to index page
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                //redirect
                return RedirectToAction(nameof(UnEnroll),
                    new { id = EnrollmentID, saveChangesError = true });
            }

        }
                      
    }//end class
}//end namespace






















































//        // GET: StudentEnrollment/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var enrollment = await _context.Enrollments
//                .Include(e => e.Course)
//                .Include(e => e.Student)
//                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
//            if (enrollment == null)
//            {
//                return NotFound();
//            }

//            return View(enrollment);
//        }

//        // GET: StudentEnrollment/Create
//        public IActionResult Create()
//        {
//            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title");
//            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "Address");
//            return View();
//        }

//        // POST: StudentEnrollment/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("EnrollmentID,CourseID,StudentID,Grade")] Enrollment enrollment)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(enrollment);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
//            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "Address", enrollment.StudentID);
//            return View(enrollment);
//        }

//        // GET: StudentEnrollment/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var enrollment = await _context.Enrollments.FindAsync(id);
//            if (enrollment == null)
//            {
//                return NotFound();
//            }
//            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
//            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "Address", enrollment.StudentID);
//            return View(enrollment);
//        }

//        // POST: StudentEnrollment/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentID,CourseID,StudentID,Grade")] Enrollment enrollment)
//        {
//            if (id != enrollment.EnrollmentID)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(enrollment);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!EnrollmentExists(enrollment.EnrollmentID))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
//            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "Address", enrollment.StudentID);
//            return View(enrollment);
//        }

//        // GET: StudentEnrollment/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var enrollment = await _context.Enrollments
//                .Include(e => e.Course)
//                .Include(e => e.Student)
//                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
//            if (enrollment == null)
//            {
//                return NotFound();
//            }

//            return View(enrollment);
//        }

//        // POST: StudentEnrollment/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var enrollment = await _context.Enrollments.FindAsync(id);
//            _context.Enrollments.Remove(enrollment);
//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool EnrollmentExists(int id)
//        {
//            return _context.Enrollments.Any(e => e.EnrollmentID == id);
//        }
//   }
//}
