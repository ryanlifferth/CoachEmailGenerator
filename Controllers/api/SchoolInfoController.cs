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

namespace CoachEmailGenerator.Controllers.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchoolInfoController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private SaveTemplateService _saveTemplateService;

        public SchoolInfoController(ILogger<HomeController> logger, SaveTemplateService saveTemplateService)
        {
            _logger = logger;
            _saveTemplateService = saveTemplateService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("SaveTheSchool")]
        public IActionResult SaveTheSchool(School school)
        {
            // Load JSON from file
            var fileName = _saveTemplateService.GetUserNameFromEmail(User.Claims.FirstOrDefault(x => x.Type.ToString().IndexOf("emailaddress") > 0)?.Value);
            var filePath = Directory.GetCurrentDirectory() + "\\Data\\" + fileName + "-schools.json";

            var jsonString = System.IO.File.Exists(filePath) ? System.IO.File.ReadAllText(filePath) : string.Empty;
            var allSchools = !string.IsNullOrEmpty(jsonString) ? JsonSerializer.Deserialize<List<School>>(jsonString) : null;

            if (allSchools != null)
            {
                foreach (var item in allSchools.Where(x => x.Id == school.Id))
                {
                    item.SchoolName = school.SchoolName;
                    item.SchoolNameShort = school.SchoolNameShort;
                    item.HeadCoach.Name = school.HeadCoach.Name;
                    item.HeadCoach.Email = school.HeadCoach.Email;
                    item.HeadCoach.PhoneNumber = school.HeadCoach.PhoneNumber;
                    item.IsEnabled = school.IsEnabled;
                }
            }

            // save the file
            jsonString = JsonSerializer.Serialize(allSchools);
            System.IO.File.WriteAllText(filePath, jsonString);

            return Ok();
        }

    }
}
