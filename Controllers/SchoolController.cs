using CoachEmailGenerator.Models;
using CoachEmailGenerator.Services;
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
    public class SchoolController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private SaveTemplateService _saveTemplateService;

        public SchoolController(ILogger<HomeController> logger,
            SaveTemplateService saveTemplateService)
        {
            _logger = logger;
            _saveTemplateService = saveTemplateService;
        }

        public IActionResult Index()
        {
            var fileName = _saveTemplateService.GetUserNameFromEmail(User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value);
            var filePath = Directory.GetCurrentDirectory() + "\\Data\\" + fileName + "-schools.json";

            var jsonString = System.IO.File.Exists(filePath) ? System.IO.File.ReadAllText(filePath) : string.Empty;
            var school = !string.IsNullOrEmpty(jsonString) ? JsonSerializer.Deserialize<List<School>>(jsonString) : null;

            return View(school);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index([Bind("Id, SchoolName, SchoolNameShort, HeadCoach.Name, HeadCoach.Email, HeadCoach.PhoneNumber, IsEnabled")] EmailTemplate template)
        {

            /************ TEMP CODE **************/
            var schools = new List<School>()
            {
                new School { Id = Guid.NewGuid(), SchoolName = "Brigham Young University", SchoolNameShort = "BYU", HeadCoach = new Coach { Name = "Coach Smith", Email = "coach@byu.edu", PhoneNumber = "801-555-1234" }, IsEnabled = true },
                new School { Id = Guid.NewGuid(), SchoolName = "Utah Valley University", SchoolNameShort = "UVU", HeadCoach = new Coach { Name = "Greg Maas", Email = "greg.maas@uvu.edu" }, IsEnabled = true },
                new School { Id = Guid.NewGuid(), SchoolName = "Seattle University", SchoolNameShort = "SU", HeadCoach = new Coach { Name = "Pete Fewing", Email = "fewingp@seattleu.edu", PhoneNumber = "206-296-5498" }, IsEnabled = true }
            };
            
            var fileName = _saveTemplateService.GetUserNameFromEmail(User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value);
            var filePath = Directory.GetCurrentDirectory() + "\\Data\\" + fileName + "-schools.json";

            var jsonString = JsonSerializer.Serialize(schools);
            System.IO.File.WriteAllText(filePath, jsonString);
            /********** END TEMP CODE ************/


            return View();
        }

    }
}
