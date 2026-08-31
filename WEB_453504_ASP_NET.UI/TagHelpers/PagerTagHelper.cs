using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WEB_453504_ASP_NET.Domain.Entities;

namespace WEB_453504_ASP_NET.UI.TagHelpers;

[HtmlTargetElement("Pager")]
public class PagerTagHelper(LinkGenerator linkGenerator,IHttpContextAccessor httpContextAccessor) : TagHelper
{
    [HtmlAttributeName("current-page")]
    public int CurrentPage { get; set; }
    [HtmlAttributeName("total-pages")]
    public int TotalPages { get; set; }

    [HtmlAttributeName("is-admin")]
    public bool IsAdmin { get; set; } = false;
    
    [HtmlAttributeName("category")]
    public string Category { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "Pager";
        output.TagMode = TagMode.StartTagAndEndTag;
    
        int prev = CurrentPage > 1 ? CurrentPage - 1 : 1;
        int next = CurrentPage < TotalPages ? CurrentPage + 1 : TotalPages;

        var ul = new TagBuilder("ul");
        ul.AddCssClass("pagination");

        ul.InnerHtml.AppendHtml(CreatePageItem(prev, "&laquo;"));
        ul.InnerHtml.AppendHtml(CreatePageItem(prev, prev == CurrentPage ? "&nbsp;" : prev.ToString()));
        ul.InnerHtml.AppendHtml(CreatePageItem(CurrentPage, CurrentPage.ToString(), isActive: true));
        ul.InnerHtml.AppendHtml(CreatePageItem(next, next == CurrentPage ? "&nbsp;" : next.ToString()));
        ul.InnerHtml.AppendHtml(CreatePageItem(next, "&raquo;"));

        output.Content.AppendHtml(ul);

    }

    private string CreatePageUrl(int pageNo)
    {
        if(IsAdmin)
        {
            var url = linkGenerator.GetPathByPage(
                httpContextAccessor.HttpContext,
                "Admin/Index",
                values: new { pageNo = pageNo, category = Category }
            );
            return url ?? "#";
        }
        else
        {
            var url = linkGenerator.GetUriByAction(
                httpContextAccessor.HttpContext,
                action: "Index",
                controller: "Product",
                values: new { pageNo = pageNo, category = Category }
            );
            return url ?? "#";
            }   
        }

    private TagBuilder CreatePageItem(int pageNo, string text, bool isActive = false)
    {
        var li = new TagBuilder("li");
        li.AddCssClass("page-item");

        var a = new TagBuilder("a");
        a.AddCssClass("page-link");
        if (isActive)
        {
            a.AddCssClass("active");
        }
        
        a.Attributes["width"] = "45px";
        
        a.Attributes["href"] = CreatePageUrl(pageNo) ?? "#";
        a.InnerHtml.AppendHtml(text);

        li.InnerHtml.AppendHtml(a);
        return li;
    }
}