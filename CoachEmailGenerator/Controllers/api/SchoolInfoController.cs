using CoachEmailGenerator.Helpers;
using CoachEmailGenerator.Interfaces;
using CoachEmailGenerator.Models;
using CoachEmailGenerator.Models.Requests;
using CoachEmailGenerator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        private ISchoolService _schoolService;

        public SchoolInfoController(ILogger<HomeController> logger, IDataService dataService, ISchoolService schoolService)
        {
            _logger = logger;
            _saveTemplateService = dataService;
            _schoolService = schoolService;
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

        [HttpPost("SaveAllIsEnabled")]
        public IActionResult SaveAllIsEnabled(string userEmail, bool isEnabled)
        {
            // Load all schools
            var schools = _saveTemplateService.GetSchoolsByEmailAddress(userEmail);

            if (schools != null )
            {
                foreach (var school in schools)
                {
                    school.IsEnabled = isEnabled;
                }
                _saveTemplateService.SaveSchools(userEmail, schools);
                return Ok();
            }
            else
            {
                return NotFound();
            }
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

        [HttpPost("GetSchools")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<School>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetSchools(string userEmail)
        {
            try
            {
                var schools = _saveTemplateService.GetEmailTemplateByEmailAddress(userEmail);
                return Ok(schools);
            }
            catch (Exception ex)
            {
                return Helper.CreateHttpResponse(ex);
                //return BadRequest(ex);
            }
        }

        [HttpPost("SearchSchools")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<SearchSchoolResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult SearchSchools([FromBody] CoachesSearchRequest request)
        {
            try
            {
                var schools = _schoolService.SearchSchools(request);
                // translate to search school response object
                var translatedSchoolsResponse = TranslateCoachesToSchoolsResponse(schools);
                return Ok(translatedSchoolsResponse);
            }
            catch (Exception ex)
            {
                return Helper.CreateHttpResponse(ex);
                //return BadRequest(ex);
            }
        }

        private IEnumerable<SearchSchoolResponse> TranslateCoachesToSchoolsResponse(IEnumerable<CoachesResponse> coachesResponse)
        {
            //var result = coachesResponse.GroupBy(x => x.SchoolId).ToList();

            IEnumerable<SearchSchoolResponse> schools = coachesResponse
                            .GroupBy(x => new { x.SchoolId, x.SchoolName, x.SchoolNameShort, x.Sport, x.Division, x.Conference, x.IsEnabled })
                            .Select(s => new SearchSchoolResponse
                                {
                                    SchoolId = s.Key.SchoolId,
                                    SchoolName = s.Key.SchoolName,
                                    SchoolNameShort = s.Key.SchoolNameShort,
                                    Coaches = s.Select(c => new Coach
                                    {
                                        Email = c.RowKey.ToString(),
                                        Name = c.CoachName,
                                        Title = c.CoachTitle,
                                        Sport = c.Sport
                                    }).ToList(),
                                    Division = s.Key.Division,
                                    Conference = s.Key.Conference,
                                    IsEnabled = s.Key.IsEnabled
                                })
                            .OrderBy(s => s.SchoolName);


            return schools;
        }

    }
}
