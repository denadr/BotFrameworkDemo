using Microsoft.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.BotBuilderSamples
{
    public class SaveStateMiddleware : IMiddleware
    {
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;

        public SaveStateMiddleware(UserState userState, ConversationState conversationState)
        {
            _userState = userState;
            _conversationState = conversationState;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            await next(cancellationToken);

            await _userState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
            await _conversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
        }
    }
}
