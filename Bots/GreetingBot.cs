using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using PlurasightBot.Models;
using PlurasightBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlurasightBot.Bots
{
    public class GreetingBot : ActivityHandler
    {
        public readonly StateService _stateService;
        public GreetingBot(StateService stateService)
        {
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
        }

        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(turnContext, () => new UserProfile()); //<== Getting the user name.

            ConversationData conversationData = await _stateService.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData()); //<== Getting the required data as requested so as to be stated.

            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(string.Format("Hi {0}, How can I help you today?", userProfile.Name)), cancellationToken);
            }
            else
            {
                if (conversationData.PromtedUserForName)
                {
                    //set the name to what the user provided
                    userProfile.Name = turnContext.Activity.Text?.Trim(); //<== Trying to recover the particular details that was provided.

                    //Acknowledge that we got their name
                    await turnContext.SendActivityAsync(MessageFactory.Text(string.Format("Thanks {0}. How can I help you today?", userProfile.Name)), cancellationToken);

                    //Restart the flag to allow the bot to go through thr cycle again.
                    conversationData.PromtedUserForName = false;
                }
                else
                {
                    //promt the user for their name
                    await turnContext.SendActivityAsync(MessageFactory.Text($"What is your name?"), cancellationToken);

                    //set the flag to turn, so we dont  promt it in the next turn.
                    conversationData.PromtedUserForName = true;
                }
                //Save any state changes that might occured during the turn.
                await _stateService.UserProfileAccessor.SetAsync(turnContext, userProfile);
                await _stateService.ConversationDataAccessor.SetAsync(turnContext, conversationData);

                await _stateService.UserState.SaveChangesAsync(turnContext);
                await _stateService.ConversationState.SaveChangesAsync(turnContext);
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await GetName(turnContext, cancellationToken);
                }
            }
        }
    }
}
