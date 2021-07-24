using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleApp1.Actions
{
    public abstract class DynamicAction
    {
        public abstract Expression GetQueryExpression(Type entityType, PropertyInfo propertyInfo, Expression query,
            object argument);

        public abstract object GetResult(IQueryable q);
    }
}
