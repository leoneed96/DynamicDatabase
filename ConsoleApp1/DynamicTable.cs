using ConsoleApp1.Actions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace ConsoleApp1
{
    public class DynamicTable : DynamicObject
    {
        private (Func<object> getter, PropertyInfo[] properties) _entityData;

        private Dictionary<string, DynamicAction> _actions = new Dictionary<string, DynamicAction>()
        {
            { "searchby", new SearchBy() },
            { "gteby", new GTEBy() }
        };

        public DynamicTable((Func<object> getter, PropertyInfo[] properties) data)
        {
            _entityData = data;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var name = binder.Name.ToLower();

            var keys = _actions.Keys.Where(x => name.StartsWith(x));
            var keysCount = keys.Count();

            if (keysCount == 0)
                throw new NotSupportedException($"{binder.Name} is not supported");
            if (keysCount > 1)
                throw new AmbiguousMatchException($"Multiple actions found for requested method {binder.Name}");

            var key = keys.First();

            var field = name.Substring(key.Length);
            var action = _actions[key];

            var tableObject = _entityData.getter() as IQueryable;
            var tableType = tableObject.GetType().GetGenericArguments()[0];

            var propertyInfo = _entityData.properties.Where(x => x.Name.ToLower() == field).SingleOrDefault();
            if (propertyInfo == null)
                throw new Exception($"Property {field} is not found in {tableType.Name}");

            var expression = action.GetQueryExpression(tableType, propertyInfo, tableObject.Expression, args[0]);

            var query = tableObject.Provider.CreateQuery(expression);

            result = action.GetResult(query);

            return true;
        }
    }
}
