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
using System.IO;
using System.Text.Json;

namespace CoachEmailGenerator.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private GmailApiService _gmailApiService;
        private SaveTemplateService _saveTemplateService;

        public HomeController(ILogger<HomeController> logger, 
            GmailApiService gmailApiService, 
            SaveTemplateService saveTemplateService)
        {
            _logger = logger;
            _gmailApiService = gmailApiService;
            _saveTemplateService = saveTemplateService;

            //var opts = config.GetSection("TinyDrive");
        }

        public IActionResult Index()
        {
            var fileName = _saveTemplateService.GetUserNameFromEmail(User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value);
            var filePath = Directory.GetCurrentDirectory() + "\\Data\\" + fileName;
            var jsonString = System.IO.File.ReadAllText(filePath);
            var template = JsonSerializer.Deserialize<EmailTemplate>(jsonString);

            return View(template);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(int Id, [Bind("Id, EmailAddress, EmailSubjectLine, EmailBody")] EmailTemplate template)
        {
            var s = Id.ToString();
            var emailText = template.EmailBody;

            // Save to file
            template.EmailAddress = string.IsNullOrEmpty(template.EmailAddress) ? User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value : template.EmailAddress;
            _saveTemplateService.SaveTemplate(template);

            //var userEmailAddress = User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value;
            //var accessToken = await HttpContext.GetTokenAsync("access_token");
            //var cred = GoogleCredential.FromAccessToken(accessToken);
            //_gmailApiService.CreateEmail(cred, emailText, userEmailAddress);

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
