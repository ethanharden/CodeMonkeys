﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DataAccess;
using Infrastructure.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Graph.Models;

namespace SchedulingSystemWeb.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private IWebHostEnvironment _webhostenvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UnitOfWork _unitOfWork;
        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            UnitOfWork unitOfWork,
            IWebHostEnvironment webhostenvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _webhostenvironment = webhostenvironment;
        }
        
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
       
        
        public bool Edit {  get; set; }
        [BindProperty]

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [BindProperty]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [BindProperty]
        [Display(Name = "Phone Number")]
        public string PhoneNum { get; set; }
        
        [BindProperty]
        [Display(Name = "Remote Link")]
        public string RemoteLink { get; set; }
        [BindProperty]
        [Display(Name = "Booking Prompt")]
        public string BookingPrompt { get; set; }
        [BindProperty]
        
        public int? WNumber { get; set; }
        [BindProperty]
        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }
        [BindProperty]
        public bool IsProvider { get; set; }
        [BindProperty]
        public string Department { get; set; }
        [BindProperty]
        [Display(Name = "Working Start Time")]
        public TimeOnly? StartTime { get; set; }
        [BindProperty]
        [Display(Name = "Working End Time")]
        public TimeOnly? EndTime { get; set; }
        public IEnumerable<SelectListItem> DepartmentList { get; set; }
        private async Task LoadAsync(ApplicationUser user)
        {
            Edit = false;
            var userName = await _userManager.GetUserNameAsync(user);
            PhoneNum = user.PhoneNum;
            FirstName = user.FirstName;
            LastName = user.LastName;
            ProfilePicture = user.ProfilePicture;
            Username = userName;
            
            DepartmentList = _unitOfWork.Department.GetAll().Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString(),
            });
            var role = await _userManager.GetRolesAsync(user);
            if (role[0] == "STUDENT")
            {
                var studentprofile = _unitOfWork.CustomerProfile.Get(u => u.User == user.Id);
                if (studentprofile.WNumber != null) 
                {
                    WNumber = _unitOfWork.CustomerProfile.Get(u => u.User == user.Id).WNumber;
                }
                
            }
            if (role[0] == "TEACHER" || role[0] == "ADVISOR" || role[0] == "TUTOR")
            {
                IsProvider = true;
                var prof = _unitOfWork.ProviderProfile.Get(u => u.User == user.Id);
                if (prof.DeparmentId == 0)
                {
                    Department = "No Department";
                }
                else
                {
                    Department = _unitOfWork.Department.Get(u => u.Id == prof.DeparmentId).Name;
                }
                if(prof.workingStartHours!=null)
                {
                    StartTime = (TimeOnly)prof.workingStartHours;
                }
                if(prof.workingEndHours!=null) { EndTime = (TimeOnly)prof.workingEndHours;}
                
                BookingPrompt = prof.BookingPrompt;
                RemoteLink = prof.RemoteLink;
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }
        public async Task OnPostEdit()
        {
            //Edit = true;
            await OnGetAsync();
            Edit = true;
        }
        public async Task<IActionResult> OnPostSubmit()
        {
           
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }//determine Root Path of wwwroot
            string webRootPath = _webhostenvironment.WebRootPath;
            //retrieve the files
            //var files = ProfilePicture;
            var files = HttpContext.Request.Form.Files;
            //if new product

            if (files.Count > 0)
                {
                    //create unique identifier
                    string fileName = Guid.NewGuid().ToString().ToString();
                    //create path to /images/products folder
                    var uploads = Path.Combine(webRootPath, @"images\profile\");
                    //get and preserve extension type
                    var extension = Path.GetExtension(files[0].FileName);
                    //Create full path
                    var fullPath = uploads + fileName + extension;
                    //stream the binary files to the server
                    using var fileStream = System.IO.File.Create(fullPath);
                    files[0].CopyTo(fileStream);
                    //associate real URL and save to DB
                    user.ProfilePicture = @"\images\profile\" + fileName + extension;
                }
                user.FirstName = FirstName;
            user.LastName = LastName;
            user.PhoneNum = PhoneNum;
           
            await _userManager.UpdateAsync(user);
            var role = await _userManager.GetRolesAsync(user);
            if (role[0] == "STUDENT")
            {
                CustomerProfile customerProfile = _unitOfWork.CustomerProfile.Get(u => u.User == user.Id);
                customerProfile.WNumber = WNumber;
                _unitOfWork.CustomerProfile.Update(customerProfile);
            }
            if (role[0] == "TEACHER" || role[0] == "TUTOR" || role[0] == "ADVISOR")
            {
                
                ProviderProfile profiderProfile = _unitOfWork.ProviderProfile.Get(u => u.User == user.Id);
                profiderProfile.BookingPrompt = BookingPrompt;
                profiderProfile.workingStartHours = StartTime;
                profiderProfile.workingEndHours = EndTime;
                if (Department != null) { profiderProfile.DeparmentId = Int32.Parse(Department); }
               
                
                profiderProfile.RemoteLink = RemoteLink;
                _unitOfWork.ProviderProfile.Update(profiderProfile);
            }
              
            _unitOfWork.Commit();
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            Edit = false;
            return Redirect("/Identity/Account/Manage");
        }
    }
}
