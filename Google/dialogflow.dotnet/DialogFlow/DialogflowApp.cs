using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using dialogflow.dotnet.Controllers;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dialogflow.dotnet.DialogFlow 
{
    public class DialogflowApp 
    {
        private static readonly Dictionary<string, Conversation> conversations = new Dictionary<string, Conversation>();
        private static readonly JsonParser jsonParser = new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));
        private readonly ILogger<ConversationController> logger;

        public DialogflowApp(ILogger<ConversationController> logger)
        {
            this.logger = logger;
        }

        public static void Show(string html) => HomeController.SetPage(html);

        public async Task<WebhookResponse> HandleRequest(HttpRequest httpRequest)
        {
            WebhookRequest request;

            using (var reader = new StreamReader(httpRequest.Body))
            {
                request = jsonParser.Parse<WebhookRequest>(reader);
            }

            logger.LogInformation($"Intent: '{request.QueryResult.Intent.DisplayName}',  QueryText: '{request.QueryResult.QueryText}'");

            var conversation = GetOrCreateConversation(request.Session);

            return await conversation.HandleAsync(request);
        }    

        
        private Conversation GetOrCreateConversation(string sessionId)
        {
            Conversation conversation;

            lock (conversations)
            {
                if (!conversations.TryGetValue(sessionId, out conversation))
                {
                    logger.LogInformation($"Creating new conversation with sessionId: {sessionId}");
                    conversation = new Conversation();
                    conversations.Add(sessionId, conversation);
                }
            }

            return conversation;
        }
    }
}