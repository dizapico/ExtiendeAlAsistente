using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodolfo.Domain
{
    public interface ITravel
    {
        Task<string> EvaluateQueryText(string queryText);
    }
}
