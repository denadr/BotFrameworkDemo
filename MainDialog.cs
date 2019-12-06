using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : ComponentDialog
    {
        public MainDialog()
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NameStepAsync,
                EndStepAsync
            }));
            AddDialog(new TextPrompt(nameof(TextPrompt), ValidateNameAsync));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("What's your name?"),
                RetryPrompt = MessageFactory.Text("Please try again.")
            }, cancellationToken);
        }
        
        private async Task<DialogTurnResult> EndStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thank you, {stepContext.Result}!"));
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private Task<bool> ValidateNameAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value.Length > 2);
        }
    }
}
