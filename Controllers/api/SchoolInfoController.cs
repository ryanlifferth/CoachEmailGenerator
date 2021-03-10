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
        private string _filePath = string.Empty;

        public SchoolInfoController(ILogger<HomeController> logger, SaveTemplateService saveTemplateService)
        {
            _logger = logger;
            _saveTemplateService = saveTemplateService;

            // Get the local file location/path
            //_filePath = Directory.GetCurrentDirectory() + "\\Data\\" + fileName + "-schools.json";
            _filePath = Directory.GetCurrentDirectory() + "\\Data\\";
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("SaveIsEnabled")]
        public IActionResult SaveIsEnabled(string userEmail, Guid schoolId, bool isEnabled)
        {
            // Load JSON from file
            var schools = LoadSchoolListFromJsonSource(userEmail);

            if (schools != null)
            {
                foreach (var item in schools.Where(x => x.Id == schoolId))
                {
                    item.IsEnabled = isEnabled;
                }
            }

            // save the file
            SaveSchoolListBackToJsonSource(userEmail, schools);

            return Ok();
        }

        [HttpPost("SaveTheSchool")]
        public IActionResult SaveTheSchool(string userEmail, School school)
        {
            // Load JSON from file
            var schools = LoadSchoolListFromJsonSource(userEmail);

            if (schools != null && school.Id != Guid.Empty)
            {
                foreach (var item in schools.Where(x => x.Id == school.Id))
                {
                    item.SchoolName = school.SchoolName;
                    item.SchoolNameShort = school.SchoolNameShort;
                    item.HeadCoach.Name = school.HeadCoach.Name;
                    item.HeadCoach.Email = school.HeadCoach.Email;
                    item.HeadCoach.PhoneNumber = school.HeadCoach.PhoneNumber;
                    item.IsEnabled = school.IsEnabled;
                }
            }

            if (school != null && school.Id == Guid.Empty)
            {
                // This is a new School, so add it to the object
                school.Id = Guid.NewGuid();
                schools.Add(school);
            }

            // save the file
            SaveSchoolListBackToJsonSource(userEmail, schools);

            return Ok();
        }

        [HttpPost("DeleteTheSchool")]
        public IActionResult DeleteTheSchool(string userEmail, Guid schoolId)
        {
            // Load JSON from file
            var schools = LoadSchoolListFromJsonSource(userEmail);

            if (schools != null && schoolId != Guid.Empty)
            {
                schools.Remove(schools.FirstOrDefault(x => x.Id == schoolId));
            }

            // save the file
            SaveSchoolListBackToJsonSource(userEmail, schools);

            return Ok();
        }

        private List<School> LoadSchoolListFromJsonSource(string userEmail)
        {
            // Load JSON from file
            var fileName = _saveTemplateService.GetUserNameFromEmail(userEmail) + "-schools.json";
            var fullFilePath = _filePath + fileName;

            var jsonString = System.IO.File.Exists(fullFilePath) ? System.IO.File.ReadAllText(fullFilePath) : string.Empty;
            
            return !string.IsNullOrEmpty(jsonString) ? JsonSerializer.Deserialize<List<School>>(jsonString) : null;
        }

        private void SaveSchoolListBackToJsonSource(string userEmail, List<School> schools)
        {
            var fileName = _saveTemplateService.GetUserNameFromEmail(userEmail) + "-schools.json";
            var fullFilePath = _filePath + fileName;

            System.IO.File.WriteAllText(fullFilePath, JsonSerializer.Serialize(schools));
        }

    }
}
