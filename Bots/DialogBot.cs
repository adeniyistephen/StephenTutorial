using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using PlurasightBot.Services;
using PlurasightBot.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlurasightBot.Bots
{
    public class DialogBot<T> : ActivityHandler where T : Dialog     
    {
        protected readonly StateService _stateService;
        protected readonly ILogger _ilogger;
        protected readonly Dialog _dialog;

        public DialogBot(StateService botStateService, T dialog, ILogger<DialogBot<T>> logger)
        {
            _stateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            _ilogger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _dialog = dialog ?? throw new System.ArgumentNullException(nameof(dialog));
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            //Save my state changes that might have occur during the turn
            await _stateService.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _stateService.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _ilogger.LogInformation("Running dialog with Message Activity");

            //Run the Dialog with the new message activity
            await _dialog.Run(turnContext, _stateService.DialogStateAccessor, cancellationToken);
        }
    }
}
