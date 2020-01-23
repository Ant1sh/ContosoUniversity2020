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

namespace ContosoUniversity2020.Controllers
{
    public class InstructorController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorController(SchoolContext context)
        {
            _context = context;
        }




        // GET: Instructor

        //part 8: Creating view models
        //1.instructor related records (instructors, courses , enrollments)
        //wich is in the viewmodel (instructorindexdata)
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Instructors.ToListAsync());
        //}
        public async Task<IActionResult> Index(int? id, int? courseID)//part 4: Which instructor is selected
        {
            var viewModel = new InstructorIndexData();
            //1. part 1: Get Instructors including office assignments
            viewModel.Instructors = await _context.Instructors
                .Include(i=>i.OfficeAssignment)//part 1: get instructors including office assignment
                .Include(i=>i.Courses)//part2: get the courses
                  .ThenInclude(i=>i.Course)//have to get the course entity out of the courses join entity
                  .ThenInclude(i=>i.Department)//part3: Get the department
                .ToListAsync();

            //==================================INSTRUCTOR SELECTED=========================================//

            if(id != null)
            {//if the id instructor param is passed in 
                //get the instructor data 
                Instructor instructor = viewModel.Instructors.Where(
                    i => i.ID == id.Value).SingleOrDefault();
                //to do : check if we are a valid instructor
                if(instructor == null)
                {
                    return NotFound();
                }
                //now get the instructor courses (course assignment)
                viewModel.Courses = instructor.Courses.Select(s => s.Course);
                //get the instructor name for display within view (using ViewData)
                ViewData["Instructor Name"] = instructor.FullNameAlt;
                //Return the instructor id (id) back to the view for highliting the selected row
                ViewData["InstructorID"] = id.Value;
            }

            //===================================END INSTRUCTOR SELECTED====================//

            //===================================COURSE SELECTED====================//

            if (courseID != null)
            {//if the courseID course param is passed in 
             //get the enrollment data
                _context.Enrollments.Include(i => i.Student)
                       .Where(c => c.CourseID == courseID.Value).Load();//explicit Loading
                //Only enrollments for a single selected course (courseID = 1050)
                //we dont want all enrollments in this case, for example:
                //viewModel.Enrollments = _context.Enrollments
                var enrollments = viewModel.Courses
                    .Where(x => x.CourseID == courseID).SingleOrDefault();
                if (enrollments == null)
                {
                    return NotFound();
                }
                //populate the wiew model with enrollments ( to pass back to view)
                viewModel.Enrollments = enrollments.Enrollments;
                //pass back the course id to the view using ViewData for selected row Css
                ViewData["CourseID"] = courseID.Value;

            }

            //===================================END COURSE SELECTED====================//

            //return view with instructor indext data
            return View(viewModel);
        }

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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HireDate,ID,FirstName,LastName,Email,Address,City,Province,PostalCode")] Instructor instructor)
        {
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

            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HireDate,ID,FirstName,LastName,Email,Address,City,Province,PostalCode")] Instructor instructor)
        {
            if (id != instructor.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }

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
            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }
    }
}
