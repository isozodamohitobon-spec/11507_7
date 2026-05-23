using System;
using System.Collections.Generic;
using System.Reflection;

public class TypeFactory
{
    public static object CreateAndFill(Type type, Dictionary<string, object> values)
    {
        var instance = Activator.CreateInstance(type);
        
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        foreach (var property in properties)
        {
            if (values.TryGetValue(property.Name, out var value))
            {
              
                if (value != null && !property.PropertyType.IsAssignableFrom(value.GetType()))
                {
                    try
                    {
                        value = Convert.ChangeType(value, property.PropertyType);
                    }
                    catch
                    {
                        continue; 
                    }
                }
                
            
                property.SetValue(instance, value);
            }
        }

        return instance;
    }
}
