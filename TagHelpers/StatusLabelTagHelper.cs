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
                "Scheduled" => "badge bg-warning text-dark",  // Burda randevu alınıyor
                "Completed" => "badge bg-success", // Randevu sonucu doktor tamamlandı diyor
                "Cancelled" => "badge bg-danger", // Doktor iptal etti
                _ => "badge bg-secondary"
            };

            output.TagName = "span";
            output.Attributes.SetAttribute("class", cssClass);
            output.Content.SetContent(Status);

        }
    }
}