using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleApp1.Actions
{
    /// <summary>
    /// Builds where predicate with property greater then or equal provided argument
    /// </summary>
    public class GTEBy : DynamicAction
    {
        public override Expression GetQueryExpression(Type entityType, PropertyInfo propertyInfo, Expression query,
            object argument)
        {
            var parameterExpression = Expression.Parameter(entityType, "x");
            var propertyExpression = Expression.Property(parameterExpression, propertyInfo.Name);
            var valueExpression = Expression.Constant(argument, propertyInfo.PropertyType);

            var body = GetBodyExpression(propertyInfo.PropertyType, propertyExpression, valueExpression);

            var wherePredicate = Helper.BuildPredicate(entityType, parameterExpression, body);

            return Helper.GetWhereWithPredicate(entityType, query, wherePredicate);
        }
        public override object GetResult(IQueryable q) => q;

        protected virtual Expression GetBodyExpression(Type propertyType, MemberExpression property, ConstantExpression valueExpression)
        {
            if (propertyType == typeof(string))
                throw new InvalidOperationException($"Property {property.Member.Name} of type {propertyType.Name} can't " +
                    $"use {GetType().Name} action");

            return Expression.GreaterThanOrEqual(property, valueExpression);
        }
    }
}
