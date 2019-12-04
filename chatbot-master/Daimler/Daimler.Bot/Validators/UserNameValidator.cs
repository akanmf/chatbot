namespace Daimler.Bot.PasswordReset.Bot.Validators
{
    public class UserNameValidator
    {
        public static bool Validate(string userName)
        {
            return (!string.IsNullOrWhiteSpace(userName));
        }
    }
}
