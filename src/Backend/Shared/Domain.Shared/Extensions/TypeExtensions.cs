using System.Reflection;

namespace Domain.Shared.Extensions
{
    public class SkipPropertyAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnNameAttribute : Attribute
    {
        public string Name { get; }
        public ColumnNameAttribute(string name) => Name = name;
    }

    public static class TypeExtensions
    {
        public static PropertyInfo[] GetFilteredProperties(this Type type)
        {
            return type.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(SkipPropertyAttribute), true).Length == 0).ToArray();
        }

        public static string GetColumnName(this PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<ColumnNameAttribute>();
            return attr?.Name ?? prop.Name;
        }
    }
}
