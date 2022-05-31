using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XperienceCommunity.WidgetFilter
{
    [HtmlTargetElement("editable-area")]
    public class EditableAreaWidgetSetTagHelper : EditableAreaTagHelper
    {
        private readonly IWidgetSetFilter _widgetSetFilter;
        private readonly IWidgetPermissionFilter _widgetPermissionFilter;

        /// <summary>
        /// The Widget Set names you wish to use to filter widgets.
        /// </summary>
        public IEnumerable<string> WidgetSets { get; set; }

        /// <summary>
        /// If widget sets provided and this is true, it will also include General widgets.  Default is false when Widget Sets are defined, true otherwise.
        /// </summary>
        public bool? IncludeGeneralWidgets { get; set; }
        public EditableAreaWidgetSetTagHelper(IHtmlHelper htmlHelper,
            IWidgetSetFilter widgetSetFilter,
            IWidgetPermissionFilter widgetPermissionFilter) : base(htmlHelper)
        {
            _widgetSetFilter = widgetSetFilter;
            _widgetPermissionFilter = widgetPermissionFilter;
        }
        
        protected override async Task<IHtmlContentProxy> CallHtmlHelper()
        {
            // Filter by Set and Permissions

            // Preserve any hard coded values
            var hardCodedAllowedWidgets = AreaOptionsAllowedWidgets;

            // if not explicitly defined, show all widgets if no hard coded values
            var includeGeneralWidgets = IncludeGeneralWidgets ?? (hardCodedAllowedWidgets?.Any() ?? false ? false : true);

            // Get the widgets
            var widgets = await _widgetSetFilter.WidgetsBySetsAsync(WidgetSets, includeGeneralWidgets);

            // Add in any hard coded allowed widgets
            widgets = hardCodedAllowedWidgets?.Any() ?? false ? hardCodedAllowedWidgets.Union(widgets).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray() : widgets;

            // Filter list by permissions
            AreaOptionsAllowedWidgets = await _widgetPermissionFilter.WidgetsPermissableAsync(widgets);
            return await base.CallHtmlHelper();
        }
    }
}
