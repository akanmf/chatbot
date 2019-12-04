using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daimler.Bot.PasswordReset.Bot.Enums;
using Daimler.Bot.PasswordReset.Bot.States;
using Daimler.Bot.PasswordReset.Bot.Validators;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Daimler.Bot.PasswordReset.Bot.Bots
{
    public class PasswordResetBot : ActivityHandler
    {

        private ConversationState _conversationState;
        private UserState _userState;
        public PasswordResetBot(ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;
        }

        protected async override Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await base.OnMembersAddedAsync(membersAdded, turnContext, cancellationToken);
            await turnContext.SendActivityAsync("Merhaba. Hangi konuda size yardımcı olmamı istersiniz? Şimdilik sadece aşağıdaki işlemler konusunda size yardım edebilirim.");

            List<string> supportedActions = new List<string>();
            supportedActions.Add("Şifre Resetleme");

            await turnContext.SendActivityAsync(MessageFactory.SuggestedActions(supportedActions));
        }

        protected async override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await base.OnMessageActivityAsync(turnContext, cancellationToken);

            var conversationStateAccessors = _conversationState.CreateProperty<PasswordResetConversationState>(nameof(PasswordResetConversationState));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new PasswordResetConversationState());

            var userStateAccessors = _userState.CreateProperty<PasswordResetRequest>(nameof(PasswordResetRequest));
            var userProfile = await userStateAccessors.GetAsync(turnContext, () => new PasswordResetRequest());

            var inputText = turnContext.Activity.Text.Trim().ToLower();

            switch (conversationData.CurrentState)
            {
                case Enums.PwdResetConversationStates.Initial:
                    conversationData.CurrentState = PwdResetConversationStates.AskUserName;
                    await turnContext.SendActivityAsync("Şifrenizi resetlemek için kullanıcı adınızı öğrenebilir miyim?");
                    break;
                case Enums.PwdResetConversationStates.AskUserName:
                    if (UserNameValidator.Validate(inputText))
                    {
                        userProfile.UserName = inputText;
                        conversationData.CurrentState = PwdResetConversationStates.AskEmail;
                        await turnContext.SendActivityAsync($"Teşekkürler {userProfile.UserName}, Şimdi mail adresini öğrenebilir miyim?");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"İsmini tekrar girebilir misin?");
                    }
                    break;
                case Enums.PwdResetConversationStates.AskEmail:
                    if (EmailValidator.Validate(inputText))
                    {
                        userProfile.Email = inputText;
                        conversationData.CurrentState = PwdResetConversationStates.AskApproval;
                        await turnContext.SendActivityAsync($"Bilgilerini aldım. Teşekkür ederim. Şimdi onayına ihtiyacım var.");
                        await turnContext.SendActivityAsync($"Kullanıcı adı :{userProfile.UserName}, email:{userProfile.Email}. Bu bilgiler ile şifreni resetlemek istediğinden emin misiniz?");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"{userProfile.UserName} emailini tekrar girebilir misin?");
                    }
                    break;
                case Enums.PwdResetConversationStates.AskApproval:
                    if (ApprovalValidator.Validate(inputText))
                    {
                        
                        if (ApprovalValidator.IsApprooved(inputText))
                        {
                            conversationData.CurrentState = PwdResetConversationStates.Completed;
                            //EXCEL BAŞLANGIÇ
                            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                            Excel.Workbook xlWorkBook;
                            Excel.Worksheet xlWorkSheet;
                            object misValue = System.Reflection.Missing.Value;

                            xlWorkBook = xlApp.Workbooks.Add(misValue);
                            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                            xlWorkSheet.Cells[1, 1] = "Username";
                            xlWorkSheet.Cells[1, 2] = "Email";
                            xlWorkSheet.Cells[2, 1] = userProfile.UserName;
                            xlWorkSheet.Cells[2, 2] = userProfile.Email;

                            xlApp.DisplayAlerts = false;
                            xlWorkBook.SaveAs("C:\\Users\\BaranOzsarac\\Documents\\" + userProfile.UserName + ".xlsx", Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                            xlWorkBook.Close(true, misValue, misValue);
                            xlApp.Quit();

                            Marshal.ReleaseComObject(xlWorkSheet);
                            Marshal.ReleaseComObject(xlWorkBook);
                            Marshal.ReleaseComObject(xlApp);
                            //EXCEL BİTİŞ
                            await turnContext.SendActivityAsync($"Onayınızı aldım. En kısa zamanda şifreniz resetlenecektir.");
                        }
                        else
                        {
                            conversationData.CurrentState = PwdResetConversationStates.Cancelled;
                            await turnContext.SendActivityAsync($"İşlem iptal edilmiştir.");
                        }
                        
                        await turnContext.SendActivityAsync($"İyi çalışmalar.");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"sizi anlayamadım.");
                        await turnContext.SendActivityAsync($"Kullanıcı adı :{userProfile.UserName}, email:{userProfile.Email}. Bu bilgiler ile şifreni resetlemek istediğinden emin misiniz?");
                    }
                    break;
                case Enums.PwdResetConversationStates.Cancelled:
                case Enums.PwdResetConversationStates.Completed:
                    await turnContext.SendActivityAsync("Yeni bir işlem yapmak için lütfen hangi işlemi yapmak istediğinizi söyleyiniz.");
                    conversationData.CurrentState = PwdResetConversationStates.Initial;
                    List<string> supportedActions = new List<string>();
                    supportedActions.Add("Şifre Resetleme");
                    await turnContext.SendActivityAsync(MessageFactory.SuggestedActions(supportedActions));
                    break;
                                
                default:
                    break;
            }


        }

        public async override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            await _conversationState.SaveChangesAsync(turnContext, true, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, true, cancellationToken);
        }
    }
}
