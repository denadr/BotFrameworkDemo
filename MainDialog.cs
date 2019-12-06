using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
                BirthdayStepAsync,
                GenderStepAsync,
                AddressStepAsync,
                EndStepAsync
            }));
            AddDialog(new TextPrompt(nameof(TextPrompt), ValidateNameAsync));
            AddDialog(new DateTimePrompt(nameof(DateTimePrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt("AdaptiveCardPrompt", ValidateAddressAsync));

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

        private async Task<DialogTurnResult> BirthdayStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["name"] = stepContext.Result;

            return await stepContext.PromptAsync(nameof(DateTimePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please enter your date of birth."),
                RetryPrompt = MessageFactory.Text("Please try again.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> GenderStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var date = DateTime.Parse((stepContext.Result as List<DateTimeResolution>)[0].Timex);
            stepContext.Values["date"] = date;

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please choose your gender."),
                RetryPrompt = MessageFactory.Text("Please try again."),
                Choices = new List<Choice>()
                {
                    new Choice("male"),
                    new Choice("female"),
                    new Choice("diverse")
                }
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> AddressStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["gender"] = (stepContext.Result as FoundChoice).Value;

            return await stepContext.PromptAsync("AdaptiveCardPrompt", new PromptOptions()
            {
                Prompt = (Activity)MessageFactory.Attachment(new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine("AdaptiveCards", "AddressCard.json")))
                }, "Please enter your address information."),
                RetryPrompt = MessageFactory.Text("Please try again.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> EndStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var addressInfo = JsonConvert.DeserializeObject<AddressCardResult>(stepContext.Context.Activity.Value.ToString());
            stepContext.Values["address"] = addressInfo;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thank you, {stepContext.Values["name"]}!"));
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private Task<bool> ValidateNameAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value.Length > 2);
        }

        private Task<bool> ValidateAddressAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var addressInfo = JsonConvert.DeserializeObject<AddressCardResult>(promptContext.Context.Activity.Value.ToString());

            var streetValid = addressInfo.Street.Length > 2;
            var houseNumberValid = addressInfo.HouseNumber > 0 && addressInfo.HouseNumber < 10000;
            var zipCodeValid = addressInfo.ZipCode > 0 && addressInfo.ZipCode < 100000;
            var cityValid = addressInfo.City.Length > 2;

            return Task.FromResult(streetValid && houseNumberValid && zipCodeValid && cityValid);
        }
    }
}
