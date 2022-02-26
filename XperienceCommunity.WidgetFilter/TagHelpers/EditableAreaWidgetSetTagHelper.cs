using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
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
        public bool IncludeGeneralWidgets { get; set; }=false;
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
            var widgets = await _widgetSetFilter.WidgetsBySetsAsync(WidgetSets, IncludeGeneralWidgets);
            AreaOptionsAllowedWidgets = await _widgetPermissionFilter.WidgetsPermissableAsync(widgets);
            return await base.CallHtmlHelper();
        }
    }
}
