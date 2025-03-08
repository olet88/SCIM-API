using System.Linq.Expressions;
using System.Reflection;

internal class ScimFilter
{
    internal required string PropertyName { get; set; }
    internal required string Operator { get; set; }
    internal required string Value { get; set; }

    internal static ScimFilter Parse(string filter)
    {
        var parts = filter.Split(new[] { " eq " }, StringSplitOptions.None);
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid filter format");
        }

        return new ScimFilter
        {
            PropertyName = parts[0].Trim(),
            Operator = "eq",
            Value = parts[1].Trim().Trim('"')
        };
    }

    internal Func<T, bool> ToLambda<T>()
    {
        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(param, PropertyName);

        var propertyType = ((PropertyInfo)property.Member).PropertyType;
        var typedValue = Convert.ChangeType(Value, propertyType);
        var constant = Expression.Constant(typedValue);

        Expression comparison = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(comparison, param);

        return lambda.Compile();
    }
}