using System;
using System.Linq;
using System.Threading.Tasks;
using dialogflow.dotnet.DialogFlow.Intents;
using Google;
using Google.Cloud.Dialogflow.V2;

namespace dialogflow.dotnet.DialogFlow
{
    public class Conversation 
    {
        public ConvState State { get; private set; }

        public Conversation()
        {
            State = new ConvState();
        }

        public async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            var intentName = req.QueryResult.Intent.DisplayName;
            var handler = FindHandler(intentName);

            if (handler == null)
            {
                return new WebhookResponse
                {
                    FulfillmentText = $"Sorry, no handler found for intent: {intentName}"
                };
            }

            try
            {
                return handler.Handle(req) ??

                    await handler.HandleAsync(req) ??

                    new WebhookResponse

                    {

                        FulfillmentText = "Error. Handler did not return a valid response."

                    }; 
            }
            catch (Exception e) when (req.QueryResult.Intent.DisplayName != "exception.throw")
            {
                var msg = (e as GoogleApiException)?.Error.Message ?? e.Message;
                return new WebhookResponse
                {
                    FulfillmentText = $"Sorry, there's a problem: {msg}"
                };
            }
        }

        private BaseHandler FindHandler(string intentName)
        {
            var baseHandlerTypes = typeof(BaseHandler).Assembly.GetTypes()
                .Where(t => t.IsClass && t.IsSubclassOf(typeof(BaseHandler)));

            var typeList = from baseHandlerType in baseHandlerTypes
                          from attribute in baseHandlerType.GetCustomAttributes(typeof(IntentAttribute), true)
                          where ((IntentAttribute)attribute).Name == intentName
                          select baseHandlerType;

            var type = typeList.FirstOrDefault();
            if (type == null) return null;

            var constructorInfo = type.GetConstructor(new[] { GetType() });
            var instance = (BaseHandler)constructorInfo.Invoke(new object[] { this });

            return instance;
        }        
    }
}