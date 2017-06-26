using System.Web;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Westco.XA.Foundation.Rules.Conditions.Cookie
{
    public class CookieExists<T> : WhenCondition<T> where T : RuleContext
    {
        public string CookieName { get; set; }

        protected override bool Execute(T ruleContext)
        {
            if (string.IsNullOrEmpty(CookieName)) { return false; }

            return HttpContext.Current.Request.Cookies[CookieName] != null;
        }
    }
}
