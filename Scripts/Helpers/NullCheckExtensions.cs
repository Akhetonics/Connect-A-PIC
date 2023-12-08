using ConnectAPic.LayoutWindow;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.Helpers
{
    public static class NullCheckExtensions
    {
        public static void CheckForNull<T>(this T instance, Expression<Func<T, object>> expr) where T : Node
        {
            var body = expr.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression uBody = (UnaryExpression)expr.Body;
                body = uBody.Operand as MemberExpression;
            }

            string variableName = body.Member.Name;
            var compile = expr.Compile();
            if (compile(instance) == null)
            {
                string className = instance.GetType().Name;
                string errorMsg = $"'{variableName}' of this element is not set in the class: '{className}'";
                GameManager.instance.Logger.PrintErr(errorMsg);
                GD.PrintErr(errorMsg);
            }
        }
    }
}
