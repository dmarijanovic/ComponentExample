using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Enterwell.Helpers
{
    public static class TableHelper
    {

        public static MvcHtmlString SortOrderFor<TModel, TValue>(
                        this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string sortOrder, string filter)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string resolvedLabelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();

            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            string propertyName = metadata.PropertyName.ToLower();
            string paramSortOrder = sortOrder == propertyName ? propertyName + "_desc" : propertyName;
            var url = urlHelper.Action("Index", new { sortOrder = paramSortOrder, filter = filter });

            string orderCSSClass = "glyphicon-sort";
            if (propertyName == sortOrder)
            {
                orderCSSClass = "glyphicon-sort-by-attributes";
            }
            else if (propertyName + "_desc" == sortOrder)
            {
                orderCSSClass = "glyphicon-sort-by-attributes-alt";
            }

            string tag = String.Format("<a href=\"{0}\">" +
                "{1}" +
                "<span class=\"glyphicon {2}\" style=\"padding-left: 10px\"></span>" +
                "</a>", url, resolvedLabelText, orderCSSClass);

            return MvcHtmlString.Create(tag);
        }

        public static MvcHtmlString MenuItem(this HtmlHelper html, string label, string action, string controller, bool requireAuth, bool fullMatch)
        {
            if (requireAuth && !html.ViewContext.HttpContext.Request.IsAuthenticated)
            {
                return MvcHtmlString.Empty;
            }
            else
            {
                string activeController = html.ViewContext.RouteData.Values["controller"].ToString();
                string activeAction = html.ViewContext.RouteData.Values["action"].ToString();

                string cssClass = activeController.Equals(controller) && (!fullMatch || activeAction.Equals(action)) ? "active" : "";
                string actionLink = html.ActionLink(label, action, controller).ToString();

                string tag = String.Format("<li class=\"{0}\">{1}</li>", cssClass, actionLink, label);

                return MvcHtmlString.Create(tag);
            }
        }

        public static MvcHtmlString MenuItem(this HtmlHelper html, string label, string action, string controller, bool requireAuth)
        {
            return MenuItem(html, label, action, controller, requireAuth, false);
        }
    }
}