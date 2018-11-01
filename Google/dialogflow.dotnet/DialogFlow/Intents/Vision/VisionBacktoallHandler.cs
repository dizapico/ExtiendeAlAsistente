using System.Linq;
using System.Net;
using Google.Cloud.Dialogflow.V2;

namespace dialogflow.dotnet.DialogFlow.Intents.Vision
{
    [Intent("vision.backtoall")]
    public class VisionBacktoallHandler : BaseVisionHandler
    {
        public VisionBacktoallHandler(Conversation conversation) : base(conversation)
        {
        }

        public override WebhookResponse Handle(WebhookRequest req)
        {
            // Unfocus the image
            conversation.State.FocusedImage = null;

            // Update state with list of pictures
            var images = conversation.State.ImageList;
            var imagesList = images.Select(x => $"<li><img src=\"{x.Url}\" alt=\"{WebUtility.HtmlEncode(x.Title)}\" style=\"width:200px\" /></li>");

            DialogflowApp.Show($"<ol>{string.Join("", imagesList)}</ol>");

            return new WebhookResponse { FulfillmentText = "OK, looking at all images now." };
        }
    }
}