using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rodolfo.Domain;
using System.Text;
using Rodolfo.Domain.Services;

namespace Rodolfo.LUIS.Cortana
{
    public class RodolfoBot : IBot
    {
        private const string LuisKey = "rodolfo-viajeo-Luis";

        private readonly IConfiguration configuration;
        private readonly ITravel travel;
        private readonly IStorageService storageService;

        public RodolfoBot(IConfiguration configuration, ITravel travel, IStorageService storageService)
        {
            this.configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
            this.travel = travel ?? throw new System.ArgumentNullException(nameof(travel));
            this.storageService = storageService ?? throw new System.ArgumentNullException(nameof(storageService));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message && !turnContext.Responded)
            {
                var speechText = await this.travel.EvaluateQueryText(turnContext.Activity.Text);

                await turnContext.SendActivityAsync(speechText, cancellationToken: cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }
        }
    }
}
