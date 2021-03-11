﻿using CoachEmailGenerator.Models;
using CoachEmailGenerator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Controllers
{

    [Authorize]
    [Route("email-template")]
    public class EmailTemplateController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private GmailApiService _gmailApiService;
        private DataService _saveTemplateService;
        private string _filePath = string.Empty;

        public EmailTemplateController(ILogger<HomeController> logger,
            GmailApiService gmailApiService,
            DataService saveTemplateService)
        {
            _logger = logger;
            _gmailApiService = gmailApiService;
            _saveTemplateService = saveTemplateService;

        }

        public IActionResult Index()
        {
            var userEmail = User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value;
            var template = _saveTemplateService.LoadTemplateFromJsonSource(userEmail);

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


    }
}