using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rodolfo.Alexa.Luis.Models
{
    public class LuisResponse
    {
        public string Query { get; set; }

        public LuisIntent TopScoringIntent { get; set; }

        public IList<LuisIntent> Intents { get; set; }

        public IList<LuisEntity> Entities { get; set; }
    }
}
