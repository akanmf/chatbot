namespace Daimler.Bot.PasswordReset.Bot.Validators
{
    public class EmailValidator
    {
        public static bool Validate(string email)
        {   //EMAİL KONTROLÜ
            if (email.Contains(".com") & email.Contains("@"))
            {
                return (email.Substring(email.IndexOf("@"), email.IndexOf(".com") - email.IndexOf("@")).Length > 1) & email.IndexOf("@") > 2;
            }
            else
                return email.Contains(".com");
        }
    }
}
