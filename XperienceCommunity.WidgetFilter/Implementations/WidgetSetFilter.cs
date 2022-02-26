using CMS.Core;
using Kentico.PageBuilder.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XperienceCommunity.WidgetFilter
{
    public class WidgetSetFilter : IWidgetSetFilter
    {
        public WidgetSetFilter()
        {
            // Build dictionaries
            SetWidgetSets();
        }

        private void SetWidgetSets()
        {
            var widgetSetAttributes = new List<RegisterWidgetToSetsAttribute>();

            // Find filters that apply
            foreach (var assembly in AssemblyDiscoveryHelper.GetAssemblies(true))
            {
                widgetSetAttributes.AddRange(assembly.GetCustomAttributes<RegisterWidgetToSetsAttribute>());
            }

            // Get all registered widgets
            var allRegisteredWidgets = new List<RegisterWidgetAttribute>();
            foreach (var assembly in AssemblyDiscoveryHelper.GetAssemblies(true))
            {
                allRegisteredWidgets.AddRange(assembly.GetCustomAttributes<RegisterWidgetAttribute>());
            }
            var allRegisteredWidgetIdentities = allRegisteredWidgets.Select(x => x.Identifier);

            // General widgets are widgets that have NO WidgetSet attribute, OR have an attribute but IncludeInGeneralWidgets is true
            var attributedButStillGeneralWidgets = widgetSetAttributes.Where(x => x.IncludeInGeneralWidgets).Select(x => x.WidgetIdentity);
            GeneralWidgetNames = allRegisteredWidgetIdentities.Except(widgetSetAttributes.Select(x => x.WidgetIdentity), StringComparer.InvariantCultureIgnoreCase).ToList();
            GeneralWidgetNames.AddRange(attributedButStillGeneralWidgets);
            GeneralWidgetNames = GeneralWidgetNames.Distinct().ToList();

            // Widget Set Names by their set name
            var distinctSetNames = widgetSetAttributes.SelectMany(x => x.SetNames).Select(x => x.ToLower()).Distinct();
            var setNameToWidgetIdentities = new Dictionary<string, IEnumerable<string>>();
            foreach (var setName in distinctSetNames)
            {
                setNameToWidgetIdentities.Add(setName, widgetSetAttributes.Where(x => x.SetNames.Contains(setName, StringComparer.InvariantCultureIgnoreCase)).Select(x => x.WidgetIdentity).Distinct());

            }
            WidgetSetNames = setNameToWidgetIdentities;
        }

        public Dictionary<string, IEnumerable<string>> WidgetSetNames { get; internal set; }
        public List<string> GeneralWidgetNames { get; internal set; }

        public Task<string[]> WidgetsBySetsAsync(IEnumerable<string> setNames, bool includeGeneralWidgets = false)
        {
            if (setNames?.Any() ?? false)
            {
                var validWidgets = WidgetSetNames.Where(x => setNames.Contains(x.Key, StringComparer.InvariantCultureIgnoreCase)).SelectMany(x => x.Value).Distinct().ToList();
                if (includeGeneralWidgets)
                {
                    validWidgets.AddRange(GeneralWidgetNames);
                }
                return Task.FromResult(validWidgets.Distinct().ToArray());
            }
            return Task.FromResult(GeneralWidgetNames.ToArray());
        }
    }
}
