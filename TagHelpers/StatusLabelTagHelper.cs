using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PatientTrackingSite.TagHelpers
{
    public class StatusLabelTagHelper : TagHelper
    {
        public string Status { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var cssClass = Status switch
            {
                "Scheduled" => "badge bg-warning text-dark",
                "Completed" => "badge bg-success",
                "Cancelled" => "badge bg-danger",
                _ => "badge bg-secondary"
            };

            output.TagName = "span";
            output.Attributes.SetAttribute("class", cssClass);
            output.Content.SetContent(Status);

        }
    }
}