# XperienceCommunity.WidgetFilter
Ability to have widgets designated to "Sets" as well as limit widgets based on user, role, or site.

# Requirements
Kentico Xperience 13 on .Net core 3.1 or above

# Installation
1. On your Xperience MVC Site, install the `XperienceCommunity.WidgetFilter` nuget package.
1. In your startup, call `services.AddWidgetFilter()`
1. Add `@addTagHelper *, XperienceCommunity.WidgetFilter` to your `_ViewImports.cshtml` or whichever view you wish to get the tag helper.

# Usage
There are 2 new assembly attributes, `[assembly: RegisterWidgetToSets]` and `[assembly: RegisterWidgetPermissions]`. 

There should be only one `RegisterWidgetToSets` and one `RegisterWidgetPermissions` per widget max.

## Widget Sets
Add `[assembly: RegisterWidgetToSets]` to any widget you wish to designate to a set.  If a widget has no `RegisterWidgetToSets` attribute, it's considered a general widget.

Add `widget-sets` (along with optional `include-general-widgets='true'`) to your `<editable-area/>` tag.

Example:
`<editable-area area-identifier="main" widget-sets="@(new string[] { SiteWidgetSets.BANNERS })" include-general-widgets=true />`

By default, if you specify any widget sets, general widgets will be excluded unless you specifically call `include-general-widgets=true`, likewise if you do not specify any widget sets, general widgets are always shown.

## Widget Permission
Add `[assembly: RegisterWidgetPermissions]` to any widget widget you wish to limit by User type, Role, or Site.  If a widget has no `RegisterWidgetPermissions` attribute, then it will not get filtered out by permission.

Widget identity and the `widgetPermissionMinimumUserType` required, the user type is a minimum so `WidgetPermissionMinimumUserType.Editor` means editors and above can add (all inclusive).  Roles can be an empty array, same for sites, but if you provide then the user either needs to be in the role for roles, or the current site is listed in the sites (site code names) to display.

# Existing Widgets
If a widget is added prior to being excluded, or if a widget that a user doesn't have permission to add is already on the page, that widget is still editable.  However, you cannot copy / paste it and you cannot add a new widget unless the widget is in the widget set or the user has permission.

So be aware that while you may set a widget to only be usable by Global Administrators, for example, once a Global Administrator adds the widget it can be edited by anyone.

# Mixing area-options-allowed-widgets and widget-sets
If you hardcode `area-options-allowed-widgets` on the editable area, please note that the Include Generic Widgets will be defaulted to `false` unless specified, otherwise Generic widgets will be included.  Your hard coded widgets will be included as long as the widget permissions allow them to be included.

# Contributions
If you find a bug, please feel free to submit a pull request!
