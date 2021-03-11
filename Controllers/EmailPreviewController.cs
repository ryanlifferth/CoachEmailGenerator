using CoachEmailGenerator.Models;
using CoachEmailGenerator.Services;
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
            // Test
            var userEmail = User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value;
            var template = _saveTemplateService.LoadTemplateFromJsonSource(userEmail);
            var schools = _saveTemplateService.LoadSchoolListFromJsonSource(userEmail);

            dynamic previewModel = new ExpandoObject();
            previewModel.Template = template;
            previewModel.Schools = schools;

            return View(previewModel);
        }


    }
}
