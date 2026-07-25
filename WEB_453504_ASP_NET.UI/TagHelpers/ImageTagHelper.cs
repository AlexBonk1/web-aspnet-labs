using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WEB_453504_ASP_NET.UI.TagHelpers
{
    public class ImageTagHelper : TagHelper
    {

        [HtmlTargetElement("img", Attributes = "asp-image-name")]
        public class ImagePathTagHelper : TagHelper
        {
            private readonly IWebHostEnvironment _env;
            public ImagePathTagHelper(IWebHostEnvironment env)
            {
                _env = env;
            }
            public string AspImageName { get; set; }

            public string AspImageFolder { get; } = "images";

            public override void Process(TagHelperContext context, TagHelperOutput output)
            {
                if (string.IsNullOrWhiteSpace(AspImageName))
                {
                    AspImageName = "no-image.jpg"; 
                }

                string physicalPath = Path.Combine(_env.WebRootPath, AspImageFolder, AspImageName);

                if (!File.Exists(physicalPath))
                {
                    AspImageName = "no-image.jpg";
                }

                string relativePath = $"/{AspImageFolder}/{AspImageName}";

                output.Attributes.SetAttribute("src", relativePath);

                output.Attributes.RemoveAll("asp-image-name");
                output.Attributes.RemoveAll("asp-image-folder");
            }
        }
    }
}
