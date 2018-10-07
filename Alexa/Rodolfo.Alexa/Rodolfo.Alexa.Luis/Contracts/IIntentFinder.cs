using Rodolfo.Alexa.Luis.Models;
using System.Threading.Tasks;

namespace Rodolfo.Alexa.Luis.Contracts
{
    public interface IIntentFinder
    {
        Task<IntentResponse> GetIntentAsync(string phrase);
    }
}
