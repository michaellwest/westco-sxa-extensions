using System.Web;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Westco.XA.Foundation.Rules.Conditions.Cookie
{
    public class CookieStringValue<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public string CookieName { get; set; }
        public string CookieValue { get; set; }

        protected override bool Execute(T ruleContext)
        {
            if (string.IsNullOrEmpty(CookieName))
                return false;

            if (string.IsNullOrEmpty(CookieValue))
                return false;

            if (HttpContext.Current.Request.Cookies[CookieName] == null)
                return false;

            var actualVal = HttpContext.Current.Request.Cookies[CookieName].Value;

            return !string.IsNullOrEmpty(actualVal) && Compare(CookieValue, actualVal);
        }
    }
}