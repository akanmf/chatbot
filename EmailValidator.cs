using System.Text.RegularExpressions;

namespace Daimler.Bot.PasswordReset.Bot.Validators
{
    public class EmailValidator
    {
        public static bool Validate(string email)
        {   //EMAİL KONTROLÜ
            Regex rg = new Regex(@".{2}\@\w+\.com");
            return rg.IsMatch(email);
        }
    }
}
