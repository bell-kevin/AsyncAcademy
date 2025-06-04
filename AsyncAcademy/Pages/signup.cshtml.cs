﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AsyncAcademy.Data;
using AsyncAcademy.Models;
using Microsoft.AspNetCore.Identity;

namespace AsyncAcademy.Pages
{
    public class SignupModel : PageModel
    {
        private readonly AsyncAcademyContext _context;

        public SignupModel(AsyncAcademyContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public User Account { get; set; } = default!;

        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            //Create passwordHasher variable that will take data type "User"
            var passwordHasher = new PasswordHasher<User>();

            //Hash the actual password (salt already added on the hasher):
            Account.Pass = passwordHasher.HashPassword(Account, Account.Pass);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!Account.IsProfessor)  // Only for students -> We will create an entry in StudentPayment table
            {
                var newStudentPayment = new Payment
                {
                    UserId = Account.Id,    
                    AmountPaid = 0,          // No classes signed up yet 
                    Timestamp = DateTime.Now
                };

                _context.Payments.Add(newStudentPayment);
                await _context.SaveChangesAsync();
            }

            _context.Users.Add(Account);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("CurrentUserId", Account.Id);
            return RedirectToPage("./welcome");
        }
    }
}
