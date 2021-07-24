using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleApp1
{
    public static class Helper
    {
        public static MethodInfo GetStringContainsMethodInfo = typeof(string)
           .GetMethod(nameof(string.Contains), new Type[] { typeof(string) });

        public static Func<object> GenerateGetter(string propName, object instance)
        {
            var objParameterExpr = Expression.Constant(instance);
            var propertyExpr = Expression.Property(objParameterExpr, propName);
            return Expression.Lambda<Func<object>>(propertyExpr).Compile();
        }

        public static Expression GetWhereWithPredicate(Type entityType, Expression queryParam, Expression body)
        {
            var delegateType = typeof(Func<,>).MakeGenericType(entityType, typeof(bool));

            var expressionType = typeof(Expression<>).MakeGenericType(delegateType);
            var queryableType = typeof(IQueryable<>).MakeGenericType(entityType);

            return Expression.Call(
                            // class with static method
                            typeof(Queryable),
                            // method and its generic argument
                            nameof(Queryable.Where), new Type[] { entityType },
                            // first argument - this IQueryable<tableObject>
                            queryParam,
                            // second argument - Expression<Func<TSource, bool>> predicate
                            body
                            );
        }

        public static Expression BuildPredicate(Type entityType,
            ParameterExpression parameterExpression,
            Expression bodyExpression)
        {
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(entityType, typeof(bool)),
                bodyExpression,
                parameterExpression);
        }
    }

}
