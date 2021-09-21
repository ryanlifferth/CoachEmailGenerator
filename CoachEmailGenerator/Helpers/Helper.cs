
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CoachEmailGenerator.Helpers
{
    public static class Helper
    {

        public static string GetUserNameFromEmail(string emailAddress)
        {
            var addr = new System.Net.Mail.MailAddress(emailAddress);
            //string name = addr.User;
            //string domain = addr.Host;
            return addr.User;
        }

        public static IActionResult CreateHttpResponse(Exception ex = null) 
        {
            if (ex != null)
            {
                int? statusCode = 400;

                //TODO:  Look at the types of exceptions
                return new BadRequestObjectResult(ex.Message)
                {
                    StatusCode = statusCode
                };
            }

            return new BadRequestObjectResult(ex.Message)
            {
                StatusCode = 400
            };

        }

    }
}
