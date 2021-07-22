using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleApp1
{
    public class DynamicTable : DynamicObject
    {
        private static MethodInfo _stringContainsMethodInfo = typeof(string)
            .GetMethod(nameof(string.Contains), new Type[] { typeof(string) });



        private (Func<object> getter, PropertyInfo[] properties) _entityData;

        public DynamicTable((Func<object> getter, PropertyInfo[] properties) data)
        {
            _entityData = data;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var name = binder.Name.ToLower();

            // TODO: findfirstby, anyby
            var action = "searchby";

            if (!name.StartsWith(action))
                throw new NotImplementedException("Only search by value is implemented");

            var field = name.Replace(action, "").ToLower();

            var tableObject = _entityData.getter() as IQueryable;
            var tableType = tableObject.GetType().GetGenericArguments()[0];

            var propertyInfo = _entityData.properties.Where(x => x.Name.ToLower() == field).SingleOrDefault();
            if (propertyInfo == null)
                throw new Exception($"Property {field} is not found in {tableType.Name}");

            var predicate = BuildPredicate(tableType, propertyInfo, args[0]);


            // TODO: Dictionary<string action, Func<in IQueryable, in entityType, out object>
            var query = tableObject.Provider
                .CreateQuery(Helper.GetWhereWithPredicate(tableType, tableObject.Expression, predicate));
            
            result = query;

            return true;

        }

        private Expression BuildPredicate(Type entityType, PropertyInfo propertyInfo, object value)
        {
            var parameterExpression = Expression.Parameter(entityType, "x");
            var propertyExpression = Expression.Property(parameterExpression, propertyInfo.Name);
            var valueExpression = Expression.Constant(value, propertyInfo.PropertyType);

            var body = GetWhereBodyExpression(propertyInfo.PropertyType,
                propertyExpression,
                valueExpression);

            return Expression.Lambda(typeof(Func<,>).MakeGenericType(entityType, typeof(bool)),
                body,
                parameterExpression);
        }

        private Expression GetWhereBodyExpression(Type propertyType, Expression property, Expression value)
        {
            if(propertyType == typeof(string))
            {
                return Expression.Call(property, _stringContainsMethodInfo, value);
            }
            return Expression.Equal(property, value);
        }
    }

}
