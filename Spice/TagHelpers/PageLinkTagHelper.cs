using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.TagHelpers
{
    //1. NOT FORGET TO ADD CUSTOM TAG HELPER IN _VIEWIMPORTS.CS
    //2. Properties must has got the same name like the Atrriues in HtmlTargetElement adnotation

    //for which element we want to create the tag helper, atributes
    [HtmlTargetElement("div",Attributes = "page-model")]
    //Extend tag helper
    public class PageLinkTagHelper : TagHelper
    {
        //we need this, becouse we'll be modyfying url based on requirements of current page
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            this.urlHelperFactory = urlHelperFactory;
        }

        //object that provide acces to httpcontext, httprequest and so on
        [ViewContext]
        [HtmlAttributeNotBound] // atribute which we don't want to set in HTML
        public ViewContext ViewContext { get; set; }
        //it must be the same name like in [HtmlTargetElement] ex [HtmlTargetElement("div",Attributes = "page-model")]
        public PagingInfo PageModel { get; set; }
        //action to riderect to
        public string PageAction { get; set; } 
        public bool PageClassesEnable { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder tagBuilder = new TagBuilder("div");
            for (int i = 1; i <= PageModel.TotalPage; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                string url = PageModel.urlParam.Replace(":",i.ToString());
                tag.Attributes["href"] = url;
                if (PageClassesEnable)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                tagBuilder.InnerHtml.AppendHtml(tag);
            }

            //append our tml to output
            output.Content.AppendHtml(tagBuilder);

            base.Process(context, output);
        }

    }
}
