using System.Collections.Generic;
using System.Linq;

namespace dialogflow.dotnet.DialogFlow.Intents.Vision
{
    public class BaseVisionHandler : BaseHandler
    {
        public BaseVisionHandler(Conversation conversation) : base(conversation)
        {
        }

        protected static string CombineList(IReadOnlyList<string> col, string nonEmptyPrefix, string onEmpty)
        {
            if (col == null || col.Count == 0) return onEmpty;
            if (col.Count == 1) return $"{nonEmptyPrefix} {col[0]}";

            return $"{nonEmptyPrefix} {string.Join(", ", col.Take(col.Count - 1))}, and {col.Last()}";
        }
    }
}