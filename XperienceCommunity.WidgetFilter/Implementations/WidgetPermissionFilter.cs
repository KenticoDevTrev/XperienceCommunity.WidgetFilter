using CMS.Base;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using Kentico.Membership;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XperienceCommunity.WidgetFilter
{
    public class WidgetPermissionFilter<TUser> : IWidgetPermissionFilter where TUser : ApplicationUser
    {
        private readonly IUserInfoProvider _userInfoProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<TUser> _userManager;
        private readonly ISiteService _siteService;
        private readonly IProgressiveCache _progressiveCache;

        public WidgetPermissionFilter(IUserInfoProvider userInfoProvider, 
            IHttpContextAccessor httpContextAccessor,
            UserManager<TUser> userManager,
            ISiteService siteService,
            IProgressiveCache progressiveCache)
        {
            _userInfoProvider = userInfoProvider;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _siteService = siteService;
            _progressiveCache = progressiveCache;

            // Build dictionaries
            SetWidgetPermissions();
        }

        private void SetWidgetPermissions()
        {
            // Find filters that apply
            var widgetPermissionAttributes = new List<RegisterWidgetPermissionsAttribute>();

            foreach (var assembly in AssemblyDiscoveryHelper.GetAssemblies(true))
            {
                widgetPermissionAttributes.AddRange(assembly.GetCustomAttributes<RegisterWidgetPermissionsAttribute>());
            }
            // Only one widget permission attribute per widget identity allowed
            WidgetIdentityToPermission = widgetPermissionAttributes.ToDictionary(key => key.WidgetIdentity.ToLowerInvariant(), value => value);
        }

        public Dictionary<string, RegisterWidgetPermissionsAttribute> WidgetIdentityToPermission { get; internal set; }

        public async Task<string[]> WidgetsPermissableAsync(IEnumerable<string> widgetIdentities)
        {
            if (!WidgetIdentityToPermission.Any())
            {
                return widgetIdentities.ToArray();
            }
            var widgetIdentitiesLowerToWidgetIdentities = widgetIdentities.GroupBy(x => x.ToLowerInvariant()).ToDictionary(key => key.Key, value => value.First());
            var lowerIdentityToProperIdentity = widgetIdentities.ToDictionary(key => key.ToLowerInvariant(), value => value);
            var lowerIdentities = widgetIdentities.Select(x => x.ToLowerInvariant());
            var noPermissionWidgets = lowerIdentities.Except(WidgetIdentityToPermission.Keys);
            var permissionedWidgets = lowerIdentities.Except(noPermissionWidgets);

            List<string> allowedWidget = new List<string>();
            allowedWidget.AddRange(noPermissionWidgets);
            if (permissionedWidgets.Any())
            {
                // remove any widgets not on the site
                var widgetsExcludedBySite = permissionedWidgets.Where(x => WidgetIdentityToPermission[x].SiteNames.Any() && !WidgetIdentityToPermission[x].SiteNames.Contains(_siteService.CurrentSite.SiteName, StringComparer.InvariantCultureIgnoreCase));
                permissionedWidgets = permissionedWidgets.Except(widgetsExcludedBySite);

                // All excluded so ignore rest
                if (!permissionedWidgets.Any())
                {
                    return allowedWidget.Select(x => widgetIdentitiesLowerToWidgetIdentities[x]).ToArray();
                }

                // Filter by User / User Type
                var username = (_httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "public").ToLower().Replace("public", "");
                
                // Can't determine username
                if(string.IsNullOrWhiteSpace(username))
                {
                    return allowedWidget.Select(x => widgetIdentitiesLowerToWidgetIdentities[x]).ToArray();
                }

                // Get User
                var currentUser = await _progressiveCache.LoadAsync(async cs =>
                {
                    if(cs.Cached)
                    {
                        cs.CacheDependency = CacheHelper.GetCacheDependency(new string[]
                        {
                            $"{UserInfo.OBJECT_TYPE}|byname|{username}",
                            $"{UserRoleInfo.OBJECT_TYPE}|all",
                            $"{RoleInfo.OBJECT_TYPE}|all",
                            $"{MembershipRoleInfo.OBJECT_TYPE}|all",
                            $"{MembershipUserInfo.OBJECT_TYPE}|all",
                        });
                    }
                    return await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
                }, new CacheSettings(30, "WidgetPermissionsUserManager", username));

                var currentuserInfo = await _progressiveCache.LoadAsync(async cs =>
                {
                    if (cs.Cached)
                    {
                        cs.CacheDependency = CacheHelper.GetCacheDependency(new string[]
                        {
                            $"{UserInfo.OBJECT_TYPE}|byname|{username}",
                            $"{UserRoleInfo.OBJECT_TYPE}|all",
                            $"{RoleInfo.OBJECT_TYPE}|all",
                            $"{MembershipRoleInfo.OBJECT_TYPE}|all",
                            $"{MembershipUserInfo.OBJECT_TYPE}|all",
                        });
                    }
                    return await _userInfoProvider.GetAsync(username);
                }, new CacheSettings(30, "WidgetPermissionsUserInfo", username));
                
                foreach (var permissionedWidget in permissionedWidgets)
                {
                    var permissionAttribute = WidgetIdentityToPermission[permissionedWidget];
                    bool allowed = true;

                    // Roles don't impact Admin / global Admin
                    bool checkRoles = currentuserInfo.SiteIndependentPrivilegeLevel == UserPrivilegeLevelEnum.Editor || currentuserInfo.SiteIndependentPrivilegeLevel == UserPrivilegeLevelEnum.None;

                    // Check roles
                    if (allowed && checkRoles && permissionAttribute.RoleNames.Any())
                    {
                        if (!permissionAttribute.RoleNames.Intersect(currentUser.Roles).Any())
                        {
                            allowed = false;
                        }
                    }
                    if (allowed)
                    {
                        switch (permissionAttribute.WidgetPermissionMinimumUserType)
                        {
                            case WidgetPermissionMinimumUserType.Adminstrator:
                                if (currentuserInfo.SiteIndependentPrivilegeLevel == UserPrivilegeLevelEnum.Editor || currentuserInfo.SiteIndependentPrivilegeLevel == UserPrivilegeLevelEnum.None)
                                {
                                    allowed = false;
                                }
                                break;
                            case WidgetPermissionMinimumUserType.GlobalAdministrator:
                                if (currentuserInfo.SiteIndependentPrivilegeLevel != UserPrivilegeLevelEnum.GlobalAdmin)
                                {
                                    allowed = false;
                                }
                                break;
                        }
                    }

                    // Passes
                    if(allowed)
                    {
                        allowedWidget.Add(permissionedWidget);
                    }
                }
            }

            //return allowedWidget.ToArray();
            return allowedWidget.Select(x => widgetIdentitiesLowerToWidgetIdentities[x]).ToArray();
        }
    }
}
