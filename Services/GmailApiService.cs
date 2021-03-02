using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Services
{
    public class GmailApiService
    {
        private readonly IConfiguration _config;

        public GmailApiService(IConfiguration config)
        {
            _config = config;

            // Get the client id and secrent from web.config
            var clientId = _config["GoogleApiAuth:ClientId"];
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0

            var s = clientId;

        }

    }
}
