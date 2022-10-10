﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC6.Utilities
{
	[HtmlTargetElement("ul", Attributes = "page-model")]
	public class PageLinkTagHelper : TagHelper
	{
        private readonly IUrlHelperFactory _urlHelper;

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        #region Base
        [HtmlAttributeName("page-model")]
        public PagingInfo PageModel { get; set; }

        [HtmlAttributeName("page-controller")]
        public string PageController { get; set; }

        [HtmlAttributeName("page-action")]
        public string PageAction { get; set; }
        #endregion

        #region CSS
        [HtmlAttributeName("page-classes-enabled")]
        public bool PageClassesEnabled { get; set; } = false;

        [HtmlAttributeName("page-class")]
        public string PageClass { get; set; }

        [HtmlAttributeName("page-a-class")]
        public string PageAClass { get; set; }

        [HtmlAttributeName("page-class-normal")]
        public string PageClassNormal { get; set; }

        [HtmlAttributeName("page-class-selected")]
        public string PageClassSelected { get; set; }
        #endregion

        public PageLinkTagHelper(IUrlHelperFactory urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            IUrlHelper urlHelper = _urlHelper.GetUrlHelper(ViewContext);
            TagBuilder ul = new TagBuilder("ul");
            ul.Attributes["class"] = "pagination";

            #region First
            TagBuilder liFirst = new TagBuilder("li");
            TagBuilder aFirst = new TagBuilder("a");
            liFirst.Attributes["id"] = "first";
            aFirst.Attributes["href"] = PageModel.CurrentPage == 1 ?
                                        "#" : urlHelper.Action(PageAction,
                                                               PageController,
                                                               new
                                                               {
                                                                   PageNo = 1,
                                                                   PageSize = PageModel.ItemsPerPage,
                                                                   KeyWord = PageModel.SearchKeyword,
                                                                   SearchSubject = PageModel.SearchSubject
                                                               });
            if (PageClassesEnabled)
            {
                liFirst.AddCssClass(PageClass + " first");
                liFirst.AddCssClass(PageModel.CurrentPage == 1 ?
                                    "disabled" : PageClassNormal);
                aFirst.AddCssClass(PageAClass);
            }

            aFirst.InnerHtml.AppendHtml("First");
            liFirst.InnerHtml.AppendHtml(aFirst);
            ul.InnerHtml.AppendHtml(liFirst);
            #endregion

            #region Previous
            TagBuilder liPrev = new TagBuilder("li");
            TagBuilder aPrev = new TagBuilder("a");
            liPrev.Attributes["id"] = "previous";
            aPrev.Attributes["href"] = PageModel.CurrentPage == 1 ?
                                        "#" : urlHelper.Action(PageAction,
                                                               PageController,
                                                               new
                                                               {
                                                                   PageNo = PageModel.CurrentPage - 1,
                                                                   PageSize = PageModel.ItemsPerPage,
                                                                   KeyWord = PageModel.SearchKeyword,
                                                                   SearchSubject = PageModel.SearchSubject
                                                               });
            //CSS
            if (PageClassesEnabled)
            {
                liPrev.AddCssClass(PageClass + " previous");
                liPrev.AddCssClass(PageModel.CurrentPage == 1 ?
                                    "disabled" : PageClassNormal);
                aPrev.AddCssClass(PageAClass);
            }

            aPrev.InnerHtml.AppendHtml("Previous");
            liPrev.InnerHtml.AppendHtml(aPrev);
            ul.InnerHtml.AppendHtml(liPrev);
            #endregion

            #region Numbers
            int pagingGroup = (int)Math.Ceiling((decimal)PageModel.CurrentPage / PageModel.NumberLinksPerPage);
            PageModel.StartPage = (pagingGroup * PageModel.NumberLinksPerPage)
                                  - PageModel.NumberLinksPerPage + 1;
            PageModel.EndPage = PageModel.StartPage + (PageModel.NumberLinksPerPage - 1);

            if (PageModel.EndPage >= PageModel.TotalPage)
            {
                PageModel.EndPage = PageModel.TotalPage;
            }

            for (int index = PageModel.StartPage; index <= PageModel.EndPage;
                index++)
            {
                TagBuilder li = new TagBuilder("li");
                TagBuilder a = new TagBuilder("a");
                string aLink = urlHelper.Action(PageAction,
                                                PageController,
                                                new
                                                {
                                                    PageNo = index,
                                                    PageSize = PageModel.ItemsPerPage,
                                                    KeyWord = PageModel.SearchKeyword,
                                                    SearchSubject = PageModel.SearchSubject
                                                });
                if (PageModel.CurrentPage == index)
                {
                    aLink = "#";
                }

                a.Attributes["href"] = aLink;

                // CSS
                if (PageClassesEnabled)
                {
                    li.AddCssClass(PageClass);
                    li.AddCssClass(PageModel.CurrentPage == index ?
                                   PageClassSelected : PageClassNormal);
                    a.AddCssClass(PageAClass);
                }

                a.InnerHtml.AppendHtml(index.ToString());
                li.InnerHtml.AppendHtml(a);
                ul.InnerHtml.AppendHtml(li);
            }
            #endregion

            #region Next
            TagBuilder liNext = new TagBuilder("li");
            TagBuilder aNext = new TagBuilder("a");
            liNext.Attributes["id"] = "next";
            aNext.Attributes["href"] = PageModel.CurrentPage == PageModel.TotalPage
                                       || PageModel.TotalItems == 0 ?
                                       "#" : urlHelper.Action(PageAction,
                                                              PageController,
                                                              new
                                                              {
                                                                  PageNo = PageModel.CurrentPage + 1,
                                                                  PageSize = PageModel.ItemsPerPage,
                                                                  KeyWord = PageModel.SearchKeyword,
                                                                  SearchSubject = PageModel.SearchSubject
                                                              });
            //CSS
            if (PageClassesEnabled)
            {
                liNext.AddCssClass(PageClass + " next");
                liNext.AddCssClass(PageModel.CurrentPage == PageModel.TotalPage
                                   || PageModel.TotalItems == 0 ?
                                   "disabled" : PageClassNormal);
                aNext.AddCssClass(PageAClass);
            }

            aNext.InnerHtml.AppendHtml("Next");
            liNext.InnerHtml.AppendHtml(aNext);
            ul.InnerHtml.AppendHtml(liNext);
            #endregion

            #region Last
            TagBuilder liLast = new TagBuilder("li");
            TagBuilder aLast = new TagBuilder("a");
            liLast.Attributes["id"] = "last";
            aLast.Attributes["href"] = PageModel.CurrentPage == PageModel.TotalPage
                                       || PageModel.TotalItems == 0 ?
                                       "#" : urlHelper.Action(PageAction,
                                                              PageController,
                                                              new
                                                              {
                                                                  PageNo = PageModel.TotalPage,
                                                                  PageSize = PageModel.ItemsPerPage,
                                                                  KeyWord = PageModel.SearchKeyword,
                                                                  SearchSubject = PageModel.SearchSubject
                                                              });
            //CSS
            if (PageClassesEnabled)
            {
                liLast.AddCssClass(PageClass + " last");
                liLast.AddCssClass(PageModel.CurrentPage == PageModel.TotalPage
                                   || PageModel.TotalItems == 0 ?
                                   "disabled" : PageClassNormal);
                aLast.AddCssClass(PageAClass);
            }

            aLast.InnerHtml.AppendHtml("Last");
            liLast.InnerHtml.AppendHtml(aLast);
            ul.InnerHtml.AppendHtml(liLast);
            #endregion

            output.Content.AppendHtml(ul.InnerHtml);
        }
    }
}
