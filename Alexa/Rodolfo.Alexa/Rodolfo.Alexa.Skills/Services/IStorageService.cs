using System.Collections.Generic;
using System.Threading.Tasks;
using Rodolfo.Alexa.Skills.Models;

namespace Rodolfo.Alexa.Skills.Services
{
    public interface IStorageService
    {
        Task<List<MessageEntity>> GetAllAsync();
        Task<MessageEntity> GetLastMessageAsync();
    }
}