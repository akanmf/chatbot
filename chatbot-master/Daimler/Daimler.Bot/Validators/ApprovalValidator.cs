
namespace Daimler.Bot.PasswordReset.Bot.Validators
{
    public class ApprovalValidator
    {
        public static bool Validate(string input)
        {
            return (",OK,APPROVE,YES,"
                + ",NO,NOK,REJECT,CANCEL,"
                + ",EVET,TABİ,ONAYLIYORUM,TABİKİ"
                + ",HAYIR,İPTAL,ONAYLAMIYORUM")
                .ToLower()
                .Contains($",{input.ToLower()},");
        }

        public static bool IsApprooved(string input)
        {
            return (",OK,APPROVE,YES,RIGHT,"
                + ",EVET,TABİ,ONAYLIYORUM,TABİKİ")
                .ToLower()
                .Contains($",{input.ToLower()},");
        }

        public static bool IsDeclined(string input)
        {
            return (",NO,NOK,REJECT,CANCEL,"
                 + ",HAYIR,İPTAL,ONAYLAMIYORUM"
                )
                .ToLower()
                .Contains($",{input.ToLower()},");
        }
    }
}
