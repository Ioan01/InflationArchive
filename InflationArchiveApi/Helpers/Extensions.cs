using System.Linq.Expressions;
using System.Reflection;

namespace InflationArchive.Helpers
{
    public static class Extensions
    {
        public static string OnlyFirstCharToUpper(this string input)
        {
            return input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.ToLowerInvariant().AsSpan(1))
            };
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, string propertyName, bool descending = false)
        {
            var entityType = typeof(TSource);

            // Create x=>x.PropName
            var propertyInfo = entityType.GetProperty(propertyName)!;
            if (propertyInfo.DeclaringType != entityType)
            {
                propertyInfo = propertyInfo.DeclaringType!.GetProperty(propertyName);
            }

            // If we try to order by a property that does not exist in the object return the list
            if (propertyInfo == null)
            {
                return (IOrderedQueryable<TSource>)query;
            }

            var arg = Expression.Parameter(entityType, "x");
            var property = Expression.MakeMemberAccess(arg, propertyInfo);
            var selector = Expression.Lambda(property, arg);

            var methodName = "OrderBy";

            if (descending)
            {
                methodName = $"{methodName}Descending";
            }

            // Get System.Linq.Queryable.OrderBy() method.
            MethodInfo method = typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == methodName && m.IsGenericMethodDefinition) // ensure selecting the right overload
                .Single(static m => m.GetParameters().ToList().Count == 2);

            //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);

            /* Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it. */
            return (IOrderedQueryable<TSource>)genericMethod.Invoke(genericMethod, new object[] { query, selector })!;
        }
    }
}
