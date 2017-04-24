using System;
using System.Linq;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.StringExtensions;
using Sitecore.XA.Foundation.Theming.Pipelines.AssetService;

namespace Company.XA.Foundation.Theming.Pipelines.AssetService
{
    public class AddPlainIncludes : AddAssetsProcessor
    {
        public override void Process(AssetsArgs args)
        {
            if (!IsSxaPage()) return;

            var contextItem = Context.Item;
            if (contextItem == null) return;

            var pageAssetsIds = contextItem[Templates.Page.Fields.PageAssets]?.Split('|');

            var assetsList = args.AssetsList;
            const int num = 0;

            var plainIncludeItems = contextItem.Axes.SelectItems("/sitecore/media library//*[@@templateid='{0}']".FormatWith(Templates.PlainInclude.Id));

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

                var url = plainIncludeItem[Templates.PlainInclude.Fields.AssetUrl];
                var hash = plainIncludeItem[Templates.PlainInclude.Fields.SriHash];
                var cors = GetEnumFieldValue(plainIncludeItem.Fields[Templates.PlainInclude.Fields.Cors]);
                var content = string.Empty;

                switch (assetType)
                {
                    case AssetType.Script:
                        if (!string.IsNullOrEmpty(plainIncludeItem[Templates.PlainInclude.Fields.AssetUrl]))
                        {
                            content += "<script src=\"{0}\" integrity=\"{1}\" crossorigin=\"{2}\"></script>".FormatWith(url, hash, cors);
                        }

                        if (!string.IsNullOrEmpty(plainIncludeItem[Templates.PlainInclude.Fields.RawContent]))
                        {
                            content += "<script>{0}</script>".FormatWith(plainIncludeItem[Templates.PlainInclude.Fields.RawContent]);
                        }
                        break;
                    case AssetType.Style:
                        if (!string.IsNullOrEmpty(plainIncludeItem[Templates.PlainInclude.Fields.AssetUrl]))
                        {
                            content += "<link href=\"{0}\" rel=\"stylesheet\" integrity=\"{1}\" crossorigin=\"{2}\" </>".FormatWith(url, hash, cors);
                        }

                        if (!string.IsNullOrEmpty(plainIncludeItem[Templates.PlainInclude.Fields.RawContent]))
                        {
                            content += "<link href=\"{0}\" rel=\"stylesheet\" />".FormatWith(plainIncludeItem[Templates.PlainInclude.Fields.RawContent]);
                        }
                        break;
                    default:
                        continue;
                }

                if (string.IsNullOrEmpty(content)) continue;

                var plainInclude = new PlainInclude
                {
                    SortOrder = num,
                    Name = plainIncludeItem.Name,
                    Type = assetType,
                    Content = content
                };
                assetsList.Add(plainInclude);
            }
        }

        private static string GetEnumFieldValue(LookupField field)
        {
            var enumItem = field?.TargetItem;
            return enumItem?.Fields[Sitecore.XA.Foundation.Common.Templates.Enum.Fields.Value].Value;
        }
    }
}
