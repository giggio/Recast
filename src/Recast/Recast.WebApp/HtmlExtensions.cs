using System;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Recast.WebApp
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString Script(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_helperresult_" + Guid.NewGuid()] = template;
            return MvcHtmlString.Empty;
        }        
        public static MvcHtmlString Script(this HtmlHelper htmlHelper, params IHtmlString[] htmlStrings)
        {
            foreach (var htmlString in htmlStrings)
                htmlHelper.ViewContext.HttpContext.Items["_script_htmlstring_" + Guid.NewGuid()] = htmlString;                
            return MvcHtmlString.Empty;
        }

        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_helperresult_"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    if (template != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
                else if (key.ToString().StartsWith("_script_htmlstring_"))
                {
                    var htmlString = htmlHelper.ViewContext.HttpContext.Items[key] as IHtmlString;
                    if (htmlString != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(htmlString.ToHtmlString());
                    }
                }
            }
            return MvcHtmlString.Empty;
        }
    }
}