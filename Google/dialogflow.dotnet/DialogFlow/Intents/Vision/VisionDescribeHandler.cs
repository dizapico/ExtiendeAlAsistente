using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace dialogflow.dotnet.DialogFlow.Intents.Vision
{
    [Intent("vision.describe")]
    public class VisionDescribeHandler : BaseVisionHandler
    {
        private static readonly List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
        };

        public VisionDescribeHandler(Conversation conversation) : base(conversation)
        {
        }    

        public override async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            var subscriptionKey = Program.AppSettings.Cognitive.ComputerVision;
            var computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
            computerVision.Endpoint = "https://westeurope.api.cognitive.microsoft.com";

            var result = await computerVision.AnalyzeImageAsync(conversation.State.FocusedImage.Url, features);
            var caption = result.Description.Captions[0].Text;

            // var toSay = result.Tags
            //     .OrderByDescending(x => x.Confidence)
            //     .TakeWhile((x, i) => i <= 2 || x.Confidence > 0.75)
            //     .Select(x => x.Name)
            //     .ToList();

            return new WebhookResponse 
            { 
                FulfillmentText = caption  //CombineList(toSay, caption, "This picture is labelled", "Nothing at all, apparently. Which is odd.")
            };
        }        
    }
}