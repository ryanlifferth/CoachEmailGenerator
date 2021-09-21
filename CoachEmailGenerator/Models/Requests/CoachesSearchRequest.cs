using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Models.Requests
{
    public class CoachesSearchRequest
    {

        public string PartitionKey { get; set; }  // {SPORT}_{GOVERNING BODY}_{DIVISION}  e.g., M_SOCCER_NCAA_D1
        public string SchoolName { get; set; }
        public string SchoolNameShort { get; set; }


    }
}
