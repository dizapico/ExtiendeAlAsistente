using Rodolfo.Cognitive.Models;
using System.Threading.Tasks;

namespace Rodolfo.Cognitive
{
    public interface IIntentFinder
    {
        Task<IntentResponse> GetIntentAsync(string phrase);
    }
}
