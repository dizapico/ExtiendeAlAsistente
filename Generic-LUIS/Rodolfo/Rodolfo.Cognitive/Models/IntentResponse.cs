using System;
using System.Collections.Generic;
using System.Text;

namespace Rodolfo.Cognitive.Models
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
