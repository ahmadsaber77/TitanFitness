using System.Reflection;

namespace TitanFitness.Domain.Common;

public abstract class Enumeration
{
    public int Id { get; }
    public string Name { get; }

    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static IEnumerable<T> GetAll<T>()
        where T : Enumeration
    {
        return typeof(T)
            .GetFields(
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly)
            .Where(field => field.FieldType == typeof(T))
            .Select(field => (T)field.GetValue(null)!);
    }
}
