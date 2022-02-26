using System.Collections.Generic;
using System.Threading.Tasks;

namespace XperienceCommunity.WidgetFilter
{
    public interface IWidgetSetFilter
    {
        public Task<string[]> WidgetsBySetsAsync(IEnumerable<string> setNames, bool includeGeneralWidgets = false);
    }
}
