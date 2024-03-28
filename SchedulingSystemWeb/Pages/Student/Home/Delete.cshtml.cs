using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Infrastructure.Models;
using SendGrid;
using Microsoft.Graph.Models;
using SendGrid.Helpers.Mail;

namespace SchedulingSystemWeb.Pages.Student.Home
{
    public class DeleteModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;

        public DeleteModel(UnitOfWork unit)
        {
            _unitOfWork = unit;
        }
        public Booking bookings { get; set; }
        public ApplicationUser Teacherinfo { get; set; }
        public ApplicationUser student { get; set; }
        public void Onget(int? id)
        {
            bookings = _unitOfWork.Booking.Get(b => b.Id == id);
            ProviderProfile p = _unitOfWork.ProviderProfile.Get(p => p.Id == bookings.ProviderProfileID);
            Teacherinfo = _unitOfWork.ApplicationUser.Get(u => u.Id == p.User);
            student = _unitOfWork.ApplicationUser.Get(b => b.Id == bookings.User);
        }
        public async Task<IActionResult> OnPost(int? id)
        {
            
            bookings = _unitOfWork.Booking.Get(b => b.Id == id);
            _unitOfWork.Booking.Delete(bookings);
            _unitOfWork.Commit();
            await SendEmail(id);
            return RedirectToPage("./Index");
        }
        public async Task SendEmail(int? id)
        {
            bookings = _unitOfWork.Booking.Get(b => b.Id == id);
            ProviderProfile p = _unitOfWork.ProviderProfile.Get(p => p.Id == bookings.ProviderProfileID);
            Teacherinfo = _unitOfWork.ApplicationUser.Get(u => u.Id == p.User);
            student = _unitOfWork.ApplicationUser.Get(b => b.Id == bookings.User);

            var sendgridclient = new SendGridClient("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");
            var from = new SendGrid.Helpers.Mail.EmailAddress("CodemonkeysScheduling@outlook.com", "Code Monkeys");
            var to = new SendGrid.Helpers.Mail.EmailAddress(Teacherinfo.NormalizedUserName, Teacherinfo.FirstName + Teacherinfo.LastName);
            var subject = "Booking Canceled!";
            var plainText = student.FirstName + " " + student.LastName + " Cancelled their appointment with you on " + bookings.StartTime.ToShortDateString() + " at " + bookings.StartTime.ToShortTimeString();
            var htmlContent = "<p>" + plainText + "</p>";
            var message = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);
            var response = await sendgridclient.SendEmailAsync(message);
        }
    }
}
