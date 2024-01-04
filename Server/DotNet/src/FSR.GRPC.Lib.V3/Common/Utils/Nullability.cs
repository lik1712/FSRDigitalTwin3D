using System.Collections;
using System.Reflection;

namespace FSR.GRPC.Lib.V3.Common.Utils;

public class Nullability {
    public static void SetEmptyPropertiesToNull<T>(T obj)
    {
        Type type = obj?.GetType() ?? typeof(T);

        foreach (PropertyInfo property in type.GetProperties())
        {
            if (property.PropertyType == typeof(string))
            {
                string? value = (string?) property.GetValue(obj);
                if (string.IsNullOrEmpty(value))
                {
                    property.SetValue(obj, null);
                }
            }
            else if (property.PropertyType == typeof(byte[]))
            {
                byte[]? value = (byte[]?) property.GetValue(obj);
                if (value != null && value.Length == 0) 
                {
                    property.SetValue(obj, null);
                }
            }
            else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                IList? list = (IList?) property.GetValue(obj);
                if (list != null && list.Count == 0)
                {
                    property.SetValue(obj, null);
                }
            }
        }
    }
}