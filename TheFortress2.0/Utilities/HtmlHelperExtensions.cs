using System.Collections.Generic;
using DataAccessLibrary.Models;
using HtmlTags;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.WebEncoders.Testing;

namespace TheFortress.Utilities
{
    public static class HtmlHelperExtensions
    {
        private static readonly List<string> _cardColors = new List<string>
        {
            "card-navy",
            "card-lightblue",
            "card-purple",
            "card-indigo",
            "card-olive",
            "card-teal"
        };

        public static HtmlTag RenderCommentTree(this IHtmlHelper html, CommentModel comment)
        {
            // HtmlTag htmlOutput = new NoTag();
            if (comment.Children == null) return new NoTag();

            var ul = new HtmlTag("ul");
            // htmlOutput.Append(ul);
            if (comment.Children.Count > 0)
            {
                foreach (var child in comment.Children)
                {
                    var li = new HtmlTag("li", ul);
                    var card = new HtmlTag("div", li).AddClass("card make-transparent")
                        .AddClass(_cardColors[child.Height % 6])
                        .Id(child.CommentId.ToString());

                    var cardHeader = new HtmlTag("div", card).AddClass("card-header");
                    var h3 = new HtmlTag("h3", cardHeader).AddClass("card-title").Text(child.UserName);
                    var tools = new HtmlTag("div", cardHeader).AddClass("card-tools");
                    var collapse = new HtmlTag("button", tools).AddClass("btn btn-tool")
                        .Attr("type", "button")
                        .Attr("data-card-widget", "collapse");
                    var collapseImg = new HtmlTag("i", collapse).AddClass("fas fa-minus");
                    var reply = new HtmlTag("button", tools).AddClass("btn btn-tool")
                        .Attr("type", "button")
                        .Attr("data-toggle", "modal")
                        .Attr("href", "#replyModal")
                        .Attr("data-parentid", child.CommentId)
                        .Attr("data-eventid", child.EventId);
                    var replyImg = new HtmlTag("i", reply).AddClass("fas fa-reply");

                    var cardBody = new HtmlTag("div", card).AddClass("card-body make-transparent");
                    var bodyText = new HtmlTag("p", cardBody).AddClass("text-light")
                        .Text(child.Content);
                    var br = new HtmlTag("br");
                    cardBody.Append(html.RenderCommentTree(child));
                }
            }

            return ul;
        }

        public static HtmlTag RenderConcertCard(this IHtmlHelper hmtl, LocalConcert lc)
        {
            var card = new HtmlTag("div").AddClass("card card-transparent");

            var img1 = new HtmlTag("img", card).AddClass("card-img-top d-lg-none").Attr("height", 320)
                .Attr("src", lc.FlyerUrl).Attr("width", 200)
                .Attr("alt", "...");
            var img2 = new HtmlTag("img", card).AddClass("card-img-top d-none d-lg-block").Attr("height", 420)
                .Attr("src", lc.FlyerUrl).Attr("width", 200)
                .Attr("alt", "...");

            var body1 = new HtmlTag("div", card).AddClass("card-body");
            var h5 = new HtmlTag("h5", body1).AddClass("card-title text-light mx-auto").Text(lc.Artists);
            var p = new HtmlTag("p", body1).AddClass("card-text text-light").Text(lc.VenueName);

            var ul = new HtmlTag("ul", card).AddClass("list-group list-group-flush");
            var li1 = new HtmlTag("li", ul).AddClass("list-group-item make-transparent text-light")
                .Text(lc.TimeStart.ToLongDateString() + " at " + lc.TimeStart.TimeOfDay);
            var li2 = new HtmlTag("li", ul).AddClass("list-group-item make-transparent text-light");
            // var a = new HtmlTag("a", li2).AddClass("btn btn-dark")
            //     .Attr("href", "/Home/Comments?eventid=" + lc.EventConcertId)
            //     .Text("Comments");
            var a = new HtmlTag("a", li2).AddClass("btn btn-dark nav-link comment-show-button")
                .Attr("data-widget", "pushmenu")
                .Text("Comments")
                .Id(lc.EventConcertId.ToString())
                .Attr("onclick", 
                    "showCommentsForEvent(this.id); changeCommentSectionName('" + lc.Artists + "')");

            return card;
        }
        public static HtmlTag RenderHouseShowCard(this IHtmlHelper hmtl, HouseShow hs)
        {
            var card = new HtmlTag("div").AddClass("card card-transparent");

            var img1 = new HtmlTag("img", card).AddClass("card-img-top d-lg-none").Attr("height", 320)
                .Attr("src", hs.FlyerUrl).Attr("width", 200)
                .Attr("alt", "...");
            var img2 = new HtmlTag("img", card).AddClass("card-img-top d-none d-lg-block").Attr("height", 420)
                .Attr("src", hs.FlyerUrl).Attr("width", 200)
                .Attr("alt", "...");

            var body1 = new HtmlTag("div", card).AddClass("card-body");
            var h5 = new HtmlTag("h5", body1).AddClass("card-title text-light mx-auto").Text(hs.Artists);
            var p = new HtmlTag("p", body1).AddClass("card-text text-light").Text(hs.HouseName);

            var ul = new HtmlTag("ul", card).AddClass("list-group list-group-flush");
            var li1 = new HtmlTag("li", ul).AddClass("list-group-item make-transparent text-light")
                .Text(hs.TimeStart.ToLongDateString() + " at " + hs.TimeStart.TimeOfDay);
            var li2 = new HtmlTag("li", ul).AddClass("list-group-item make-transparent text-light");
            // var a = new HtmlTag("a", li2).AddClass("btn btn-dark")
            //     .Attr("href", "/Home/Comments?eventid=" + lc.EventConcertId)
            //     .Text("Comments");
            var a = new HtmlTag("a", li2).AddClass("btn btn-dark nav-link comment-show-button")
                .Attr("data-widget", "pushmenu")
                .Text("Comments")
                .Id(hs.EventConcertId.ToString())
                .Attr("onclick", 
                    "showCommentsForEvent(this.id); changeCommentSectionName('" + hs.Artists + "')");

            return card;
        }
    }
}