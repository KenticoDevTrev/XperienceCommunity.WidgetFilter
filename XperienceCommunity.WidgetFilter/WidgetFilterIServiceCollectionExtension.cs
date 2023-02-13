using Kentico.Membership;
using Microsoft.Extensions.DependencyInjection;

namespace XperienceCommunity.WidgetFilter
{
    public static class WidgetFilterIServiceCollectionExtension
    {
        public static IServiceCollection AddWidgetFilter<TUser>(this IServiceCollection services) 
            where TUser : ApplicationUser
        {
            return AddWidgetFilterInternal(services)
                .AddScoped<IWidgetPermissionFilter, WidgetPermissionFilter<TUser>>();
        }

        public static IServiceCollection AddWidgetFilter(this IServiceCollection services)
        {
            return AddWidgetFilterInternal(services)
                .AddScoped<IWidgetPermissionFilter, WidgetPermissionFilter<ApplicationUser>>();
        }

        private static IServiceCollection AddWidgetFilterInternal(IServiceCollection services)
        {
            return services
                .AddSingleton<IWidgetSetFilter, WidgetSetFilter>();
        }
    }
}
