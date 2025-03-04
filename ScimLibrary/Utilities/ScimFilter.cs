using System.Linq.Expressions;
using System.Reflection;

public class ScimFilter
{
    public required string PropertyName { get; set; }
    public required string Operator { get; set; }
    public required string Value { get; set; }

    public static ScimFilter Parse(string filter)
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
            Value = parts[1].Trim().Trim('"') // Removing potential double quotes
        };
    }

    public Func<T, bool> ToLambda<T>()
    {
        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(param, PropertyName);

        // Convert value to the property type
        var propertyType = ((PropertyInfo)property.Member).PropertyType;
        var typedValue = Convert.ChangeType(Value, propertyType);
        var constant = Expression.Constant(typedValue);

        Expression comparison = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(comparison, param);

        return lambda.Compile();
    }
}