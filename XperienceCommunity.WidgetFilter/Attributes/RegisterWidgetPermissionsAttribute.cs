using System;

namespace Kentico.PageBuilder.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterWidgetPermissionsAttribute : Attribute
    {
        /// <summary>
        /// Registers a widget's permissions.  Provide empty arrays for RoleName or SiteName if it doesn't matter.
        /// </summary>
        /// <param name="widgetIdentity">The Widget Identity</param>
        /// <param name="widgetPermissionMinimumUserType">Minimum User Type (includes all higher user types)</param>
        /// <param name="roleNames">Roles the current user must be to add.  Only affects Editors.</param>
        /// <param name="siteNames">Sites this widget is allowed to show on.</param>
        public RegisterWidgetPermissionsAttribute(string widgetIdentity, WidgetPermissionMinimumUserType widgetPermissionMinimumUserType, string[] roleNames, string[] siteNames)
        {
            WidgetIdentity = widgetIdentity;
            WidgetPermissionMinimumUserType = widgetPermissionMinimumUserType;
            RoleNames = roleNames ?? Array.Empty<string>();
            SiteNames = siteNames ?? Array.Empty<string>();
        }
        public string WidgetIdentity { get; }
        public WidgetPermissionMinimumUserType WidgetPermissionMinimumUserType { get; }
        public string[] RoleNames { get; }
        public string[] SiteNames { get; }
    }

    public enum WidgetPermissionMinimumUserType
    {
        Editor, Adminstrator, GlobalAdministrator
    }
}
