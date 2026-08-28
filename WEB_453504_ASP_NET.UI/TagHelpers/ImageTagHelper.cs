using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WEB_453504_ASP_NET.UI.TagHelpers
{
    public class ImageTagHelper : TagHelper
    {

        [HtmlTargetElement("img", Attributes = "asp-image-name")]
        public class ImagePathTagHelper : TagHelper
        {
            private readonly IWebHostEnvironment _env;
            private readonly IConfiguration _configuration;
            public ImagePathTagHelper(IWebHostEnvironment env, IConfiguration configuration)
            {
                _env = env;
                _configuration = configuration;
            }
            public string AspImageName { get; set; }


            public override void Process(TagHelperContext context, TagHelperOutput output)
            {
                if (string.IsNullOrWhiteSpace(AspImageName))
                {
                    AspImageName = "no-image.jpg";
                }
                
                var physicalPath = Path.Combine(_configuration["ApiHost"], AspImageName);
                
                output.Attributes.SetAttribute("src", physicalPath);

                output.Attributes.RemoveAll("asp-image-name");
                output.Attributes.RemoveAll("asp-image-folder");
            }
        }
    }
}
