using CoachEmailGenerator.Models;
using CoachEmailGenerator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        private DataService _saveTemplateService;

        public EmailPreviewController(ILogger<HomeController> logger,
                GmailApiService gmailApiService,
                DataService saveTemplateService)
        {
            _logger = logger;
            _gmailApiService = gmailApiService;
            _saveTemplateService = saveTemplateService;
        }

        public IActionResult Index()
        {
            return View();
        }


    }
}
