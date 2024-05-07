using System.Reflection;

namespace BlockchainHistoryRecorder.Common.Extensions;

public static class EntityExtensions
{
    public static (string columnNames, string bindMarkers, object?[] propertiesValue) ExtractEntityProperties<T>(
        this PropertyInfo[] properties, T entity)
    {
        var columnNames = new List<string>();
        var bindMarkers = new List<string>();
        var propertiesValue = new List<object?>();

        foreach (var property in properties)
        {
            columnNames.Add(property.Name);
            bindMarkers.Add("?");
            propertiesValue.Add(property.GetValue(entity));
        }

        return (string.Join(", ", columnNames), string.Join(", ", bindMarkers), propertiesValue.ToArray());
    }
}