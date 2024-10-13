using System.Collections;
using System.Text.Json;

namespace DelegateRetry.Models;

internal static class Helpers
{
    internal static object? DeserializeParameter(object? param, Type targetType)
    {
        // If the parameter is already of the correct type, return it as-is
        if (param.GetType() == targetType || targetType.IsInstanceOfType(param))
        {
            return param;
        }

        // Convert the parameter to a JSON string if it's not already one
        string jsonParam = param is string stringParam ? stringParam : JsonSerializer.Serialize(param);

        // Handle primitive types
        if (targetType.IsPrimitive || targetType == typeof(string) || targetType == typeof(decimal))
        {
            return JsonSerializer.Deserialize(jsonParam, targetType);
        }

        // Handle nullable types
        if (Nullable.GetUnderlyingType(targetType) != null)
        {
            return JsonSerializer.Deserialize(jsonParam, targetType);
        }

        // Handle arrays
        if (targetType.IsArray)
        {
            var elementType = targetType.GetElementType();
            object?[]? arrayJson = JsonSerializer.Deserialize<object[]>(jsonParam);
            var array = Array.CreateInstance(elementType, arrayJson.Length);
            for (int i = 0; i < arrayJson.Length; i++)
            {
                array.SetValue(DeserializeParameter(arrayJson[i], elementType), i);
            }

            return array;
        }

        // Handle generic collections (List<T>, IEnumerable<T>, etc.)
        if (targetType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(targetType))
        {
            var genericArgs = targetType.GetGenericArguments();
            var listType = typeof(List<>).MakeGenericType(genericArgs);
            var list = JsonSerializer.Deserialize(jsonParam, listType);
            if (targetType.IsAssignableFrom(listType))
            {
                return list;
            }
            // If it's another type of collection, you might need to convert the list
            // to the specific collection type here.
        }

        // Handle dictionaries
        if (targetType.IsGenericType && typeof(IDictionary).IsAssignableFrom(targetType))
        {
            return JsonSerializer.Deserialize(jsonParam, targetType);
        }

        // For other complex types, use default deserialization
        return JsonSerializer.Deserialize(jsonParam, targetType);
    }

    internal static string? SerializeOutput(object? result, string errorMessage)
    {
        if (result != null)
        {
            return JsonSerializer.Serialize(result);
        }

        return !string.IsNullOrEmpty(errorMessage) ? JsonSerializer.Serialize(new { ErrorMessage = errorMessage }) : null; // if void or no error message
    }
}