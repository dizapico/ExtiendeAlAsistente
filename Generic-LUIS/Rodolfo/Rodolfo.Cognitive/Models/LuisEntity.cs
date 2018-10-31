using System;
using System.Collections.Generic;
using System.Text;

namespace Rodolfo.Cognitive.Models
{
    public class LuisEntity
    {
        public string Entity { get; set; }
        public string Type { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public double Score { get; set; }
    }
}
