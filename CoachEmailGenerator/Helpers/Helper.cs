
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

    }
}
