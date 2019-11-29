using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daimler.Bot.PasswordReset.Bot.Enums
{
    public enum PwdResetConversationStates
    {
        Initial,
        AskUserName,
        Completed,
        Cancelled,
        AskEmail,
        AskApproval
    }
}
