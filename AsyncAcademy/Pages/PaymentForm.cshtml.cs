﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AsyncAcademy.Data;
using AsyncAcademy.Models;
using Microsoft.EntityFrameworkCore;

namespace AsyncAcademy.Pages
{
    public class PaymentFormModel : PageModel
    {
        public int? currentUserID { get; set; }
        private readonly AsyncAcademy.Data.AsyncAcademyContext _context;

        [ViewData]
        public string NavBarLink { get; set; } = "Course Pages/StudentIndex";

        [ViewData]
        public string NavBarText { get; set; } = "Register";

        [ViewData]
        public string NavBarAccountTabLink { get; set; } = "/Account";

        [ViewData]
        public string NavBarAccountText { get; set; } = "Account";

        [BindProperty]
        public Enrollment Enrollment { get; set; } = default!;

        public List<Course> EnrolledCourses { get; set; } = new List<Course>();

        public Decimal StudentPaymentBalance { get; set; } = 0;

        public Payment PaymentRecord { get; set; } = default!;

        public PaymentFormModel(AsyncAcademy.Data.AsyncAcademyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            currentUserID = HttpContext.Session.GetInt32("CurrentUserId");

            if (currentUserID == null)
            {
                return NotFound(); // Or redirect to a login page
            }

            // Load the enrollment details for the current user
            Enrollment = await _context.Enrollments
                                       .FirstOrDefaultAsync(e => e.UserId == currentUserID);


            EnrolledCourses = await(from Enrollments in _context.Enrollments
                                    join Course in _context.Course
                                    on Enrollments.CourseId equals Course.Id
                                    where Enrollments.UserId == currentUserID
                                    select Course).ToListAsync();

            //Calculate the total amount for all courses:
            decimal totalCourseCost = 0;
            foreach (var course in EnrolledCourses)
            {
                totalCourseCost += course.CreditHours * 100;
            }

            //Calculate the total payments made by the student:
            var totalPaymentsMade = await _context.Payments.Where(p => p.UserId == currentUserID).SumAsync(p => p.AmountPaid);

            //The balance owed is the total course cost minus the payments made by the student:
            StudentPaymentBalance = totalCourseCost - totalPaymentsMade;




            return Page();
        }

        [BindProperty]
        public Payment Payment { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Payments.Add(Payment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
