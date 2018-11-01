using System.Net;
using Google.Cloud.Dialogflow.V2;

namespace dialogflow.dotnet.DialogFlow.Intents.Vision
{
    [Intent("vision.select")]
    public class VisionSelectHandler : BaseVisionHandler
    {
        public VisionSelectHandler(Conversation conversation) : base(conversation)
        {
        }

        public override WebhookResponse Handle(WebhookRequest req)
        {
            int index = (int)req.QueryResult.Parameters.Fields["index"].NumberValue;

            if (index < 1 || index > conversation.State.ImageList.Count)
            {
                // Tried to select an out-of-range picture.
                return new WebhookResponse { FulfillmentText = $"That picture doesn't exist!" };
            }

            // Store the selected image in the state
            var image = conversation.State.ImageList[index - 1];
            conversation.State.FocusedImage = image;

            DialogflowApp.Show($"<img src=\"{image.Url}\" alt=\"{WebUtility.HtmlEncode(image.Title)}\" style=\"height:100%;width:100%\" />");

            return new WebhookResponse 
            { 

                FulfillmentText = $"Picture {index} selected. You can describe, show landmarks or ask whether the image is safe."
            };
        }
    }
}