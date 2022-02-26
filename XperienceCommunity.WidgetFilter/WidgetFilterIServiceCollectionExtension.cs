using Microsoft.Extensions.DependencyInjection;

namespace XperienceCommunity.WidgetFilter
{
    public static class WidgetFilterIServiceCollectionExtension
    {
        public static IServiceCollection AddWidgetFilter(this IServiceCollection services)
        {
            services.AddSingleton<IWidgetSetFilter, WidgetSetFilter>();
            services.AddScoped<IWidgetPermissionFilter, WidgetPermissionFilter>();
            return services;
        }
    }
}
