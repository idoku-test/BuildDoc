using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;


namespace BuildDoc.Web
{
    public static class HtmlDropDownExtensions
    {
        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, TEnum selectedValue)
        {
            return EnumDropDownList(htmlHelper, name, selectedValue, "", null);
        }

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, TEnum selectedValue, string optionLabel, object htmlAttributes)
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            var items =
                from value in values
                let fi = value.GetType().GetField(value.ToString())
                let attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault()
                let text = attribute == null ? value.ToString() : ((DescriptionAttribute)attribute).Description
                select new SelectListItem
                {
                    Text = text,
                    Value = Convert.ToInt32(value).ToString(),                    
                    Selected = (value.Equals(selectedValue))
                };
            return htmlHelper.DropDownList(name, items, optionLabel, htmlAttributes);
        }


        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, TEnum selectedValue, string optionLabel, bool isEnumValue, object htmlAttributes)
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            var items =
                from value in values
                let fi = value.GetType().GetField(value.ToString())
                let attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault()
                let text = attribute == null ? value.ToString() : ((DescriptionAttribute)attribute).Description
                select new SelectListItem
                {
                    Text = text,
                    Value = isEnumValue? value.ToString() : Convert.ToInt32(value).ToString(),
                    Selected = (value.Equals(selectedValue))
                };
            return htmlHelper.DropDownList(name, items, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString EnumDropDownList(this HtmlHelper helper, string name, Type type, object selected, string optionLabel,bool isEnumValue, object htmlAttributes)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Type is not an enum.");

            if (selected != null && selected.GetType() != type)
                throw new ArgumentException("Selected object is not " + type.ToString());
            var values = Enum.GetValues(type).Cast<object>();

            var items =
               from value in values
               let fi = value.GetType().GetField(value.ToString())
               let attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault()
               let text = attribute == null ? value.ToString() : ((DescriptionAttribute)attribute).Description
               select new SelectListItem
               {
                   Text = text,
                   Value = isEnumValue ? value.ToString() : Convert.ToInt32(value).ToString(),
                   Selected = (value.Equals(selected))
               };

            return System.Web.Mvc.Html.SelectExtensions.DropDownList(helper, name, items, optionLabel, htmlAttributes);
        }         
    }
}