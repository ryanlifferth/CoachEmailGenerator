using CoachEmailGenerator.Models;
using CoachEmailGenerator.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Interfaces
{
    public interface ISchoolService
    {
        public List<CoachesResponse> SearchSchools(CoachesSearchRequest request);
        public List<CoachesResponse> FindSchool(CoachesSearchRequest request);

    }
}
