using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodolfo.Domain
{
    public interface ITravel
    {
        Task<string> GetVisitingPlaceAsync(string location);
        Task<string> GetAllVisitingPlaceAsync();
        Task<string> GetLastVisitingPlaceAsync();
    }
}
