using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Infrastructure.Models;
using SendGrid;
using Microsoft.Graph.Models;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace SchedulingSystemWeb.Pages.Tutor.Home
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public DeleteModel(UnitOfWork unit, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unit;
            _userManager = userManager;
        }
        public Booking bookings { get; set; }
        public ApplicationUser Teacherinfo { get; set; }
        public ApplicationUser student { get; set; }
        public List<string> Role { get; set; }
        [Required]
        [BindProperty]
        public string CancelReason { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ApplicationUser user = _unitOfWork.ApplicationUser.Get(i => i.Id == _userManager.GetUserId(User));
            Role = (List<string>)await _userManager.GetRolesAsync(user);
            string first = Role.First();
            bookings = _unitOfWork.Booking.Get(b => b.Id == id);
            ProviderProfile prov;
            prov = _unitOfWork.ProviderProfile.Get(p => p.Id == bookings.ProviderProfileID);
            
            
            Teacherinfo = _unitOfWork.ApplicationUser.Get(u => u.Id == prov.User);
            student = _unitOfWork.ApplicationUser.Get(b => b.Id == bookings.User);
            return Page();
        }
        public async Task<IActionResult> OnPost(int? id)
        {
            
            bookings = _unitOfWork.Booking.Get(b => b.Id == id);
            _unitOfWork.Booking.Delete(bookings);
            _unitOfWork.Commit();
            await SendEmail(id);
            var user = _unitOfWork.ApplicationUser.Get(i => i.Id == _userManager.GetUserId(User));
            Role = (List<string>)await _userManager.GetRolesAsync(user);
            if (Role.First() != "TEACHER")
            {
                return Redirect("/Tutor/Home");
                
            }
            else
            {
                return Redirect("/Tutor/Home");
            }
        }
        public async Task SendEmail(int? id)
        {
            ApplicationUser user = _unitOfWork.ApplicationUser.Get(i => i.Id == _userManager.GetUserId(User));
            Role = (List<string>)await _userManager.GetRolesAsync(user);
        
            ProviderProfile p = _unitOfWork.ProviderProfile.Get(p => p.Id == bookings.ProviderProfileID);
            Teacherinfo = _unitOfWork.ApplicationUser.Get(u => u.Id == p.User);
            student = _unitOfWork.ApplicationUser.Get(b => b.Id == bookings.User);
            var sendgridclient = new SendGridClient("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");
            if (Role.First() == "STUDENT || TUTOR") // STUDENT 
            {
                var from = new SendGrid.Helpers.Mail.EmailAddress("CodemonkeysScheduling@outlook.com", "Code Monkeys");
                var to = new SendGrid.Helpers.Mail.EmailAddress(Teacherinfo.Email, Teacherinfo.FirstName + " " + Teacherinfo.LastName);
                var subject = "Booking Canceled!";
                var plainText = student.FirstName + " " + student.LastName + " Cancelled their appointment with you on " + bookings.StartTime.ToShortDateString() + " at " + bookings.StartTime.ToShortTimeString()
                    + " With the reason of: " + CancelReason;
                var htmlContent = "<p>" + plainText + "</p>";
                var message = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);
                var response = await sendgridclient.SendEmailAsync(message); //Teacher Message
                to = new SendGrid.Helpers.Mail.EmailAddress(student.Email, student.FirstName + " " + student.LastName);
                plainText = "You successfully cancelled your booking with " + Teacherinfo.FirstName + " " + Teacherinfo.LastName + " on " + bookings.StartTime.ToShortDateString() + " at " + bookings.StartTime.ToShortTimeString();
                htmlContent = "<p>" + plainText + "</p>";
                message = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);
                response = await sendgridclient.SendEmailAsync(message); //Student Message
            }
            else //TEACHER
            {
                var from = new SendGrid.Helpers.Mail.EmailAddress("CodemonkeysScheduling@outlook.com", "Code Monkeys");
                var to = new SendGrid.Helpers.Mail.EmailAddress(Teacherinfo.Email, Teacherinfo.FirstName + " " + Teacherinfo.LastName);
                var subject = "Booking Canceled!";
                var plainText =  "You Cancelled your booking with " + student.FirstName + " " + student.LastName + " At "+  bookings.StartTime.ToShortDateString() + " at " + bookings.StartTime.ToShortTimeString();
                var htmlContent = "<p>" + plainText + "</p>";
                var message = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);
                var response = await sendgridclient.SendEmailAsync(message); //Teacher Message
                to = new SendGrid.Helpers.Mail.EmailAddress(student.Email, student.FirstName + " " + student.LastName);
                plainText = Teacherinfo.FirstName + " " + Teacherinfo.LastName + " Cancelled their appointment with you on " + bookings.StartTime.ToShortDateString() + " at " + bookings.StartTime.ToShortTimeString()
                    + " With the reason of: " + CancelReason;
                htmlContent = "<p>" + plainText + "</p>";
                message = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);
                response = await sendgridclient.SendEmailAsync(message); //Student Message
            }           
        }
    }
}
