using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace identiity.Infrastructure
{
    [HtmlTargetElement("td", Attributes ="identity-claim-type")]
    public class ClaimTypeTagHelper : TagHelper
    {
        [HtmlAttributeName("identity-claim-type")]
        public string ClaimType { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            bool foundType = false;
            var fields = typeof(ClaimTypes).GetFields();
            foreach (var fi in fields)
            {
                if (fi.GetValue(null).ToString() == ClaimType)
                {
                    output.Content.SetContent(fi.Name);
                    foundType = true;
                }
                if (!foundType)
                {
                    //output.Content.SetContent(ClaimType.Split('/', '.').Last());
                    output.Content.SetContent(ClaimType);
                }
            }
        }
    }
}
