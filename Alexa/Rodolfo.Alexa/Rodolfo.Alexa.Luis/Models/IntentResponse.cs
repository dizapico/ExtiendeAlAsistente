using System.Collections.Generic;

namespace Rodolfo.Alexa.Luis.Models
{
    public class IntentResponse
    {
        public IntentResponse()
        {
            this.Entities = new List<Entity>();
        }

        public string Intent { get; set; }

        public IList<Entity> Entities { get; set; }
    }
}
