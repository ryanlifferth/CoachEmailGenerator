using CoachEmailGenerator.Helpers;
using CoachEmailGenerator.Interfaces;
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
        private IDataService _saveTemplateService;

        public SchoolInfoController(ILogger<HomeController> logger, IDataService dataService)
        {
            _logger = logger;
            _saveTemplateService = dataService;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("SaveIsEnabled")]
        public IActionResult SaveIsEnabled(string userEmail, Guid schoolId, bool isEnabled)
        {
            // Load JSON from file
            var schools = _saveTemplateService.GetSchoolsByEmailAddress(userEmail);
            
            if (schools != null)
            {
                var school = schools.Where(x => x.Id == schoolId).FirstOrDefault();
                school.IsEnabled = isEnabled;
                _saveTemplateService.SaveSchool(userEmail, school);

                return Ok();
            }
            else
            {
                return NotFound();
            }

            // save the file
            //_saveTemplateService.SaveSchools(userEmail, schools);
        }

        [HttpPost("SaveTheSchool")]
        public IActionResult SaveTheSchool(string userEmail, School school)
        {
            // Load JSON from file
            var schools = _saveTemplateService.GetSchoolsByEmailAddress(userEmail);
            if (schools == null)
            {
                schools = new List<School>();
            }

            if (schools != null && school.Id != Guid.Empty)
            {
                foreach (var item in schools.Where(x => x.Id == school.Id))
                {
                    item.SchoolName = school.SchoolName;
                    item.SchoolNameShort = school.SchoolNameShort;
                    item.CoachName = school.CoachName;
                    item.CoachEmail = school.CoachEmail;
                    item.CoachPhoneNumber = school.CoachPhoneNumber;
                    item.IsEnabled = school.IsEnabled;
                }
            }

            if (school != null && school.Id == Guid.Empty)
            {
                // This is a new School, so add it to the object
                school.Id = Guid.NewGuid();
                school.PartitionKey = Helper.GetUserNameFromEmail(userEmail);
                school.RowKey = school.Id.ToString();
                schools.Add(school);
            }

            // save the file
            _saveTemplateService.SaveSchools(userEmail, schools);

            return Ok();
        }

        [HttpPost("DeleteTheSchool")]
        public IActionResult DeleteTheSchool(string userEmail, Guid schoolId)
        {
            // Load JSON from file
            var schools = _saveTemplateService.GetSchoolsByEmailAddress(userEmail);
            var school = schools.Where(x => x.Id == schoolId).FirstOrDefault();

            //if (schools != null && schoolId != Guid.Empty)
            //{
            //    schools.Remove(schools.FirstOrDefault(x => x.Id == schoolId));
            //}
            //
            //// save the file
            //_saveTemplateService.SaveSchools(userEmail, schools);

            if (school != null)
            {
                _saveTemplateService.DeleteSchool(userEmail, school);
            }

            return Ok();
        }



    }
}
