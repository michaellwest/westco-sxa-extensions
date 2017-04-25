﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.StringExtensions;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Sitecore.XA.Foundation.Theming.Configuration;
using Sitecore.XA.Foundation.Theming.Pipelines.AssetService;

namespace Company.XA.Foundation.Theming.Pipelines.AssetService
{
    public class AddPlainIncludes : AddAssetsProcessor
    {
        public override void Process(AssetsArgs args)
        {
            if (!IsSxaPage()) return;
            var assetConfiguration = AssetConfigurationReader.Read();

            var contextItem = Context.Item;
            if (contextItem == null) return;

            var pageAssetsIds = contextItem[Templates.Page.Fields.PageAssets]?.Split('|');

            var assetsList = args.AssetsList;
            const int num = 0;

            var plainIncludeItems = contextItem.Axes.SelectItems("/sitecore/media library//*[@@templateid='{0}']".FormatWith(Templates.PlainInclude.Id));
            if (!plainIncludeItems.Any()) return;

            foreach (var plainIncludeItem in plainIncludeItems)
            {
                var mode = GetEnumFieldValue(plainIncludeItem.Fields[Templates.PlainInclude.Fields.Mode]);
                if (string.IsNullOrEmpty(mode) || mode.Equals("Disabled", StringComparison.OrdinalIgnoreCase)) continue;

                var isGlobalAsset = MainUtil.GetBool(plainIncludeItem[Templates.PlainInclude.Fields.IsGlobalAsset], false);
                if (!isGlobalAsset)
                {
                    if (pageAssetsIds == null || !pageAssetsIds.Any() || !pageAssetsIds.Contains(plainIncludeItem.ID.ToString()))
                    {
                        continue;
                    }
                }

                var assetType = (AssetType)Enum.Parse(typeof(AssetType), mode);

                var attributes = new Dictionary<string, string>
                {
                    {"integrity", plainIncludeItem[Templates.PlainInclude.Fields.SriHash]},
                    {"crossorigin", GetEnumFieldValue(plainIncludeItem.Fields[Templates.PlainInclude.Fields.Cors])}
                };

                var url = plainIncludeItem[Templates.PlainInclude.Fields.AssetUrl];
                var rawContent = plainIncludeItem[Templates.PlainInclude.Fields.RawContent];
                var joinedAttributes = string.Join(" ", attributes.Where(a => !a.Value.IsNullOrEmpty()).Select(a => $"{a.Key}=\"{a.Value}\""));

                var content = GetContent(assetType, url, rawContent, joinedAttributes);

                if (string.IsNullOrEmpty(content)) continue;

                var plainInclude = new PlainInclude
                {
                    SortOrder = num,
                    Name = plainIncludeItem.Name,
                    Type = assetType,
                    Content = content
                };
                assetsList.Add(plainInclude);

                var isFallbackEnabled = MainUtil.GetBool(plainIncludeItem[Templates.PlainInclude.Fields.IsFallbackEnabled], false);
                if (!isFallbackEnabled) continue;

                var fallbackTest = plainIncludeItem[Templates.PlainInclude.Fields.FallbackTest];
                if(string.IsNullOrEmpty(fallbackTest)) continue;

                var tagBuilder = new StringBuilder();
                foreach (var script in QueryAssets(plainIncludeItem, AssetType.Script).Where(i => i.TemplateID != Sitecore.XA.Foundation.Theming.Templates.OptimizedFile.ID).Select(s => "<script src=\"{0}\">\\x3C/script>".FormatWith(s.BuildAssetPath(AssetServiceMode.Disabled != assetConfiguration.ScriptsMode))))
                {
                    tagBuilder.Append(script);
                }

                foreach (var link in QueryAssets(plainIncludeItem, AssetType.Style).Where(i => i.TemplateID != Sitecore.XA.Foundation.Theming.Templates.OptimizedFile.ID).Select(s => "<link href=\"{0}\" rel=\"stylesheet\" />".FormatWith(s.BuildAssetPath(AssetServiceMode.Disabled != assetConfiguration.StylesMode))))
                {
                    tagBuilder.Append(link);
                }

                if (tagBuilder.Length > 0)
                {
                    var fallbackInclude = new PlainInclude
                    {
                        SortOrder = num,
                        Name = plainIncludeItem.Name + "-fallback",
                        Type = AssetType.Script,
                        Content = "<script>{0} || document.write('{1}')</script>".FormatWith(fallbackTest, tagBuilder.ToString())
                    };
                    assetsList.Add(fallbackInclude);
                }
            }
        }

        private List<Item> QueryAssets(Item plainIncludeItem, AssetType assetType)
        {
            Item[] selectedItems = null;
            if (assetType == AssetType.Script)
            {
                selectedItems = plainIncludeItem.Axes.SelectItems("./Scripts//*[@Extension='js']");
            }
            else if (assetType == AssetType.Style)
            {
                selectedItems = plainIncludeItem.Axes.SelectItems("./Styles//*[@Extension='css']");
            }

            return (selectedItems != null && selectedItems.Any()) ? new List<Item>(selectedItems) : new List<Item>();
        }

        private static string GetContent(AssetType assetType, string url, string rawContent, string joinedAttributes)
        {
            var content = string.Empty;
            if (assetType == AssetType.Script)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    content += "<script src=\"{0}\" {1}></script>".FormatWith(url, joinedAttributes);
                }

                if (!string.IsNullOrEmpty(rawContent))
                {
                    content += "<script>{0}</script>".FormatWith(rawContent);
                }
            }
            else if (assetType == AssetType.Style)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    content += "<link href=\"{0}\" rel=\"stylesheet\" {1} />".FormatWith(url, joinedAttributes);
                }

                if (!string.IsNullOrEmpty(rawContent))
                {
                    content += "<style>{0}</style>".FormatWith(rawContent);
                }
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
