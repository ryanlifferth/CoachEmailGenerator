using CoachEmailGenerator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoachEmailGenerator.Services;
using Google.Apis.Auth.OAuth2;

namespace CoachEmailGenerator.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private GmailApiService _gmailApiService;
        
        public HomeController(ILogger<HomeController> logger, GmailApiService gmailApiService)
        {
            _logger = logger;
            _gmailApiService = gmailApiService;

            //var opts = config.GetSection("TinyDrive");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int Id, [Bind("Id, Email")] EmailTemplate template)
        {
            var s = Id.ToString();
            var emailText = template.Email;

            var userEmailAddress = User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var cred = GoogleCredential.FromAccessToken(accessToken);
            _gmailApiService.CreateEmail(cred, emailText, userEmailAddress);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
