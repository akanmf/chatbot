namespace Daimler.Bot.PasswordReset.Bot.Validators
{
    public class EmailValidator
    {
        public static bool Validate(string email)
        {
            return email.Contains(".com");
        }
    }
}
