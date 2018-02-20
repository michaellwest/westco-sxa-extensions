using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.StringExtensions;
using Sitecore.XA.Foundation.Multisite;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Sitecore.XA.Foundation.Theming.Bundler;
using Sitecore.XA.Foundation.Theming.Configuration;
using Sitecore.XA.Foundation.Theming.Pipelines.AssetService;

namespace Westco.XA.Foundation.Theming.Pipelines.AssetService
{
    public class AddPlainIncludes : AddAssetsProcessor
    {
        public override void Process(AssetsArgs args)
        {
            if (!IsSxaPage()) return;
            var assetConfiguration = AssetConfigurationReader.Read();

            var contextItem = Context.Item;
            if (contextItem == null) return;

            var siteItem = ServiceLocator.ServiceProvider.GetService<IMultisiteContext>().GetSiteItem(contextItem);

            var settingsItem = siteItem?.Children["Settings"];
            if (settingsItem == null) return;

            var assetIds = new string[0];
            var siteAssetsIds = settingsItem[Templates.Page.Fields.AssociatedAssets]?.Split('|');
            var pageAssetsIds = contextItem[Templates.Page.Fields.AssociatedAssets]?.Split('|');

            if (siteAssetsIds != null && siteAssetsIds.Any()) assetIds = assetIds.Union(siteAssetsIds).ToArray();

            if (pageAssetsIds != null && pageAssetsIds.Any()) assetIds = assetIds.Union(pageAssetsIds).ToArray();

            if (!assetIds.Any()) return;

            var assetsList = new List<AssetInclude>();
            const int num = 0;

            var plainIncludeItems =
                contextItem.Axes.SelectItems($"/sitecore/media library//*[@@templateid='{Templates.PlainInclude.Id}']");
            if (!plainIncludeItems.Any()) return;

            foreach (var assetId in assetIds)
            {
                if (assetId.IsNullOrEmpty() || !ID.IsID(assetId)) continue;
                var plainIncludeItem = plainIncludeItems.FirstOrDefault(pii => pii.ID.ToString() == assetId);
                if (plainIncludeItem == null) continue;

                var mode = GetEnumFieldValue(plainIncludeItem.Fields[Templates.PlainInclude.Fields.Mode]);
                if (string.IsNullOrEmpty(mode) || mode.Equals("Disabled", StringComparison.OrdinalIgnoreCase)) continue;

                var assetTypeEx = (AssetTypeEx) Enum.Parse(typeof(AssetTypeEx), mode);
                var assetType = assetTypeEx == AssetTypeEx.Style ? AssetType.Style : AssetType.Script;

                var attributes = new Dictionary<string, string>();

                if (!plainIncludeItem[Templates.PlainInclude.Fields.SriHash].IsNullOrEmpty())
                    attributes.Add("integrity", plainIncludeItem[Templates.PlainInclude.Fields.SriHash]);

                if (!plainIncludeItem[Templates.PlainInclude.Fields.Cors].IsNullOrEmpty())
                    attributes.Add("crossorigin",
                        GetEnumFieldValue(plainIncludeItem.Fields[Templates.PlainInclude.Fields.Cors]));

                switch (assetTypeEx)
                {
                    case AssetTypeEx.ScriptAsync:
                        attributes.Add("async", "");
                        break;
                    case AssetTypeEx.ScriptDefer:
                        attributes.Add("defer", "");
                        break;
                }

                var url = plainIncludeItem[Templates.PlainInclude.Fields.AssetUrl];
                var joinedAttributes = string.Join(" ",
                    attributes.Where(a => !a.Value.IsNullOrEmpty()).Select(a => $"{a.Key}=\"{a.Value}\""));
                joinedAttributes += string.Join(" ",
                    attributes.Where(a => a.Value.IsNullOrEmpty()).Select(a => $"{a.Key}"));

                var urlContent = GetUrlContent(assetType, url, joinedAttributes);

                if (string.IsNullOrEmpty(urlContent)) continue;

                var urlInclude = new PlainInclude
                {
                    SortOrder = num,
                    Name = plainIncludeItem.Name,
                    Type = assetType,
                    Content = urlContent
                };
                assetsList.Add(urlInclude);

                var isFallbackEnabled =
                    MainUtil.GetBool(plainIncludeItem[Templates.PlainInclude.Fields.IsFallbackEnabled], false);
                var fallbackTest = plainIncludeItem[Templates.PlainInclude.Fields.FallbackTest];
                if (isFallbackEnabled && !string.IsNullOrEmpty(fallbackTest))
                {
                    var tagBuilder = new StringBuilder();
                    foreach (var script in ProcessAssets(plainIncludeItem, AssetType.Script,
                        assetConfiguration.ScriptsMode))
                        tagBuilder.Append(script);

                    foreach (var link in ProcessAssets(plainIncludeItem, AssetType.Style,
                        assetConfiguration.StylesMode))
                        tagBuilder.Append(link);

                    if (tagBuilder.Length > 0)
                    {
                        var fallbackInclude = new PlainInclude
                        {
                            SortOrder = num,
                            Name = plainIncludeItem.Name + "-fallback",
                            Type = AssetType.Script,
                            Content = "<script>{0} || document.write('{1}')</script>".FormatWith(fallbackTest,
                                tagBuilder.ToString())
                        };
                        assetsList.Add(fallbackInclude);
                    }
                }

                var rawContent = GetRawContent(assetType, plainIncludeItem[Templates.PlainInclude.Fields.RawContent]);
                if (string.IsNullOrEmpty(rawContent)) continue;

                var rawInclude = new PlainInclude
                {
                    SortOrder = num,
                    Name = plainIncludeItem.Name,
                    Type = assetType,
                    Content = rawContent
                };
                assetsList.Add(rawInclude);
            }

            if (assetsList.Any()) args.AssetsList.InsertRange(0, assetsList.ToArray());
        }

        private IEnumerable<string> ProcessAssets(Item plainIncludeItem, AssetType assetType,
            AssetServiceMode serviceMode)
        {
            Item[] selectedItems = null;
            if (assetType == AssetType.Script)
                selectedItems = plainIncludeItem.Axes.SelectItems("./Scripts//*[@Extension='js']");
            else if (assetType == AssetType.Style)
                selectedItems = plainIncludeItem.Axes.SelectItems("./Styles//*[@Extension='css']");

            var assets = selectedItems != null && selectedItems.Any()
                ? new List<Item>(selectedItems)
                : new List<Item>();
            var isOptimized = true;
            switch (serviceMode)
            {
                case AssetServiceMode.Disabled:
                    assets = assets
                        .Where(i => i.TemplateID != Sitecore.XA.Foundation.Theming.Templates.OptimizedFile.ID).ToList();
                    isOptimized = false;
                    break;
                case AssetServiceMode.Concatenate:
                case AssetServiceMode.ConcatenateAndMinify:
                    var optimizedItem = assets.FirstOrDefault(i =>
                        i.TemplateID == Sitecore.XA.Foundation.Theming.Templates.OptimizedFile.ID);
                    if (optimizedItem != null && !IsNotEmpty(optimizedItem))
                    {
                        assets = new List<Item> {optimizedItem};
                    }
                    else
                    {
                        optimizedItem = new AssetBundler().GetOptimizedItem(plainIncludeItem,
                            (OptimizationType) assetType, serviceMode);
                        if (optimizedItem != null && IsNotEmpty(optimizedItem))
                            assets = new List<Item> {optimizedItem};
                        else
                            assets = new List<Item>();
                    }

                    break;
            }

            switch (assetType)
            {
                case AssetType.Script:
                    return assets.Select(s =>
                        "<script src=\"{0}\">\\x3C/script>".FormatWith(s.BuildAssetPath(isOptimized)));
                case AssetType.Style:
                    return
                        assets.Select(s =>
                            "<link href=\"{0}\" rel=\"stylesheet\" />".FormatWith(s.BuildAssetPath(isOptimized)));
                default:
                    return new List<string>();
            }
        }

        private static bool IsNotEmpty(Item optimizedScriptItem)
        {
            var mediaStream = ((MediaItem) optimizedScriptItem).GetMediaStream();
            return mediaStream?.Length > 0L;
        }

        private static string GetUrlContent(AssetType assetType, string url, string joinedAttributes)
        {
            var content = string.Empty;
            if (assetType == AssetType.Script)
            {
                if (!string.IsNullOrEmpty(url))
                    content += "<script src=\"{0}\" {1}></script>".FormatWith(url, joinedAttributes);
            }
            else if (assetType == AssetType.Style)
            {
                if (!string.IsNullOrEmpty(url))
                    content += "<link href=\"{0}\" rel=\"stylesheet\" {1} />".FormatWith(url, joinedAttributes);
            }

            return content;
        }

        private static string GetRawContent(AssetType assetType, string rawContent)
        {
            var content = string.Empty;
            switch (assetType)
            {
                case AssetType.Script:
                    if (!string.IsNullOrEmpty(rawContent)) content += "<script>{0}</script>".FormatWith(rawContent);
                    break;
                case AssetType.Style:
                    if (!string.IsNullOrEmpty(rawContent)) content += "<style>{0}</style>".FormatWith(rawContent);
                    break;
            }

            return content;
        }

        private static string GetEnumFieldValue(LookupField field)
        {
            var enumItem = field?.TargetItem;
            return enumItem?.Fields[Sitecore.XA.Foundation.Common.Templates.Enum.Fields.Value].Value;
        }
    }
}