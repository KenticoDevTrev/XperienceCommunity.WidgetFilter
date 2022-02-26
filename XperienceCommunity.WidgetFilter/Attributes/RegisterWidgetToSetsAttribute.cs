using System;

namespace Kentico.PageBuilder.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterWidgetToSetsAttribute : Attribute
    {
        /// <summary>
        /// Registers a widget to the given sets.  Recommended you use string constants in a class for the set names.  If include in General Widgets is true then this will show normally, if false it will only show in zones with the widget-set parameter
        /// </summary>
        /// <param name="widgetIdentity">The Widget Code Identity</param>
        /// <param name="setNames">Array of Set Names this should show in.</param>
        /// <param name="includeInGeneralWidgets">If this widget is also considered a general widget (should show for general widget zones)</param>
        public RegisterWidgetToSetsAttribute(string widgetIdentity, string[] setNames, bool includeInGeneralWidgets)
        {
            SetNames = setNames ?? Array.Empty<string>();
            WidgetIdentity = widgetIdentity;
            IncludeInGeneralWidgets = includeInGeneralWidgets;
        }
        public string[] SetNames { get; }
        public string WidgetIdentity { get; }
        public bool IncludeInGeneralWidgets { get; }
    }
}
