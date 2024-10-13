using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using DelegateRetry.Models;

namespace DelegateRetry.Core;

internal static class MethodRegistry
                                               // ( because we can hold the state of instance , all what we can do is to hold inputs values and impl)
{
    private static readonly Dictionary<string, Delegate> RegisteredMethods = new ();
    private static bool _isInitialized = false;

    internal static void CollectRegisteredMethods(params Assembly[]? assemblies)
    {
        if (_isInitialized)
        {
            Console.WriteLine("Methods have already been collected. Skipping...");
            return;
        }

        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                    .Where(m => m.GetCustomAttributes(typeof(RegisterAttribute), false).Length > 0);

                
                foreach (var method in methods)
                {
                    var name = $"{type.Name}.{method.Name}";

                    if (method.IsStatic)
                    {
                        RegisteredMethods[name] = Delegate.CreateDelegate(Expression.GetDelegateType(
                            (from parameter in method.GetParameters() select parameter.ParameterType)
                            .Concat(new[] { method.ReturnType })
                            .ToArray()), method);
                    }
                    else
                    {
                        throw new InvalidOperationException($"The method '{method.Name}' in type '{type.Name}' is not static and cannot be registered with RegisterAttribute.");
                    }
                    Console.WriteLine($"Registered method: {name} from assembly {assembly.GetName().Name}");
                }
                
                
            }
        }

        _isInitialized = true;
    }
    internal static Delegate? GetRegisteredMethod(string name)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("MethodRegistry has not been initialized. Call CollectRegisteredMethods first.");
        }

        return RegisteredMethods.GetValueOrDefault(name);
    }
    internal static string? Execute(Delegate function, object?[] parameters)
    {
        var isVoid = function.Method.ReturnType == typeof(void);
        
        // Get parameter types and filter out closure parameters
        var paramTypes = function.Method.GetParameters().Select(p => p.ParameterType).ToArray();
        
        object?[] castedParams = new object[paramTypes.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            if (i < parameters.Length)
            {
                castedParams[i] = Helpers.DeserializeParameter(parameters[i], paramTypes[i]);
            }
            else
            {
                castedParams[i] = Type.Missing; // For optional parameters
            }

        }

        // Invoke the function
        var invokeResult = function.DynamicInvoke(castedParams);

        // Handle the result
        return isVoid ? null : JsonSerializer.Serialize(invokeResult);
    }
    
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RegisterAttribute : Attribute;

