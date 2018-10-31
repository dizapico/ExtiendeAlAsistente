using System;
using System.Collections.Generic;
using System.Text;

namespace Rodolfo.Cognitive.Models
{
    public class LuisResponse
    {
        public string Query { get; set; }

        public LuisIntent TopScoringIntent { get; set; }

        public IList<LuisIntent> Intents { get; set; }

        public IList<LuisEntity> Entities { get; set; }
    }
}
