using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace dialogflow.dotnet.DialogFlow.Intents.Vision
{
    [Intent("vision.landmarks")]
    public class VisionLandmarksHandler : BaseVisionHandler
    {
        private static readonly List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
        };

        public VisionLandmarksHandler(Conversation conversation) : base(conversation)
        {
        }

        public override async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            var subscriptionKey = Program.AppSettings.Cognitive.ComputerVision;
            var computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
            computerVision.Endpoint = "https://westeurope.api.cognitive.microsoft.com";

            var result = await computerVision.AnalyzeImageAsync(conversation.State.FocusedImage.Url, features);

            var landmarks = result.Categories[0].Detail.Landmarks;

            // Order landmarks by score, and prepare the landmark descriptions for DialogFlow
            var toSay = landmarks.OrderByDescending(x => x.Confidence)
                .TakeWhile((x, i) => i == 0 || x.Confidence > 0.75)
                .Select(x => x.Name)
                .ToList();

            return new WebhookResponse 
            {
                FulfillmentText = CombineList(toSay, "This picture contains: ", "This picture contains no landmarks")
            };

        }
    }
}