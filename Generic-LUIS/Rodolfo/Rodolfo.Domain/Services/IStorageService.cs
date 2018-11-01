using Rodolfo.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodolfo.Domain.Services
{
    public interface IStorageService
    {
        Task<List<MessageEntity>> GetAllAsync();
        Task<MessageEntity> GetLastMessageAsync();
    }
}
