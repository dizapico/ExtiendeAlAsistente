using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;

namespace dialogflow.dotnet.DialogFlow.Intents.Vision
{
    [Intent("vision.search")]
    public class VisionSearchHandler : BaseVisionHandler
    {
        public VisionSearchHandler(Conversation conversation) : base(conversation)
        {
        }

        public override async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            var searchTerm = req.QueryResult.Parameters.Fields["searchterm"].StringValue;

            DialogflowApp.Show($"<div>Searching for pictures of: {searchTerm}</div><div>Please wait...</div>");

            var searchService = CreateSearchClient();
            var result = await searchService.Images.SearchAsync(query: searchTerm);

            // Store images in state
            var images = result.Value
                .Select(x => new ConvState.Image { Title = x.Name, Url = x.ThumbnailUrl })
                .ToList();

            conversation.State.ImageList = images;

            var imageList = images.Select(x => $"<li><img src=\"{x.Url}\" alt=\"{WebUtility.HtmlEncode(x.Title)}\" style=\"width:200px\" /></li>");
            DialogflowApp.Show($"<ol>{string.Join("", imageList)}</ol>");


            // var response = new WebhookResponse();
            var message = new Intent.Types.Message();

            message.CarouselSelect = new Intent.Types.Message.Types.CarouselSelect();
            foreach(var image in images)
            {
                var item = new Intent.Types.Message.Types.CarouselSelect.Types.Item();
                item.Title = image.Title;
                var current = new Intent.Types.Message.Types.Image();
                current.ImageUri = image.Url;
                item.Image = current;

                message.CarouselSelect.Items.Add(item);
            }
            // response.FulfillmentMessages.Add(message);
            // response.FulfillmentText = $"Found some pictures of: {searchTerm}. Now, select a picture.";

            // return response;

            return new WebhookResponse
            {
                FulfillmentMessages = { message },
                FulfillmentText = $"Found some pictures of: {searchTerm}. Now, select a picture.",
                Source = ""
            };
            // return new WebhookResponse 
            // { 
            //     FulfillmentText = $"Found some pictures of: {searchTerm}. Now, select a picture."
            // };
        }

        // Create the CustomSearch client. Note this is not a cloud library.
        private static ImageSearchAPI CreateSearchClient()
        {
            string subscriptionKey = Program.AppSettings.Cognitive.ImageSearch;

            return new ImageSearchAPI(new ApiKeyServiceClientCredentials(subscriptionKey));
        }        
    }
}