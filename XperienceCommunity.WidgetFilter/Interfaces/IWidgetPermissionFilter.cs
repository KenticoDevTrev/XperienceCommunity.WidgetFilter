using System.Collections.Generic;
using System.Threading.Tasks;

namespace XperienceCommunity.WidgetFilter
{
    public interface IWidgetPermissionFilter
    {
        public Task<string[]> WidgetsPermissableAsync(IEnumerable<string> widgetIdentities);
    }
}
