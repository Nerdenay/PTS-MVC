using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PatientTrackingSite.TagHelpers
{
    public class FileIconTagHelper : TagHelper
    {
        public string Path { get; set; }
        public string Alt { get; set; } = "Medical File";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var extension = System.IO.Path.GetExtension(Path)?.ToLower();

            if (extension == ".pdf")
            {
                output.TagName = "img";
                output.Attributes.SetAttribute("src", "/images/png-icon.png");
                output.Attributes.SetAttribute("alt", "PDF File");
                output.Attributes.SetAttribute("style", "height: 120px; width: auto; object-fit: contain; display: block; margin: auto;");
            }
            else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
            {
                output.TagName = "img";
                output.Attributes.SetAttribute("src", Path);
                output.Attributes.SetAttribute("alt", Alt);
                output.Attributes.SetAttribute("style", "height: 120px; width: 100%; object-fit: cover; border-radius: 6px;");
            }
            else
            {
                output.TagName = "span";
                output.Content.SetContent("(Unsupported file)");
            }
        }
    }
}
