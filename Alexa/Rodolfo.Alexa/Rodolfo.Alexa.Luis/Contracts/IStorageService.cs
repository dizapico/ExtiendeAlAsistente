using System.Collections.Generic;
using System.Threading.Tasks;
using Rodolfo.Alexa.Luis.Models;

namespace Rodolfo.Alexa.Luis.Contracts
{
    public interface IStorageService
    {
        Task<List<MessageEntity>> GetAllAsync();
        Task<MessageEntity> GetLastMessageAsync();
    }
}