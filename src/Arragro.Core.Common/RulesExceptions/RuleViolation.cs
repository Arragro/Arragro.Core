using Arragro.Core.Common.Helpers;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Arragro.Core.Common.RulesExceptions
{
    /*
     * Taken from Steve Sanderson's Pro ASP.Net MVC 2 book around validation
     * of models, doesn't seem to be in the later books.
     * It is useful for the validation to occur at the service layer (business
     * layer) as the service layer then doesn't depend on MVC at all.  It will
     * throw the RulesException IF there are any error.  When it does, there
     * is an extension in another Arragro library that will copy these issues
     * to the ModelState.  ModelState is still validated by the MVC framework.
     */

    public class RuleViolation
    {
        public string Prefix { get; set; }

        public string Key { get; set; }

        public string Message { get; set; }

        private LambdaExpression Property { get; set; }

        private string GetPropertyPath()
        {
            var stack = new Stack<string>();

            MemberExpression me;
            switch (Property.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = Property.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = Property.Body as MemberExpression;
                    break;
            }

            while (me != null)
            {
                stack.Push(me.Member.Name);
                me = me.Expression as MemberExpression;
            }

            return string.Join(".", stack.ToArray());
        }

        public KeyValuePair<string, object> KeyValuePair
        {
            get
            {
                if (string.IsNullOrEmpty(Prefix))
                    return new KeyValuePair<string, object>(Key, Message);
                else
                    return new KeyValuePair<string, object>(string.Format("{0}.{1}", Prefix, Key), Message);
            }
        }

        public RuleViolation(LambdaExpression property, string message, string prefix = null)
        {
            Prefix = string.IsNullOrEmpty(prefix) ? prefix : prefix + ".";
            Property = property;
            Key = ExpressionHelper.GetExpressionText(property);
            Message = message;
        }

        public RuleViolation(string key, string message, string prefix = null)
        {
            Prefix = string.IsNullOrEmpty(prefix) ? prefix : prefix + ".";
            Key = key;
            Message = message;
        }
    }
}
