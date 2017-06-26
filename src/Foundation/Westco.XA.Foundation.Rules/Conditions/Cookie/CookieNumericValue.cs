using System;
using System.Web;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Westco.XA.Foundation.Rules.Conditions.Cookie
{
    public class CookieNumericValue<T> : OperatorCondition<T> where T : RuleContext
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

            if (!double.TryParse(CookieValue, out double parsedTestVal))
                return false;

            var actualVal = HttpContext.Current.Request.Cookies[CookieName].Value;

            if (string.IsNullOrEmpty(actualVal))
                return false;

            if (!double.TryParse(actualVal, out double parsedCookieVal))
                return false;

            var op = GetOperator();
            return ValuesCompare(parsedTestVal, parsedCookieVal, op);
        }

        private static bool ValuesCompare(double val1, double val2, ConditionOperator op)
        {
            switch (op)
            {
                case ConditionOperator.Equal:
                    return Math.Abs(val1 - val2) < 1;
                case ConditionOperator.GreaterThanOrEqual:
                    return val1 >= val2;
                case ConditionOperator.GreaterThan:
                    return val1 > val2;
                case ConditionOperator.LessThanOrEqual:
                    return val1 <= val2;
                case ConditionOperator.LessThan:
                    return val1 < val2;
                case ConditionOperator.NotEqual:
                    return Math.Abs(val1 - val2) > 1;
                default:
                    return false;
            }
        }
    }
}