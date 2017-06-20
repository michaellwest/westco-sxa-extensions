using System;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Web;
using Sitecore.XA.Foundation.IoC;
using Sitecore.XA.Foundation.Multisite;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;

namespace Westco.XA.Foundation.Theming.Shell.Controls.RichTextEditor
{
    //http://docs.telerik.com/devtools/aspnet-ajax/controls/editor/functionality/toolbars/dropdowns/css-styles
    //https://jammykam.wordpress.com/2015/03/11/user-specific-or-multi-site-specific-css-styles-in-sitecore-rich-text-editor/
    public class EditorConfiguration : Sitecore.Shell.Controls.RichTextEditor.EditorConfiguration
    {
        public EditorConfiguration(Item profile) : base(profile)
        {
        }

        protected override void SetupStylesheets()
        {
            var contentDatabase = Sitecore.Context.ContentDatabase;

            if (contentDatabase == null || !contentDatabase.Name.Equals("master", StringComparison.OrdinalIgnoreCase))
            {
                base.SetupStylesheets();
                return;
            }

            var id = WebUtil.GetQueryString("id");
            if (string.IsNullOrEmpty(id))
            {
                base.SetupStylesheets();
                return;
            }

            var contentItem = contentDatabase.GetItem(new ID(id));
            if (contentItem == null)
            {
                base.SetupStylesheets();
                return;
            }

            var settingsItem = ServiceLocator.Current.Resolve<IMultisiteContext>().GetSettingsItem(contentItem);
            if (settingsItem == null)
            {
                base.SetupStylesheets();
                return;
            }

            MultilistField stylesheetField = settingsItem.Fields[Templates.ThemeSettings.Fields.RichTextStylesheets];
            if (stylesheetField == null)
            {
                base.SetupStylesheets();
                return;
            }

            var ids = stylesheetField.TargetIDs;
            if (ids.Any())
            {
                const string pattern = @"((?<prefix>[a-zA-Z0-9]+)?(?<name>[\.#][_A-Za-z0-9\-]+))[^}]*{";

                foreach (var item in ids.Select(i => contentDatabase.GetItem(i)))
                {
                    if (item == null || !item.Paths.IsMediaItem) continue;

                    var url = item.BuildAssetPath(true);
                    this.Editor.CssFiles.Add(url);

                    using (var reader = new System.IO.StreamReader(((MediaItem)item).GetMediaStream()))
                    {
                        var text = reader.ReadToEnd();
                        var matches = Regex.Matches(text, pattern);
                        foreach (Match match in matches)
                        {
                            if (!match.Success) continue;
                            var name = $"{match.Groups["prefix"].Value}{match.Groups["name"].Value}";
                            base.Editor.CssClasses.Add(name, name);
                        }
                    }
                }
            }

            base.SetupStylesheets();
        }
    }
}
