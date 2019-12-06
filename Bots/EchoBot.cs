// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;

        public EchoBot(UserState userState, ConversationState conversationState)
        {
            _userState = userState;
            _conversationState = conversationState;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var userStateAccessor = _userState.CreateProperty<UserData>(nameof(UserData));
            var userData = await userStateAccessor.GetAsync(turnContext, () => new UserData(), cancellationToken);
            var conversationStateAccessor = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationStateAccessor.GetAsync(turnContext, () => new ConversationData(), cancellationToken);

            userData.TurnCount++;
            conversationData.TurnCount++;

            var replyText = 
                $"Echo: {turnContext.Activity.Text}{Environment.NewLine}" +
                $"Turn Count (user): {userData.TurnCount}{Environment.NewLine}" +
                $"Turn Count (conversation): {conversationData.TurnCount}";

            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
