using CoachEmailGenerator.Interfaces;
using CoachEmailGenerator.Models;
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
    public class SchoolController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private IDataService _saveTemplateService;

        public SchoolController(ILogger<HomeController> logger,
            IDataService dataService)
        {
            _logger = logger;
            _saveTemplateService = dataService;
        }

        public IActionResult Index()
        {
            var userEmail = User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value;
            var schools = _saveTemplateService.GetSchoolsByEmailAddress(userEmail);

            // See if there is sort in querystring
            ViewData["Sort"] = String.IsNullOrEmpty(Request.Query["sort"].ToString()) ? "SchoolName" : Request.Query["sort"].ToString();

            return View(schools);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index([Bind("Id, SchoolName, SchoolNameShort, HeadCoach.Name, HeadCoach.Email, HeadCoach.PhoneNumber, IsEnabled")] EmailTemplate template)
        {

            ///************ TEMP CODE **************/
            //var schools = new List<School>()
            //{
            //    new School { Id = Guid.NewGuid(), SchoolName = "Brigham Young University", SchoolNameShort = "BYU", HeadCoach = new Coach { Name = "Coach Smith", Email = "coach@byu.edu", PhoneNumber = "801-555-1234" }, IsEnabled = true },
            //    new School { Id = Guid.NewGuid(), SchoolName = "Utah Valley University", SchoolNameShort = "UVU", HeadCoach = new Coach { Name = "Greg Maas", Email = "greg.maas@uvu.edu" }, IsEnabled = true },
            //    new School { Id = Guid.NewGuid(), SchoolName = "Seattle University", SchoolNameShort = "SU", HeadCoach = new Coach { Name = "Pete Fewing", Email = "fewingp@seattleu.edu", PhoneNumber = "206-296-5498" }, IsEnabled = true }
            //};
            
            //var fileName = _saveTemplateService.GetUserNameFromEmail(User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value);
            //var filePath = Directory.GetCurrentDirectory() + "\\Data\\" + fileName + "-schools.json";

            //var jsonString = JsonSerializer.Serialize(schools);
            //System.IO.File.WriteAllText(filePath, jsonString);
            ///********** END TEMP CODE ************/

            return View();
        }

        public IActionResult Search()
        {
            var userEmail = User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value;
            var schools = _saveTemplateService.GetSchoolsByEmailAddress(userEmail);

            return View();
        }


    }
}
