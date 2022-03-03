using CoachEmailGenerator.Interfaces;
using CoachEmailGenerator.Models;
using CoachEmailGenerator.Services;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Controllers
{

    [Authorize]
    [Route("email-preview")]
    public class EmailPreviewController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private GmailApiService _gmailApiService;
        private IDataService _saveTemplateService;

        public EmailPreviewController(ILogger<HomeController> logger,
                GmailApiService gmailApiService,
                IDataService dataService)
        {
            _logger = logger;
            _gmailApiService = gmailApiService;
            _saveTemplateService = dataService;
        }

        public IActionResult Index()
        {
            // Test
            var userEmail = User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value;
            var template = _saveTemplateService.GetEmailTemplateByEmailAddress(userEmail);
            var schools = _saveTemplateService.GetSchoolsByEmailAddress(userEmail);

            dynamic previewModel = new ExpandoObject();
            previewModel.Template = template;
            previewModel.Schools = schools;

            // Check to see if the user has the property role (scope) to create an email
            // In this list, we are looking for the https://mail.google.com value
            var scope = HttpContext.GetTokenAsync("scope").Result;
            TempData["HasGmailRole"] = scope.IndexOf("https://mail.google.com") > -1 ? true : false;

            return View(previewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail()
        {
            var userEmail = User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value;
            var template = _saveTemplateService.GetEmailTemplateByEmailAddress(userEmail);
            var schools = _saveTemplateService.GetSchoolsByEmailAddress(userEmail);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var cred = GoogleCredential.FromAccessToken(accessToken);
            //_gmailApiService.CreateEmail(cred, emailText, userEmail);
            _gmailApiService.CreateEmail(cred, userEmail, template, schools);

            //return View();
            TempData["EmailSent"] = true;
            return RedirectToAction("Index");
        }


    }
}
