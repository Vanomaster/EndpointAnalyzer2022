using System.Dynamic;
using System.Management;

namespace Queries.NonDatabase;

/// <summary>
/// Wmi query for easy access to the Windows Management Instrumentation (WMI) infrastructure.
/// </summary>
public class WmiQuery
{
    public static List<object> GetAllObjects(string className)
    {
        var objectCollection = new ManagementObjectSearcher("SELECT * FROM " + className).Get();
        var allObjects = new List<object>();
        foreach (ManagementObject managementObject in objectCollection)
        {
            var dictionary = (IDictionary<string, object>)new ExpandoObject();
            foreach (var property in managementObject.Properties)
            {
                dictionary.Add(property.Name, property.Value);
            }

            allObjects.Add(dictionary);
        }

        return allObjects;
    }

    public static object GetObject(string className)
    {
        var objectCollection = new ManagementObjectSearcher("SELECT * FROM " + className).Get();
        var dictionary = (IDictionary<string, object>)new ExpandoObject();
        using var enumerator = objectCollection.GetEnumerator();
        if (enumerator.MoveNext())
        {
            foreach (var property in enumerator.Current.Properties)
            {
                dictionary.Add(property.Name, property.Value);
            }
        }

        return dictionary;
    }
}