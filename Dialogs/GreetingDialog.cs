using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using PlurasightBot.Models;
using PlurasightBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlurasightBot.Dialogs
{
    public class GreetingDialog : ComponentDialog
    {
        public readonly StateService _botStateService;

        public GreetingDialog(string dialogId, StateService stateService) : base(dialogId)
        {
            _botStateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));

            InitializeWaterFallDialog();
        }

        public void InitializeWaterFallDialog()
        {
            //create water fall steps
            var waterFallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };

            //Add Named Dialog
            AddDialog(new WaterfallDialog($"{nameof(GreetingDialog)}.mainFlow", waterFallSteps));
            AddDialog(new TextPrompt($"{nameof(GreetingDialog)}.name"));

            //Set the starting Dialog
            InitialDialogId = $"{nameof(GreetingDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if (string.IsNullOrEmpty(userProfile.Name))
            {
                return await stepContext.PromptAsync($"{nameof(GreetingDialog)}.name", 
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What is your name?")
                    }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(null,cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userprofile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if (string.IsNullOrEmpty(userprofile.Name))
            {
                //Set the name
                userprofile.Name = (string)stepContext.Result;

                //Save any changes that might have occured during the turn
                await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userprofile);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Hi {0}. How can I help you today?", userprofile.Name)), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
