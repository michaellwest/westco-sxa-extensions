using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Westco.XA.Foundation.Rules.Conditions.Cookie
{
    public class CookieKeyValue<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public string CookieName { get; set; }
        public string CookieKey { get; set; }
        public string CookieValue { get; set; }

        protected override bool Execute(T ruleContext)
        {
            if (string.IsNullOrEmpty(CookieName)) return false;

            if (string.IsNullOrEmpty(CookieKey)) return false;

            if (string.IsNullOrEmpty(CookieValue)) return false;

            if (HttpContext.Current.Request.Cookies[CookieName] == null) return false;

            var json = HttpContext.Current.Request.Cookies[CookieName].Value;
            if (string.IsNullOrEmpty(json)) return false;

            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(HttpUtility.UrlDecode(json));
            if (dictionary == null || !dictionary.ContainsKey(CookieKey)) return false;

            return Compare(CookieValue, dictionary[CookieKey]);
        }
    }
}