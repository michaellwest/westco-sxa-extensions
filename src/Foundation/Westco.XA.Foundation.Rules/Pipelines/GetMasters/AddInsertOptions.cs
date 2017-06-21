using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetMasters;
using Sitecore.Rules;
using Sitecore.Rules.InsertOptions;
using Sitecore.SecurityModel;

namespace Westco.XA.Foundation.Rules.Pipelines.GetMasters
{
    public class AddInsertOptions
    {
        private readonly ID _insertOptionsRuleTemplateId = new ID("{664E5035-EB8C-4BA1-9731-A098FCC9127A}");

        public List<string> Folders { get; set; }

        public AddInsertOptions()
        {
            Folders = new List<string>();
        }

        public void Process(GetMastersArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            var database = args.Item.Database;
            var insertOptionRules = new List<Item>();
            foreach (var folder in Folders)
            {
                var rulesInFolder = GetInsertOptionsRules(database, folder);
                insertOptionRules.AddRange(rulesInFolder);
            }

            var rules = RuleFactory.GetRules<InsertOptionsRuleContext>(insertOptionRules, "Rule");
            if (rules == null) return;

            var ruleContext = new InsertOptionsRuleContext
            {
                Item = args.Item,
                InsertOptions = args.Masters
            };
            rules.Run(ruleContext);
        }

        protected virtual IEnumerable<Item> GetInsertOptionsRules(Database database, string rootPath)
        {
            IEnumerable<Item> insertOptionsRules;
            using (new SecurityDisabler())
            {
                insertOptionsRules = database.SelectItems($"{rootPath}//*[@@templateid='{_insertOptionsRuleTemplateId}']");
            }
            return insertOptionsRules;
        }
    }
}
