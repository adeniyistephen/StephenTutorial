
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using PlurasightBot.Helpers;
using PlurasightBot.Services;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PlurasightBot.Dialogs
{
    public class BugTypeDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;
        private readonly BotServices _botServices;
        #endregion  


        public BugTypeDialog(string dialogId, StateService botStateService, BotServices botServices) : base(dialogId)
        {
            _stateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            _botServices = botServices ?? throw new System.ArgumentNullException(nameof(botServices));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            // Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };

            // Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(BugTypeDialog)}.mainFlow", waterfallSteps));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(BugTypeDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = await _botServices.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);
            var token = result.Entities.FindTokens("BugType").First();
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            var value = rgx.Replace(token.ToString(), "").Trim();


            if (Common.BugTypes.Any(s => s.Equals(value, StringComparison.OrdinalIgnoreCase)))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Yes! {0} is a Bug Type!", value)), cancellationToken);
                //// Create Facebook Response
                //var replyMessage = stepContext.Context.Activity;

                //var facebookMessage = new FacebookSendMessage();
                //facebookMessage.notification_type = "REGULAR";
                //facebookMessage.attachment = new FacebookAttachment();
                //facebookMessage.attachment.Type = FacebookAttachmentTypes.template;
                //facebookMessage.attachment.Payload = new FacebookPayload();
                //facebookMessage.attachment.Payload.TemplateType = FacebookTemplateTypes.generic;
                //var bugType = new FacebookElement();
                //bugType.Title = value;
                //switch (value.ToLower())
                //{
                //    case "security":
                //        bugType.ImageUrl = "https://c1.staticflickr.com/9/8604/16042227002_1d00e0771d_b.jpg";
                //        bugType.Subtitle = "This is a description of the security bug type";
                //        break;
                //    case "crash":
                //        bugType.ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/5/50/Windows_7_BSOD.png";
                //        bugType.Subtitle = "This is a description of the crash bug type";
                //        break;
                //    case "power":
                //        bugType.ImageUrl = "https://www.publicdomainpictures.net/en/view-image.php?image=1828&picture=power-button";
                //        bugType.Subtitle = "This is a description of the power bug type";
                //        break;
                //    case "performance":
                //        bugType.ImageUrl = "https://commons.wikimedia.org/wiki/File:High_Performance_Computing_Center_Stuttgart_HLRS_2015_07_Cray_XC40_Hazel_Hen_IO.jpg";
                //        bugType.Subtitle = "This is a description of the performance bug type";
                //        break;
                //    case "usability":
                //        bugType.ImageUrl = "https://commons.wikimedia.org/wiki/File:03-Pau-DevCamp-usability-testing.jpg";
                //        bugType.Subtitle = "This is a description of the usability bug type";
                //        break;
                //    case "seriousbug":
                //        bugType.ImageUrl = "https://commons.wikimedia.org/wiki/File:Computer_bug.svg";
                //        bugType.Subtitle = "This is a description of the serious bug type";
                //        break;
                //    case "other":
                //        bugType.ImageUrl = "https://commons.wikimedia.org/wiki/File:Symbol_Resin_Code_7_OTHER.svg";
                //        bugType.Subtitle = "This is a description of the other bug type";
                //        break;
                //    default:
                //        break;
                //}
                //facebookMessage.attachment.Payload.Elements = new FacebookElement[] { bugType };
                //replyMessage.ChannelData = facebookMessage;
                //await stepContext.Context.SendActivityAsync(replyMessage);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("No {0} is not a Bug Type.", value)), cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}


