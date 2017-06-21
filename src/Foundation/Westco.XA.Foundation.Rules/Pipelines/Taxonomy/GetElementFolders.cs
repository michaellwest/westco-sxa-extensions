using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines.Rules.Taxonomy;
using Sitecore.Rules;
using Sitecore.SecurityModel;

namespace Westco.XA.Foundation.Rules.Pipelines.Taxonomy
{
    public class GetElementFolders : GetRuleElementProcessor
    {
        public List<string> Folders { get; set; } = new List<string>();

        protected virtual IEnumerable<Item> GetConditionFolders(Item tag)
        {
            var database = tag.Database;

            var elementFolderItems = new List<Item>();
            foreach (var folder in Folders)
            {
                var elementsInFolders = GetElements(database, folder);
                elementFolderItems.AddRange(elementsInFolders);
            }
            return elementFolderItems.Where(i =>
            {
                if (i.TemplateID == RuleIds.ElementFolderTemplateID)
                    return i.Children.Any(f =>
                    {
                        if (f.TemplateID == RuleIds.TagDefinitionsFolderTemplateID)
                            return f.Children.Any(k => k["Tags"].Contains(tag.ID.ToString()));
                        return false;
                    });
                return false;
            }).ToArray();
        }

        protected virtual IEnumerable<Item> GetElements(Database database, string rootPath)
        {
            IEnumerable<Item> elementFolders;
            using (new SecurityDisabler())
            {
                elementFolders = database.SelectItems($"{rootPath}//*[@@templateid='{ RuleIds.ElementFolderTemplateID}']");
            }
            return elementFolders;
        }

        protected override void Action(Dictionary<ID, Item> result, RuleElementsPipelineArgs args)
        {
            foreach (Item tag in args.Tags.Values)
            {
                foreach (Item conditionFolder in this.GetConditionFolders(tag))
                {
                    if (!result.ContainsKey(conditionFolder.ID))
                        result.Add(conditionFolder.ID, conditionFolder);
                }
            }
        }

        protected override Dictionary<ID, Item> GetResultCollection(RuleElementsPipelineArgs args)
        {
            return args.ElementFolders;
        }
    }
}