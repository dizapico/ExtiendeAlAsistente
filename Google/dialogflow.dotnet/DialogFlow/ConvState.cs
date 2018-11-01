using System.Collections.Generic;

namespace dialogflow.dotnet.DialogFlow 
{
    public class ConvState
    {
        public class Image {
            public string Title { get; set; }
            public string Url { get; set; }
        }

        public List<Image> ImageList { get; set; }
        public Image FocusedImage { get; set; }
    }
}