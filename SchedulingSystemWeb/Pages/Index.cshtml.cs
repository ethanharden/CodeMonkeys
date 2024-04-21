using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Threading.Tasks;
//using Microsoft.Identity.Web;
//using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
//using Microsoft.Graph.Extensions;
using Microsoft.Identity.Client.Extensions.Msal;
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Graph.Models;
using Google.Apis.Auth.OAuth2;

namespace SchedulingSystemWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGet(string code, string state)
        {
            //var Code = Request.Query["code"].ToString();

            

            if (User.IsInRole("STUDENT"))
            {
                return LocalRedirect("/Student/Home/Index");
            }
            else if (User.IsInRole("TUTOR"))
            {
                return LocalRedirect("/Tutor/Home");
            }
            else if (User.IsInRole("ADMIN"))
            {
                return LocalRedirect("/Admin/Users/UserIndex");
            }
            else if (User.IsInRole("TEACHER"))
            {
                return LocalRedirect("/Teacher/Availabilities/Index");
            }

            if (string.IsNullOrEmpty(code))
            {
                // No code parameter, return the page without processing
                return Page();
            }

            return RedirectToPage("/Index");
            }
        


    }
}

